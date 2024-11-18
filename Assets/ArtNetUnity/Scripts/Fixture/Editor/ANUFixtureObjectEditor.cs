#if UNITY_EDITOR
using UnityEditor;

namespace ArtNetUnity.Fixture
{
    [CustomEditor(typeof(ANUFixtureObject))]
    public class ANUFixtureObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // fixtureAsset Field
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fixtureAsset"));

            ANUFixtureObject fixtureObject = (ANUFixtureObject)target;
            ANUFixtureAsset fixtureAsset = fixtureObject.fixtureAsset;

            if (fixtureAsset != null)
            {
                EditorGUILayout.LabelField("Fixture Asset", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Fixture Name", fixtureAsset.fixtureName);
                EditorGUILayout.LabelField("Description", fixtureAsset.description);
                EditorGUILayout.LabelField("Fixture Type", fixtureAsset.fixtureType.ToString());

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);

                foreach (var channel in fixtureAsset.channels)
                {
                    EditorGUILayout.LabelField($"Channel: {channel.channelName} ({channel.channelNumber})", EditorStyles.boldLabel);
                    foreach (var attribute in channel.attributes)
                    {
                        DrawAttribute(attribute);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Fixture Asset assigned.");
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAttribute(ANUFixtureAsset.Attribute attribute)
        {
            EditorGUILayout.LabelField($"Attribute: {attribute.attributeName} ({attribute.attributeType})");
            EditorGUILayout.LabelField($"Min Value: {attribute.minValue}");
            EditorGUILayout.LabelField($"Max Value: {attribute.maxValue}");
        }
    }
}
#endif
