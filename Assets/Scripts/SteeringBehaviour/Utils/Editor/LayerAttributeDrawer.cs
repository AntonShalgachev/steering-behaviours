using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityPrototype
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.LabelField(position, string.Format("Wrong {0} usage", typeof(LayerAttribute).Name));
                return;
            }

            property.intValue = Mathf.Max(property.intValue, 0);
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
