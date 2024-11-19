#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;

namespace ArtNetUnity.Fixture
{
    public class ANUAttributeTypeDropdown : AdvancedDropdown
    {
        private Action<int> _onItemSelected;
        private List<AdvancedDropdownItem> _items;

        public ANUAttributeTypeDropdown(AdvancedDropdownState state, Action<int> onItemSelected) : base(state)
        {
            this._onItemSelected = onItemSelected;
            _items = new List<AdvancedDropdownItem>();
            minimumSize = new UnityEngine.Vector2(minimumSize.x, 15 * EditorGUIUtility.singleLineHeight);
            BuildRoot();
        }

        protected sealed override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Attribute Types");

            foreach (var value in Enum.GetValues(typeof(ANUFixtureAsset.AttributeType)))
            {
                var enumName = value.ToString().Replace('_', '/');
                var enumNameSplit = enumName.Split('/');
                var parent = root;
                for (int i = 0; i < enumNameSplit.Length; i++)
                {
                    var itemName = enumNameSplit[i];
                    if (i == enumNameSplit.Length - 1)
                    {
                        itemName += $" ({value})";
                    }
                    var item = new AdvancedDropdownItem(itemName);
                    if (i == enumNameSplit.Length - 1)
                    {
                        item.id = (int)value;
                    }
                    else
                    {
                        item.id = -1;
                    }
                    var found = false;
                    foreach (var child in parent.children)
                    {
                        if (child.name == item.name)
                        {
                            parent = child;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        parent.AddChild(item);
                        parent = item;

                        if (i == enumNameSplit.Length - 1)
                        {
                            _items.Add(item);
                        }
                    }
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            _onItemSelected?.Invoke(item.id);
        }
    }
}
#endif
