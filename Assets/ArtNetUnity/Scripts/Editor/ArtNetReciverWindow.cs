#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using ArtNetUnity.ArtNet;

namespace ArtNetUnity.Editor
{
    public class ANUArtNetReciverWindow : EditorWindow
    {
        private ANUArtNetReciver _receiver;
        private ScrollView _channelDataScrollView;
        private VisualElement _channelDataField;

        [MenuItem("Tools/ArtNetUnity/ANUArtNetReciver")]
        public static void ShowWindow()
        {
            ANUArtNetReciverWindow wnd = GetWindow<ANUArtNetReciverWindow>();
            wnd.titleContent = new GUIContent("ANUArtNetReciver");
        }

        public void CreateGUI()
        {
            // Load the UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ArtNetUnity/Scripts/Editor/ArtNetReciverWindow.uxml");
            visualTree.CloneTree(rootVisualElement);

            // Find the VisualElement
            _channelDataScrollView = rootVisualElement.Q<ScrollView>("channelDataScrollView");
            _channelDataField = _channelDataScrollView.Q<VisualElement>("channelDataField");

            // Add ObjectField for selecting the receiver
            ObjectField receiverField = new ObjectField("Receiver")
            {
                objectType = typeof(ANUArtNetReciver),
                value = _receiver
            };
            receiverField.RegisterValueChangedCallback(evt =>
            {
                _receiver = evt.newValue as ANUArtNetReciver;
                UpdateChannelData(_channelDataField);
                UpdateReceiverStats();
            });
            rootVisualElement.Insert(0, receiverField);

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _receiver = FindObjectOfType<ANUArtNetReciver>();
                UpdateChannelData(_channelDataField);
                UpdateReceiverStats();
            }
        }

        private void UpdateReceiverStats()
        {
            var nodeAddressLabel = rootVisualElement.Q<Label>("NodeAddressNum");
            var universeLabel = rootVisualElement.Q<Label>("UniverseStats");

            if (_receiver != null)
            {
                nodeAddressLabel.text = _receiver.nodeAddress;
                universeLabel.text = _receiver.universe.ToString();
            }
            else
            {
                nodeAddressLabel.text = "N/A";
                universeLabel.text = "N/A";
            }
        }

        private void UpdateChannelData(VisualElement channelDataField)
        {
            channelDataField.Clear();

            if (_receiver != null && _receiver.channelData != null)
            {
                // Create a copy of the dictionary to avoid modification during enumeration
                Dictionary<int, int> channelDataCopy = new Dictionary<int, int>(_receiver.channelData);

                foreach (KeyValuePair<int, int> entry in channelDataCopy)
                {
                    int channel = entry.Key;
                    int value = entry.Value;

                    VisualElement container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;
                    container.style.flexWrap = Wrap.Wrap;
                    container.style.alignItems = Align.FlexStart;
                    container.style.width = 25;
                    container.style.height = 42;
                    container.style.marginBottom = 5;
                    container.style.marginRight = 5;
                    container.style.backgroundColor = Color.gray;
                    container.style.borderTopWidth = 1;
                    container.style.borderTopColor = Color.black;
                    container.style.borderRightWidth = 1;
                    container.style.borderRightColor = Color.black;
                    container.style.borderBottomWidth = 1;
                    container.style.borderBottomColor = Color.black;
                    container.style.borderLeftWidth = 1;
                    container.style.borderLeftColor = Color.black;

                    Label channelLabel = new Label($"{value}");
                    channelLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                    channelLabel.style.position = Position.Absolute;
                    channelLabel.style.top = 0;
                    container.Add(channelLabel);

                    VisualElement valueBar = new VisualElement();
                    valueBar.style.width = 25;
                    valueBar.style.height = value / 255f * 25;
                    valueBar.style.backgroundColor = Color.green;
                    valueBar.style.marginTop = 15;
                    container.Add(valueBar);

                    // channel select button
                    Button selectButton = new Button(() => UpdateSelectedChannel(channel))
                    {
                        text = "Select"
                    };
                    selectButton.style.position = Position.Absolute;
                    selectButton.style.bottom = 0;
                    selectButton.style.left = 0;
                    container.Add(selectButton);

                    channelDataField.Add(container);
                }
            }
        }

        private void UpdateSelectedChannel(int channel)
        {
            Debug.Log($"Selected channel: {channel}");
            var selectedChannelLabel = rootVisualElement.Q<Label>("SelectedChannel");
            selectedChannelLabel.text = channel.ToString();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

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