using UnityEngine;
using UnityEditor;


using SPACS.Utilities;

namespace SPACS.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IndentAttribute indentAttribute = (IndentAttribute)attribute;
            int indentMultiplier = indentAttribute.multiplier;

            for (int i = 0; i < indentMultiplier; i++)
            {
                EditorGUI.indentLevel++;
            }

            EditorGUI.PropertyField(position, property, label, true);

            for (int i = 0; i < indentMultiplier; i++)
            {
                EditorGUI.indentLevel--;
            }
        }
    }
}
