/*
 * Author: Baste @ Unity Forums
 * From https://forum.unity.com/threads/how-to-get-name-or-number-of-selected-layer.445296/#post-2880314
 * 
 */

//This goes in LayerAttributeDrawer.cs. This must be inside a folder named "Editor", otherwise your project won't compile for builds!
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerAttributeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // For some reason the tooltip got lost.
        var att = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault() as TooltipAttribute;
        if (att != null) label.tooltip = att.tooltip;
        // Check if property has the correct type.
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.LabelField(position, "The property has to be a layer for LayerAttribute to work!");
            return;
        }
        // Draw the field.
        property.intValue = EditorGUI.LayerField(position, label, property.intValue);
    }

}