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
        /// <summary>
        /// フィクスチャの名前
        /// </summary>
        public string fixtureName;

        /// <summary>
        /// フィクスチャの説明
        /// </summary>
        public string description;

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

        /// <summary>
        /// フィクスチャの種類
        /// </summary>
        public Type fixtureType;

        /// <summary>
        /// フィクスチャに関連付けられたチャンネルの配列
        /// </summary>
        public Channel[] channels;

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
            /// <summary>
            /// チャンネルの名前
            /// </summary>
            public string channelName;

            /// <summary>
            /// チャンネル番号
            /// </summary>
            public int channelNumber;

            /// <summary>
            /// チャンネルに関連付けられた属性のリスト
            /// </summary>
            public List<Attribute> attributes;

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
            /// <summary>
            /// 属性の名前
            /// </summary>
            public string attributeName;

            /// <summary>
            /// 属性の種類
            /// </summary>
            public AttributeType attributeType;

            /// <summary>
            /// 属性の最小値
            /// </summary>
            public float minValue;

            /// <summary>
            /// 属性の最大値
            /// </summary>
            public float maxValue;
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