using UnityEngine;

namespace ArtNetUnity.Fixture
{
    public class ANUFixtureObject : MonoBehaviour
    {
        [SerializeField] public ANUFixtureAsset fixtureAsset;

        [System.Serializable]
        public class AttributeScripts
        {
            public string attributeName;
        }

        void Start()
        {
            if (fixtureAsset != null)
            {
                foreach (var attribute in fixtureAsset.GetAttributes())
                {
                }
            }
        }
    }
}
