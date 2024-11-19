using System.Collections.Generic;
using UnityEngine;

namespace ArtNetUnity.Fixture
{
    /// <summary>
    /// ArtNetUnityのフィクスチャアセットを表すScriptableObject
    /// </summary>
    [CreateAssetMenu(menuName = "ArtNetUnity/FixtureAsset")]
    public class ANUFixtureAsset : ScriptableObject
    {
        [Tooltip("フィクスチャの名前")] public string fixtureName;

        [Tooltip("フィクスチャの説明")] public string description;

        /// <summary>
        /// フィクスチャの種類を表す列挙型
        /// </summary>
        public enum Type
        {
            Dimmer,
            Par,
            Mover,
            Strobe,
            FX,
            Other,
        }

        [Tooltip("フィクスチャの種類")] public Type fixtureType;

        [Tooltip("フィクスチャに関連付けられたチャンネルの配列")] public Channel[] channels;

        /// <summary>
        /// 名前と説明でフィクスチャアセットを初期化するコンストラクタ
        /// </summary>
        /// <param name="fixtureName">フィクスチャの名前</param>
        /// <param name="description">フィクスチャの説明</param>
        public ANUFixtureAsset(string fixtureName, string description)
        {
            this.fixtureName = fixtureName;
            this.description = description;
        }

        /// <summary>
        /// フィクスチャのチャンネルを表すクラス
        /// </summary>
        [System.Serializable]
        public class Channel
        {
            [Tooltip("チャンネルの名前")] public string channelName;

            [Tooltip("チャンネル番号")] public int channelNumber;

            [Tooltip("チャンネルに関連付けられた属性のリスト")] public List<Attribute> attributes;

            /// <summary>
            /// 空の属性リストでチャンネルを初期化するコンストラクタ
            /// </summary>
            public Channel()
            {
                attributes = new List<Attribute>();
            }
        }

        /// <summary>
        /// チャンネルの属性を表すクラス
        /// </summary>
        [System.Serializable]
        public class Attribute
        {
            [Tooltip("属性の名前")] public string attributeName;

            [Tooltip("属性の種類")] public AttributeType attributeType;

            [Tooltip("属性の最小値")] public float minValue;

            [Tooltip("属性の最大値")] public float maxValue;
        }

        /// <summary>
        /// 属性の種類を表す列挙型
        /// </summary>
        public enum AttributeType
        {
            VLB_Dimmer,
            VLB_Strobe,
            VLB_Red,
            VLB_Green,
            VLB_Blue,
            VLB_Pan,
            VLB_Tilt,
            VLB_PanFine,
            VLB_TiltFine,
            VLB_Zoom,
            VLB_Gobo,
            VLB_GoboRotation,

            Generic_Dimmer,
            Generic_Strobe,
            Generic_Red,
            Generic_Green,
            Generic_Blue,
            Generic_Pan,
            Generic_Tilt,
            Generic_PanFine,
            Generic_TiltFine,
            Generic_Zoom,
            Generic_Gobo,
            Generic_GoboRotation,
            User
        }

        /// <summary>
        /// すべてのチャンネルからすべての属性のリストを取得する
        /// </summary>
        /// <returns>すべての属性のリスト</returns>
        public List<Attribute> GetAttributes()
        {
            List<Attribute> allAttributes = new List<Attribute>();
            foreach (var channel in channels)
            {
                allAttributes.AddRange(channel.attributes);
            }
            return allAttributes;
        }
    }
}