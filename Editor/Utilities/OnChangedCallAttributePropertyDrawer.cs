
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;


using SPACS.Utilities;

namespace SPACS.Editor
{
    [CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
    public class OnChangedCallAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (EditorGUI.EndChangeCheck())
            {
                OnChangedCallAttribute at = attribute as OnChangedCallAttribute;
                MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods().Where(m => m.Name == at.methodName).First();

                // Ensure property has changed
                property.serializedObject.ApplyModifiedProperties();

                // Only instantiate methods with 0 parameters
                if (method != null && method.GetParameters().Count() == 0)
                    method.Invoke(property.serializedObject.targetObject, null);
            }
        }
    }
}
