using System;
using System.Collections.Generic;
using System.Net;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Sockets;
using LXProtocols.ArtNet;
using UnityEngine;
using LXProtocols.ArtNet.Packets;
using LXProtocols.ArtNet.Sockets;

namespace ArtNetUnity.ArtNet
{
    /// <summary>
    /// Art-Netのエンドポイントを表す列挙型
    /// </summary>
    public enum ArtNetEndpoint
    {
        Any,
        Localhost,
        SpecificIP
    }

    /// <summary>
    /// UnityプロジェクトでArt-Netデータを受信するクラス
    /// </summary>
    public class ANUArtNetReciver : MonoBehaviour
    {
        /// <summary>
        /// 選択されたArt-Netエンドポイント
        /// </summary>
        [SerializeField]
        public ArtNetEndpoint SelectedEndpoint = ArtNetEndpoint.Any;

        /// <summary>
        /// エンドポイントがSpecificIPに設定されている場合に使用する特定のIPアドレス
        /// </summary>
        [SerializeField]
        public string SpecificIPAddress = "127.0.0.1";

        private ArtNetSocket _artNetSocket;
        private const int ArtNetPort = 6454;
        private const int MaxDmxChannels = 512;

        /// <summary>
        /// UnityエディターでArt-Netデータをプレビューするかどうか
        /// </summary>
        [SerializeField]
        private bool _previewInEditor = false;

        /// <summary>
        /// 詳細な情報をログに記録するデバッグモードを有効にするかどうか
        /// </summary>
        [SerializeField]
        private bool _debugMode = false;

        /// <summary>
        /// DMXチャンネルデータを格納するディクショナリ
        /// </summary>
        public Dictionary<int, int> ChannelData = new Dictionary<int, int>();

        private int _totalPacketsReceived = 0;
        private int _validArtNetPackets = 0;
        private int _invalidPackets = 0;

        /// <summary>
        /// Art-Netレシーバーのノードアドレス
        /// </summary>
        public string NodeAddress { get; set; }

        /// <summary>
        /// Art-Netレシーバーのユニバース番号
        /// </summary>
        public int Universe { get; set; }

        /// <summary>
        /// UnityのStartメソッド、Art-Netレシーバーを初期化する
        /// </summary>
        void Start()
        {
#if UNITY_EDITOR
            if (_previewInEditor)
            {
                DisposeArtNetSocket();
                Debug.Log(_artNetSocket == null ? "ArtNet receiver disposed" : "ArtNet receiver not disposed");
            }
#endif
            InitializeArtNetReceiver();
            for (int i = 1; i <= MaxDmxChannels; i++)
            {
                ChannelData[i] = 0;
            }
        }

        /// <summary>
        /// 選択されたエンドポイントに基づいてArt-Netレシーバーを初期化する
        /// </summary>
        void InitializeArtNetReceiver()
        {
            DisposeArtNetSocket();

            try
            {
                _artNetSocket = new ArtNetSocket(UId.Empty);
                _artNetSocket.NewPacket += OnArtNetDataReceived;

                switch (SelectedEndpoint)
                {
                    case ArtNetEndpoint.Localhost:
                        _artNetSocket.Open(IPAddress.Loopback, IPAddress.Loopback);
                        break;
                    case ArtNetEndpoint.SpecificIP:
                        if (IPAddress.TryParse(SpecificIPAddress, out IPAddress ipAddress))
                        {
                            _artNetSocket.Open(ipAddress, ipAddress);
                        }
                        else
                        {
                            Debug.LogError("Invalid IP address specified");
                            return;
                        }
                        break;
                    case ArtNetEndpoint.Any:
                    default:
                        _artNetSocket.Open(IPAddress.Any, IPAddress.Any);
                        break;
                }

                Debug.Log($"ArtNet receiver initialized on {SelectedEndpoint}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize ArtNet receiver: {ex.Message}");
            }
        }

        /// <summary>
        /// Art-Netデータパケットを受信するためのイベントハンドラ
        /// </summary>
        /// <param name="sender">イベントの送信者</param>
        /// <param name="e">Art-Netパケットを含むイベント引数</param>
        private void OnArtNetDataReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            if (e.Packet.OpCode == ArtNetOpCodes.Dmx)
            {
                var dmxPacket = e.Packet as ArtNetDmxPacket;
                if (dmxPacket != null)
                {
                    _totalPacketsReceived++;
                    _validArtNetPackets++;

                    for (int i = 0; i < dmxPacket.DmxData.Length && i < MaxDmxChannels; i++)
                    {
                        int channel = i + 1;
                        int value = dmxPacket.DmxData[i];

                        if (!ChannelData.ContainsKey(channel) || ChannelData[channel] != value)
                        {
                            ChannelData[channel] = value;
                            if (_debugMode)
                            {
                                Debug.Log($"Channel {channel} updated to {value}");
                            }
                        }
                    }

                    if (_debugMode && _totalPacketsReceived % 100 == 0)
                    {
                        Debug.Log($"ArtNet Statistics: Total:{_totalPacketsReceived} Valid:{_validArtNetPackets} Invalid:{_invalidPackets}");
                    }
                }
            }
            else
            {
                _invalidPackets++;
                if (_debugMode)
                {
                    Debug.LogWarning($"Received non-DMX packet: {e.Packet.OpCode}");
                }
            }
        }

        /// <summary>
        /// UnityのOnDestroyメソッド、Art-Netソケットを破棄する
        /// </summary>
        void OnDestroy()
        {
            DisposeArtNetSocket();
        }

        /// <summary>
        /// Art-Netソケットがnullでない場合に破棄する
        /// </summary>
        private void DisposeArtNetSocket()
        {
            if (_artNetSocket != null)
            {
                _artNetSocket.Close();
                _artNetSocket = null;
                Debug.Log("ArtNet receiver closed");
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// UnityのOnValidateメソッド、previewInEditorフラグに基づいてArt-Netレシーバーを初期化または破棄する
        /// </summary>
        void OnValidate()
        {
            if (!Application.isPlaying && _previewInEditor)
            {
                InitializeArtNetReceiver();
            }
            else if (!Application.isPlaying && !_previewInEditor)
            {
                DisposeArtNetSocket();
                Debug.Log(_artNetSocket == null ? "ArtNet receiver disposed" : "ArtNet receiver not disposed");
            }
        }
#endif
    }
}