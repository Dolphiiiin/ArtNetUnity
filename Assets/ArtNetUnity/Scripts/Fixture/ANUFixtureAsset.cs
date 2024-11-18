using System.Collections.Generic;
using UnityEngine;

namespace ArtNetUnity.Fixture
{
    [CreateAssetMenu(menuName = "ArtNetUnity/FixtureAsset")]
    public class ANUFixtureAsset : ScriptableObject
    {
        public string fixtureName;
        public string description;

        public enum Type
        {
            Dimmer,
            Par,
            Mover,
            Strobe,
            FX,
            Other,
        }
        public Type fixtureType;

        public Channel[] channels;

        public ANUFixtureAsset(string fixtureName, string description)
        {
            this.fixtureName = fixtureName;
            this.description = description;
        }

        [System.Serializable]
        public class Channel
        {
            public string channelName;
            public int channelNumber;
            public List<Attribute> attributes;
            public Channel()
            {
                attributes = new List<Attribute>();
            }
        }

        [System.Serializable]
        public class Attribute
        {
            public string attributeName;
            public AttributeType attributeType;
            public float minValue;
            public float maxValue;
        }

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