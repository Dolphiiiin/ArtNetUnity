#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ArtNetUnity.Fixture
{
    [CustomEditor(typeof(ANUFixtureAsset))]
    public class ANUFixtureAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // fixtureName
            var fixtureNameProperty = serializedObject.FindProperty("fixtureName");
            fixtureNameProperty.stringValue = EditorGUILayout.TextField("Fixture Name", fixtureNameProperty.stringValue);

            // description
            var descriptionProperty = serializedObject.FindProperty("description");
            descriptionProperty.stringValue = EditorGUILayout.TextField("Description", descriptionProperty.stringValue);

            // fixtureType
            var fixtureTypeProperty = serializedObject.FindProperty("fixtureType");
            EditorGUILayout.PropertyField(fixtureTypeProperty, new GUIContent("Fixture Type"));

            // channels
            var channelsProperty = serializedObject.FindProperty("channels");
            EditorGUILayout.PropertyField(channelsProperty, new GUIContent("Channels"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
