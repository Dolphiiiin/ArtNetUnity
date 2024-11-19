#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using ArtNetUnity.ArtNet;

namespace ArtNetUnity.Editor
{
    public class ANUArtNetReceiverWindow : EditorWindow
    {
        // ArtNetReceiverのインスタンス
        private ANUArtNetReceiver _receiver;
        private VisualElement _channelDataField;

        private bool _sideMenuFolded = true;
        private int _selectedChannel = 1;
        private const int ValueBarHeight = 32; 

        [MenuItem("Tools/ArtNetUnity/ANUArtNetReceiver")]
        public static void ShowWindow()
        {
            // ANUArtNetReceiverWindowを開く (複数ウィンドウを開くことが可能)
            ANUArtNetReceiverWindow wnd = CreateInstance<ANUArtNetReceiverWindow>();
            wnd.Show();            
            wnd.titleContent = new GUIContent("ANUArtNetReceiver");
        }

        public void CreateGUI()
        {
            // ベースとなるUI ArtNetReceiverWindow.uxml
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ArtNetUnity/Scripts/Editor/ArtNetReceiverWindow.uxml");
            visualTree.CloneTree(rootVisualElement);

            var channelDataScrollView = rootVisualElement.Q<ScrollView>("channelDataScrollView");
            _channelDataField = channelDataScrollView.Q<VisualElement>("channelDataField");

            InitializeUI(_channelDataField);

            // ANUArtNetReciverReciverの選択フィールドをUIのルートに追加
            ObjectField receiverField = new ObjectField("Receiver")
            {
                objectType = typeof(ANUArtNetReceiver),
                value = _receiver
            };
            receiverField.RegisterValueChangedCallback(evt =>
            {
                _receiver = evt.newValue as ANUArtNetReceiver;
                UpdateChannelData(_channelDataField);
                UpdateReceiverStats();
            });
            rootVisualElement.Insert(0, receiverField);

            // プレイモードの変更時のイベントを登録
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // SideMenuの折り畳みボタン
            var sideMenuFoldButton = rootVisualElement.Q<Button>("SideMenuFoldButton");
            sideMenuFoldButton.clickable.clicked += () =>
            {
                _sideMenuFolded = !_sideMenuFolded;
                var sideMenu = rootVisualElement.Q<VisualElement>("SideMenu");
                
                // true: 展開状態, false: 折り畳み状態
                sideMenu.style.display = _sideMenuFolded ? DisplayStyle.Flex : DisplayStyle.None;

                Debug.Log(_sideMenuFolded);

            };
        }
        

