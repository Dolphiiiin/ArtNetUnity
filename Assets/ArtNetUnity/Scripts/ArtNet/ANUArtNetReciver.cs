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
    public enum ArtNetEndpoint
    {
        Any,
        Localhost,
        SpecificIP
    }

    public class ANUArtNetReciver : MonoBehaviour
    {
        [SerializeField]
        public ArtNetEndpoint selectedEndpoint = ArtNetEndpoint.Any;

        [SerializeField]
        public string specificIPAddress = "127.0.0.1";

        private ArtNetSocket _artNetSocket;
        private const int ArtNetPort = 6454;
        private const int MaxDmxChannels = 512;

        [SerializeField]
        private bool previewInEditor = false;

        [SerializeField]
        private bool debugMode = false;

        public Dictionary<int, int> channelData = new Dictionary<int, int>();

        private int _totalPacketsReceived = 0;
        private int _validArtNetPackets = 0;
        private int _invalidPackets = 0;

        public string nodeAddress { get; set; }
        public int universe { get; set; }

        void Start()
        {
#if UNITY_EDITOR
            if (previewInEditor)
            {
                DisposeArtNetSocket();
                Debug.Log(_artNetSocket == null ? "ArtNet receiver disposed" : "ArtNet receiver not disposed");
            }
#endif
            InitializeArtNetReceiver();
            for (int i = 1; i <= MaxDmxChannels; i++)
            {
                channelData[i] = 0;
            }
        }

        void InitializeArtNetReceiver()
        {
            DisposeArtNetSocket();

            try
            {
                _artNetSocket = new ArtNetSocket(UId.Empty);
                _artNetSocket.NewPacket += OnArtNetDataReceived;

                switch (selectedEndpoint)
                {
                    case ArtNetEndpoint.Localhost:
                        _artNetSocket.Open(IPAddress.Loopback, IPAddress.Loopback);
                        break;
                    case ArtNetEndpoint.SpecificIP:
                        if (IPAddress.TryParse(specificIPAddress, out IPAddress ipAddress))
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

                Debug.Log($"ArtNet receiver initialized on {selectedEndpoint}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize ArtNet receiver: {ex.Message}");
            }
        }

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

                        if (!channelData.ContainsKey(channel) || channelData[channel] != value)
                        {
                            channelData[channel] = value;
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
                _invalidPackets++;
                if (debugMode)
                {
                    Debug.LogWarning($"Received non-DMX packet: {e.Packet.OpCode}");
                }
            }
        }

        void OnDestroy()
        {
            DisposeArtNetSocket();
        }

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
        void OnValidate()
        {
            if (!Application.isPlaying && previewInEditor)
            {
                InitializeArtNetReceiver();
            }
            else if (!Application.isPlaying && !previewInEditor)
            {
                DisposeArtNetSocket();
                Debug.Log(_artNetSocket == null ? "ArtNet receiver disposed" : "ArtNet receiver not disposed");
            }
        }
#endif
    }
}