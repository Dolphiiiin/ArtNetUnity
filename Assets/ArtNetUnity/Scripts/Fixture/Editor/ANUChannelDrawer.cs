#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ArtNetUnity.Fixture
{
    [CustomPropertyDrawer(typeof(ANUFixtureAsset.Channel))]
    public class ANUChannelDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attributes = property.FindPropertyRelative("attributes");
            return (3 + attributes.arraySize * 4) * EditorGUIUtility.singleLineHeight + (attributes.arraySize + 1) * 2 + 10; // Added extra padding
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Add extra padding to the start position to avoid overlap
            position.y += 10;

            var channelName = property.FindPropertyRelative("channelName");
            var channelNumber = property.FindPropertyRelative("channelNumber");
            label = new GUIContent($"{channelName.stringValue} ({channelNumber.intValue})");

            var lineHeight = EditorGUIUtility.singleLineHeight + 2;
            var fullWidth = position.width;
            var halfWidth = fullWidth / 2;

            EditorGUI.LabelField(new Rect(position.x, position.y, fullWidth, EditorGUIUtility.singleLineHeight), label, EditorStyles.boldLabel);
            EditorGUI.DrawRect(new Rect(position.x + halfWidth, position.y + 0.5f * lineHeight, halfWidth, 1), Color.gray);

            var channelNameRect = new Rect(position.x, position.y + lineHeight, fullWidth, EditorGUIUtility.singleLineHeight);
            var channelNumberRect = new Rect(position.x, position.y + 2 * lineHeight, fullWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(channelNameRect, channelName, new GUIContent("Channel Name"));
            EditorGUI.PropertyField(channelNumberRect, channelNumber, new GUIContent("Channel Number"));

            var attributes = property.FindPropertyRelative("attributes");
            if (attributes != null)
            {
                EditorGUI.LabelField(new Rect(position.x, position.y + 3 * lineHeight, fullWidth, EditorGUIUtility.singleLineHeight), "Attributes", EditorStyles.boldLabel);
                EditorGUI.DrawRect(new Rect(position.x + halfWidth, position.y + 3.5f * lineHeight, halfWidth, 1), Color.gray);

                var attributeRect = new Rect(position.x, position.y + 4 * lineHeight, fullWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.indentLevel++;
                for (var i = 0; i < attributes.arraySize; i++)
                {
                    var propertyXOffset = 20;
                    var propertyFullWidth = fullWidth - propertyXOffset;
                    var propertyHalfWidth = propertyFullWidth / 2;

                    var attribute = attributes.GetArrayElementAtIndex(i);

                    var removeButtonRect = new Rect(position.x, attributeRect.y, fullWidth - propertyFullWidth, EditorGUIUtility.singleLineHeight * 3);
                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        attributes.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    var moveUpButtonRect = new Rect(position.x + fullWidth - 40, attributeRect.y, 20, EditorGUIUtility.singleLineHeight);
                    var moveDownButtonRect = new Rect(position.x + fullWidth - 20, attributeRect.y, 20, EditorGUIUtility.singleLineHeight);

                    if (GUI.Button(moveUpButtonRect, "↑") && i > 0)
                    {
                        attributes.MoveArrayElement(i, i - 1);
                    }

                    if (GUI.Button(moveDownButtonRect, "↓") && i < attributes.arraySize - 1)
                    {
                        attributes.MoveArrayElement(i, i + 1);
                    }

                    var attributeNameRect = new Rect(position.x + propertyXOffset, attributeRect.y, propertyFullWidth - 40, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(attributeNameRect, attribute.FindPropertyRelative("attributeName"), new GUIContent("Attribute Name"));

                    attributeRect.y += EditorGUIUtility.singleLineHeight + 1;

                    var attributeTypeRect = new Rect(position.x + propertyXOffset, attributeRect.y, propertyFullWidth, EditorGUIUtility.singleLineHeight);
                    attributeRect.y += EditorGUIUtility.singleLineHeight + 1;

                    var minValueRect = new Rect(position.x + propertyXOffset, attributeRect.y, propertyHalfWidth, EditorGUIUtility.singleLineHeight);
                    var maxValueRect = new Rect(position.x + propertyXOffset + halfWidth, attributeRect.y, propertyHalfWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(attributeTypeRect, attribute.FindPropertyRelative("attributeType"), new GUIContent("Type"));
                    EditorGUI.PropertyField(minValueRect, attribute.FindPropertyRelative("minValue"), new GUIContent("Min Value"));
                    EditorGUI.PropertyField(maxValueRect, attribute.FindPropertyRelative("maxValue"), new GUIContent("Max Value"));

                    attributeRect.y += EditorGUIUtility.singleLineHeight + 2;

                    EditorGUI.DrawRect(new Rect(position.x, attributeRect.y, fullWidth, 1), Color.gray);
                    attributeRect.y += 2;
                }

                EditorGUI.indentLevel--;
                var addButtonRect = new Rect(position.x, attributeRect.y, fullWidth, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(addButtonRect, "+"))
                {
                    attributes.InsertArrayElementAtIndex(attributes.arraySize);
                }

            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif
