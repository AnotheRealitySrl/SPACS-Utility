
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;


using SPACS.Utilities;

namespace SPACS.Editor
{
    [CustomPropertyDrawer(typeof(SerializedType<>))]
    public class SerializedTypePropertyDrawer : PropertyDrawer
    {
        private IEnumerable<Type> allTypes;

        IEnumerable GetInheritedClasses(Type baseType)
        {
            return allTypes.Where(t => (t.IsClass || t.IsInterface) && baseType.IsAssignableFrom(t));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Debug.Log(property.FindPropertyRelative("fullTypeName").stringValue);
            Debug.Log(fieldInfo?.FieldType);

            allTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes()).OrderBy(x => x.Name);

            // get generic type
            Type t = fieldInfo.FieldType;
            while (t.IsGenericType)
            {
                t = t.GenericTypeArguments[0];
            }

            SerializedProperty prop = property.FindPropertyRelative("fullTypeName");
            string typeName = prop.stringValue;

            // dropdown button rect
            Rect dropdownRect = position;
            dropdownRect.x += EditorGUIUtility.labelWidth + 2;
            dropdownRect.width -= EditorGUIUtility.labelWidth + 2;
            dropdownRect.height = EditorGUIUtility.singleLineHeight;

            Type currentType = allTypes.FirstOrDefault(x => x.Name == typeName);

            EditorGUI.PrefixLabel(position, label);
            if (EditorGUI.DropdownButton(!string.IsNullOrEmpty(label.text) ? dropdownRect : position, new(currentType?.Name ?? "Select Type"), FocusType.Keyboard))
            {
                GenericMenu menu = new();

                // inherited types
                foreach (Type type in GetInheritedClasses(t))
                {
                    menu.AddItem(new GUIContent(type.Name), typeName == type.FullName, () =>
                    {
                        prop.stringValue = type.FullName;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }
        }
    }
}
