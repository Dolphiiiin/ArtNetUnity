using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArtNetUnity.ArtNet
{
    /// <summary>
    /// Art-Netのチャンネルデータに基づいてマテリアルプロパティを設定するクラス
    /// </summary>
    [ExecuteAlways]
    public class ANUArtNetMaterialSetter : MonoBehaviour
    {
        [Tooltip("Art-Netレシーバーへの参照")] [SerializeField]
        private ANUArtNetReceiver artNetReceiver;

        [Tooltip("MeshRendererコンポーネントへの参照")] [SerializeField]
        private MeshRenderer meshRenderer;

        [Tooltip("チャンネルとマテリアルプロパティのマッピングの配列")] [SerializeField]
        private ChannelMaterialProperty[] channelToMaterialProperties;

        [Tooltip("マテリアルプロパティの最小値")] [SerializeField]
        private float minValue = 0f;

        [Tooltip("マテリアルプロパティの最大値")] [SerializeField]
        private float maxValue = 1f;

        [Tooltip("Editorでプレビュー")] [SerializeField]
        private bool previewInEditor = false;

        // チャンネルデータのコピー
        private Dictionary<int, int> _channelDataCopy = new Dictionary<int, int>();

        // プレイ中かどうか
        private bool _isPlaying = false;

        /// <summary>
        /// UnityのStartメソッドマテリアルセッターを初期化する
        /// </summary>
        void Start()
        {
            _isPlaying = true;
            StartCoroutine(UpdateChannelDataCopy());
        }

        /// <summary>
        /// UnityのOnDestroyメソッドマテリアルセッターを停止する
        /// </summary>
        void OnDestroy()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// 定期的にチャンネルデータのコピーを更新するコルーチン
        /// </summary>
        /// <returns>コルーチンのIEnumerator</returns>
        IEnumerator UpdateChannelDataCopy()
        {
            while (_isPlaying)
            {
                if (artNetReceiver != null)
                {
                    _channelDataCopy = new Dictionary<int, int>(artNetReceiver.ChannelData);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// UnityのUpdateメソッドチャンネルデータに基づいてマテリアルプロパティを更新する
        /// </summary>
        void Update()
        {
            if (!_isPlaying && !previewInEditor) return;

            UpdateMaterialProperties();
        }

        /// <summary>
        /// チャンネルデータに基づいてマテリアルプロパティを更新する
        /// </summary>
        private void UpdateMaterialProperties()
        {
            if (artNetReceiver != null && meshRenderer != null)
            {
                foreach (var entry in _channelDataCopy)
                {
                    int channel = entry.Key;
                    int value = entry.Value;

                    foreach (var mapping in channelToMaterialProperties)
                    {
                        if (mapping.channel == channel)
                        {
                            // マテリアルプロパティの値を正規化
                            float normalizedValue = Mathf.Lerp(minValue, maxValue, value / 255f);
                            
                            // マテリアルプロパティを更新
                            meshRenderer.sharedMaterial.SetFloat(mapping.propertyName, normalizedValue);
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// UnityのOnValidateメソッドエディタでマテリアルプロパティを更新する
        /// </summary>
        private void OnValidate()
        {
            if (!Application.isPlaying && previewInEditor)
            {
                StartCoroutine(UpdateChannelDataCopy());
                UpdateMaterialProperties();
            }
        }
#endif
    }

    /// <summary>
    /// チャンネルとマテリアルプロパティのマッピングを表すクラス
    /// </summary>
    [Serializable]
    public class ChannelMaterialProperty
    {
        /// <summary>
        /// チャンネル番号
        /// </summary>
        public int channel;

        /// <summary>
        /// マテリアルプロパティの名前
        /// </summary>
        public string propertyName;
    }
}
