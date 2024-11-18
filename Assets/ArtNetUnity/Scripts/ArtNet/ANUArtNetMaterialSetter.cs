using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtNetUnity.ArtNet
{
    /// <summary>
    /// Art-Netのチャンネルデータに基づいてマテリアルプロパティを設定するクラス
    /// </summary>
    [ExecuteAlways]
    public class ANUArtNetMaterialSetter : MonoBehaviour
    {
        /// <summary>
        /// Art-Netレシーバーへの参照
        /// </summary>
        [SerializeField]
        private ANUArtNetReciver _artNetReciver;

        /// <summary>
        /// MeshRendererコンポーネントへの参照
        /// </summary>
        [SerializeField]
        private MeshRenderer _meshRenderer;

        /// <summary>
        /// チャンネルとマテリアルプロパティのマッピングの配列
        /// </summary>
        [SerializeField]
        private ChannelMaterialProperty[] _channelToMaterialProperties;

        /// <summary>
        /// マテリアルプロパティの最小値
        /// </summary>
        [SerializeField]
        private float _minValue = 0f;

        /// <summary>
        /// マテリアルプロパティの最大値
        /// </summary>
        [SerializeField]
        private float _maxValue = 1f;

        /// <summary>
        /// Unityエディタで変更をプレビューするかどうか
        /// </summary>
        [SerializeField]
        private bool _previewInEditor = false;

        /// <summary>
        /// チャンネルデータのコピーを格納する辞書
        /// </summary>
        private Dictionary<int, int> _channelDataCopy = new Dictionary<int, int>();

        /// <summary>
        /// アプリケーションが実行中かどうかを示すフラグ
        /// </summary>
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
                if (_artNetReciver != null)
                {
                    _channelDataCopy = new Dictionary<int, int>(_artNetReciver.channelData);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// UnityのUpdateメソッドチャンネルデータに基づいてマテリアルプロパティを更新する
        /// </summary>
        void Update()
        {
            if (!_isPlaying && !_previewInEditor) return;

            UpdateMaterialProperties();
        }

        /// <summary>
        /// チャンネルデータに基づいてマテリアルプロパティを更新する
        /// </summary>
        private void UpdateMaterialProperties()
        {
            if (_artNetReciver != null && _meshRenderer != null)
            {
                foreach (var entry in _channelDataCopy)
                {
                    int channel = entry.Key;
                    int value = entry.Value;

                    foreach (var mapping in _channelToMaterialProperties)
                    {
                        if (mapping.channel == channel)
                        {
                            float normalizedValue = Mathf.Lerp(_minValue, _maxValue, value / 255f);
                            _meshRenderer.sharedMaterial.SetFloat(mapping.propertyName, normalizedValue);
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
            if (!Application.isPlaying && _previewInEditor)
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
