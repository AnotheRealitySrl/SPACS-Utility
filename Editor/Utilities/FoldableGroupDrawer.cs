using SPACS.Utilities;

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(FoldableGroupAttribute))]
public class FoldableGroupDrawer : PropertyDrawer
{
    // Questo dictionary tiene traccia dello stato di apertura di ciascuna istanza di FoldableGroup.
    private static Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FoldableGroupAttribute groupAttribute = (FoldableGroupAttribute)attribute;
        string groupKey = property.serializedObject.targetObject.GetInstanceID() + groupAttribute.GroupName;

        // Aggiunge lo stato di apertura 
        if (!foldoutStates.ContainsKey(groupKey))
            foldoutStates[groupKey] = groupAttribute.Expanded;

        bool isFirst = IsFirstInGroup(property, groupAttribute.GroupName);
        bool expanded = foldoutStates[groupKey];

        // Creiamo un'etichetta pulita con il vero nome della variabile
        GUIContent fieldLabel = new GUIContent(property.displayName);

        if (isFirst)
        {
            // Rettangolo per l'header del gruppo
            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Disegno dello sfondo stile header
            GUI.Box(headerRect, "", EditorStyles.label);

            // Il Foldout visualizza il nome del GRUPPO
            foldoutStates[groupKey] = EditorGUI.Foldout(headerRect, expanded, groupAttribute.GroupName, true, EditorStyles.foldoutHeader);

            if (expanded)
            {
                // Spostiamo il rettangolo sotto l'header per la prima variabile
                Rect firstFieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
                DrawProperty(firstFieldRect, property, fieldLabel);
            }
        }
        else if (expanded)
        {
            // Per le variabili successive, usiamo la posizione standard
            DrawProperty(position, property, fieldLabel);
        }
    }

    private void DrawProperty(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(rect, property, label, true);
        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        FoldableGroupAttribute groupAttribute = (FoldableGroupAttribute)attribute;
        string groupKey = property.serializedObject.targetObject.GetInstanceID() + groupAttribute.GroupName;
        bool expanded = foldoutStates.ContainsKey(groupKey) ? foldoutStates[groupKey] : groupAttribute.Expanded;

        bool isFirst = IsFirstInGroup(property, groupAttribute.GroupName);

        if (isFirst)
        {
            float baseHeight = EditorGUIUtility.singleLineHeight; // L'altezza dell'header
            if (expanded)
            {
                // Header + Spazio + Altezza variabile
                return baseHeight + 2f + EditorGUI.GetPropertyHeight(property, label, true);
            }
            return baseHeight;
        }

        // Se non è il primo ed è chiuso, l'altezza è 0 (nascondi)
        return expanded ? EditorGUI.GetPropertyHeight(property, label, true) : -EditorGUIUtility.standardVerticalSpacing;
    }

    /// <summary>
    /// Restituisce true se questa è la prima variabile del FoldableGroup di nome groupName.
    /// </summary>
    private bool IsFirstInGroup(SerializedProperty property, string groupName)
    {
        var fields = property.serializedObject.targetObject.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<FoldableGroupAttribute>();
            if (attr != null && attr.GroupName == groupName)
            {
                return field.Name == property.name;
            }
        }
        return false;
    }
}
