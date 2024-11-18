#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ArtNetUnity.Fixture
{
    [CustomPropertyDrawer(typeof(ANUFixtureAsset.AttributeType))]
    public class ANUAttributeTypeDrawer : PropertyDrawer
    {
        private AdvancedDropdownState dropdownState = new AdvancedDropdownState();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attributeType = (ANUFixtureAsset.AttributeType)property.enumValueIndex;
            var attributeTypeString = attributeType.ToString().Replace('_', '/');
            var attributeTypeValue = (int)attributeType;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            if (GUI.Button(position, attributeTypeString, EditorStyles.popup))
            {
                var dropdown = new ANUAttributeTypeDropdown(dropdownState, (selectedIndex) =>
                {
                    property.enumValueIndex = selectedIndex;
                    property.serializedObject.ApplyModifiedProperties();
                });
                dropdown.Show(position);
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = attributeTypeValue;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif