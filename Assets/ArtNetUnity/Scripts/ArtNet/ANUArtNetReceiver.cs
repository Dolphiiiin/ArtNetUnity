using System;
using System.Collections.Generic;
using LXProtocols.Acn.Sockets;
using LXProtocols.ArtNet;
using LXProtocols.ArtNet.Packets;
using UnityEngine;

namespace ArtNetUnity.ArtNet
{
    [ExecuteAlways]
    public class ANUArtNetReceiver : MonoBehaviour
    {
        [Tooltip("選択されたユニバース番号")] [SerializeField][Min(0)]
        public int selectedUniverse = 0;

        [Tooltip("Art-Netエンドポイント")] [SerializeField]
        public ANUArtNetEndPoint artNetEndPoint;
        
        // Art-NetエンドポイントのIPアドレス
        public string SpecificIPAddress => artNetEndPoint.specificIPAddress;

        // 1つのDMXユニバースに含まれる最大チャンネル数 (DMXチャンネルは1から始まる)
        private const int MaxDmxChannels = 512;

        [Tooltip("UnityエディターでArt-Netデータをプレビューするかどうか")] [SerializeField]
        private bool previewInEditor = false;

        [Tooltip("詳細な情報をログに記録するデバッグモードを有効にするかどうか")] [SerializeField]
        private bool debugMode = false;

        // チャンネルデータのキャッシュ
        // Usage: ChannelData[チャンネル番号] = チャンネルの値
        public Dictionary<int, int> ChannelData = new Dictionary<int, int>();

        // パケットの受信統計
        private int _totalPacketsReceived = 0;
        private int _validArtNetPackets = 0;
        private int _invalidPackets = 0;

        void Start()
        {
            ResetChannelData();
            artNetEndPoint.newPacketReceived += OnArtNetDataReceived;   // artNetEndPointからの新しいパケット受信時のイベントをListen
        }

        /// <summary>
        /// Art-Netデータを受信したときに呼び出されるコールバック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnArtNetDataReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            if (e.Packet.OpCode == ArtNetOpCodes.Dmx)
            {
                // 受信したデータがDMXパケットでかつ、選択されたユニバースのデータである場合
                if (e.Packet is ArtNetDmxPacket dmxPacket && dmxPacket.Universe == selectedUniverse)
                {
                    _totalPacketsReceived++;
                    _validArtNetPackets++;

                    // DMXパケットのデータをチャンネルデータに反映
                    for (int i = 0; i < dmxPacket.DmxData.Length && i < MaxDmxChannels; i++)
                    {
                        int channel = i + 1;
                        int value = dmxPacket.DmxData[i];

                        if (!ChannelData.ContainsKey(channel) || ChannelData[channel] != value)
                        {
                            ChannelData[channel] = value;
                            if (debugMode)
                            {
                                Debug.Log($"Channel {channel} updated to {value}");
                            }
                        }
                    }

                    if (debugMode && _totalPacketsReceived % 100 == 0)
                    {
                        Debug.Log($"ArtNet Statistics: Total:{_totalPacketsReceived} Valid:{_validArtNetPackets} Invalid:{_invalidPackets}");
                    }
                }
            }
            else
            {
                // DMXパケット以外のパケットを受信した場合
                _invalidPackets++;
                if (debugMode)
                {
                    Debug.LogWarning($"Received non-DMX packet: {e.Packet.OpCode}");
                }
            }
        }

        /// <summary>
        /// 今までに受信したチャンネルデータをリセット
        /// </summary>
        private void ResetChannelData()
        {
            ChannelData.Clear();
            for (int i = 1; i <= MaxDmxChannels; i++)
            {
                ChannelData[i] = 0;
            }
        }
    }
}