        /// <summary>
        /// プレイモードの変更時のイベント。プレイモードが変更されたときに、ArtNetReceiverを再取得する
        /// </summary>
        /// <param name="state"></param>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _receiver = FindObjectOfType<ANUArtNetReceiver>();
                UpdateChannelData(_channelDataField);
                UpdateReceiverStats();
            }
        }

        /// <summary>
        /// ArtNetReceiverを通じて、ArtNetEndpointの情報をUIに表示する
        /// </summary>
        private void UpdateReceiverStats()
        {
            var nodeAddressLabel = rootVisualElement.Q<Label>("NodeAddressNum");
            var universeLabel = rootVisualElement.Q<Label>("UniverseStats");

            if (_receiver != null)
            {
                nodeAddressLabel.text = _receiver.SpecificIPAddress;    // ArtNetエンドポイントのIPアドレスを表示
                universeLabel.text = _receiver.selectedUniverse.ToString(); // 選択されたユニバース番号を表示
            }
            else
            {
                // ArtNetReceiverが存在しない場合はN/Aを表示
                nodeAddressLabel.text = "N/A";
                universeLabel.text = "N/A";
            }
        }

        /// <summary>
        /// チャンネルデータのUIを初期化する
        /// </summary>
        /// <param name="channelDataField"></param>
        private void InitializeUI(VisualElement channelDataField)
        {
            // チャンネルデータのUIを初期化
            for (int i = 1; i <= 512; i++)
            {
                var container = new VisualElement();
                container.style.width = 25;
                container.style.height = 50;
                container.style.marginTop = 5;
                container.style.marginRight = 5;
                container.style.marginBottom = 5;
                container.style.marginLeft = 5;
                container.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);

                int channelIndex = i;
                
                // チャンネル選択ボタン
                var channelSelection = new Button(() => SelectChannel(channelIndex))
                {
                    text = channelIndex.ToString(),
                    style =
                    {
                        fontSize = 10,
                        marginTop = 1,
                        marginRight = 1,
                        marginBottom = 1,
                        marginLeft = 1,
                        paddingTop = 0,
                        paddingRight = 0,
                        paddingBottom = 0,
                        paddingLeft = 0
                    }
                };
                container.Add(channelSelection);

                // チャンネルの値を表示するバー
                var valueBar = new VisualElement();
                valueBar.style.marginTop = 0;
                valueBar.style.marginRight = 0;
                valueBar.style.marginBottom = 0;
                valueBar.style.marginLeft = 0;
                valueBar.style.backgroundColor = new Color(0.25f, 0.8f, 0.8f);
                valueBar.style.width = 25;
                valueBar.style.height = 0;
                valueBar.style.flexDirection = FlexDirection.Column;
                container.Add(valueBar);

                channelDataField.Add(container);
            }
        }

        /// <summary>
        /// チャンネルデータを更新する
        /// </summary>
        /// <param name="channelDataField"></param>
        private void UpdateChannelData(VisualElement channelDataField)
        {
            // ArtNetReceiverが存在しない場合は何もしない
            if(_receiver == null || _receiver.ChannelData == null)
            {
                return;
            }
            
            // チャンネルデータのコピーを作成
            Dictionary<int, int> channelDataCopy = new Dictionary<int, int>(_receiver.ChannelData);

            // チャンネルデータをUIに反映
            foreach (KeyValuePair<int, int> entry in channelDataCopy)
            {
                int channel = entry.Key;    // チャンネル番号
                int value = entry.Value;    // チャンネルの値
                
                if (channel - 1 >= 0 && channel - 1 < channelDataField.childCount)
                {
                    var channelContainer = channelDataField.ElementAt(channel - 1);
                    if (channelContainer != null)
                    {
                        var valueBar = channelContainer.ElementAt(1);
                        valueBar.style.height = value / 255f * ValueBarHeight;  // チャンネルの値をバーの高さに反映
                    }
                }
            }

            UpdateSelectedChannel(_selectedChannel);
        }

        /// <summary>
        /// チャンネルを選択ボタンを押したときの処理
        /// </summary>
        private void SelectChannel(int channel)
        {
            _selectedChannel = channel;
            UpdateSelectedChannel(_selectedChannel);
        }

        /// <summary>
        /// 選択中のチャンネルの情報をSideMenuに表示
        /// </summary>
        /// <param name="channel"></param>
        private void UpdateSelectedChannel(int channel)
        {
            // ArtNetReceiverが存在しない場合、またはチャンネルが範囲外の場合は何もしない
            if(_receiver == null || _receiver.ChannelData == null || channel < 1 || channel > 512)
            {
                return;
            }
            
            if (!_receiver.ChannelData.ContainsKey(channel))
            {
                return;
            }
            
            // 選択中のチャンネルの値を取得
            int value = _receiver.ChannelData[channel];

            // 選択中のチャンネルを表示
            var selectedChannelLabel = rootVisualElement.Q<Label>("SelectedChannel");
            selectedChannelLabel.text = channel.ToString();

            // 選択中のチャンネルの値を表示 (0.00-100.00%)
            var selectedChannelValue = rootVisualElement.Q<Label>("SelectedChannelValue");
            selectedChannelValue.text = (value / 255f * 100).ToString("F2") + "%";

            // 選択中のチャンネルの値を表示 (0-255)
            var selectedChannelValueRaw = rootVisualElement.Q<Label>("SelectedChannelRawValue");
            selectedChannelValueRaw.text = value.ToString();
        }

        /// <summary>
        /// ウィンドウが閉じられたときの処理
        /// </summary>
        private void OnDisable()
        {
            // プレイモードの変更時のイベントを解除
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        /// <summary>
        /// ウィンドウが更新されるときの処理
        /// </summary>
        private void Update()
        {
            if (_receiver != null)
            {
                UpdateChannelData(_channelDataField);
                UpdateReceiverStats();
            }
        }
    }
}
#endif