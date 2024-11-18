using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ArtNetUnity.ArtNet
{
    [ExecuteAlways]
    public class ANUArtNetMaterialSetter : MonoBehaviour
    {
        [SerializeField]
        private ANUArtNetReciver artNetReciver;

        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private ChannelMaterialProperty[] channelToMaterialProperties;

        [SerializeField]
        private float minValue = 0f;

        [SerializeField]
        private float maxValue = 1f;

        [SerializeField]
        private bool previewInEditor = false;

        private Dictionary<int, int> _channelDataCopy = new Dictionary<int, int>();
        private bool _isPlaying = false;

        void Start()
        {
            _isPlaying = true;
            StartCoroutine(UpdateChannelDataCopy());
        }

        void OnDestroy()
        {
            _isPlaying = false;
        }

        IEnumerator UpdateChannelDataCopy()
        {
            while (_isPlaying)
            {
                if (artNetReciver != null)
                {
                    _channelDataCopy = new Dictionary<int, int>(artNetReciver.channelData);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        void Update()
        {
            if (!_isPlaying && !previewInEditor) return;

            UpdateMaterialProperties();
        }

        private void UpdateMaterialProperties()
        {
            if (artNetReciver != null && meshRenderer != null)
            {
                foreach (var entry in _channelDataCopy)
                {
                    int channel = entry.Key;
                    int value = entry.Value;

                    foreach (var mapping in channelToMaterialProperties)
                    {
                        if (mapping.channel == channel)
                        {
                            float normalizedValue = Mathf.Lerp(minValue, maxValue, value / 255f);
                            meshRenderer.sharedMaterial.SetFloat(mapping.propertyName, normalizedValue);
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
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

    [Serializable]
    public class ChannelMaterialProperty
    {
        public int channel;
        public string propertyName;
    }
}