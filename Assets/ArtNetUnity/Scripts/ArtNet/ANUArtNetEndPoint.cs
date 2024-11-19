using System;
using System.Net;
using LXProtocols.Acn.Sockets;
using LXProtocols.ArtNet.Packets;
using LXProtocols.ArtNet.Sockets;
using UnityEngine;

namespace ArtNetUnity.ArtNet
{
    public class ANUArtNetEndPoint : MonoBehaviour
    {
        [Tooltip("エンドポイントがSpecificIPに設定されている場合に使用するIPアドレス")] [SerializeField]
        public string specificIPAddress = "127.0.0.1";

        // ArtNetのソケット
        private ArtNetSocket _artNetSocket;
        
        // ArtNetのポート番号 (一般的には6456で固定)
        private const int ArtNetPort = 6454;

        // ArtNetパケット受信時のイベント
        public event EventHandler<NewPacketEventArgs<ArtNetPacket>> newPacketReceived;

        void Start()
        {
            InitializeArtNetSocket();
        }

        void OnDestroy()
        {
            DisposeArtNetSocket();
        }

        /// <summary>
        /// ArtNetソケットを初期化します
        /// </summary>
        private void InitializeArtNetSocket()
        {
            DisposeArtNetSocket();

            try
            {
                _artNetSocket = new ArtNetSocket(LXProtocols.Acn.Rdm.UId.Empty);    // ArtNetのソケットを生成
                _artNetSocket.NewPacket += OnArtNetDataReceived;

                // IPAddressのパースに成功した場合のみソケットを開く
                if (IPAddress.TryParse(specificIPAddress, out IPAddress ipAddress))
                {
                    _artNetSocket.Open(ipAddress, ipAddress);   // ソケットを開く
                }
                else
                {
                    Debug.LogError("Invalid IP address specified");
                    return;
                }

                Debug.Log($"ArtNet socket initialized on {specificIPAddress}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize ArtNet socket: {ex.Message}");
            }
        }

        /// <summary>
        /// ArtNetデータを受信したときに呼び出されるコールバック
        /// </summary>
        private void OnArtNetDataReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            newPacketReceived?.Invoke(this, e);
        }

        /// <summary>
        /// ArtNetソケットを破棄します
        /// </summary>
        private void DisposeArtNetSocket()
        {
            if (_artNetSocket != null)
            {
                _artNetSocket.Close();
                _artNetSocket = null;
                Debug.Log("ArtNet socket closed");
            }
        }
    }
}
