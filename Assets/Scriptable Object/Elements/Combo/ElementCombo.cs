using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static MoveSets;

[CreateAssetMenu(fileName = "ElementCombo", menuName = "Elements/Combo")]
public class ElementCombo : ScriptableObject 
{
    public ElementInfo.AllElements Result;
    public List<Ingredients> Ingredient = new();
    public Dictionary<ElementInfo.AllElements, double> ElementIntensityMap = new Dictionary<ElementInfo.AllElements, double>();

    [System.Serializable]
    public class Ingredients
    {
        public ElementInfo.AllElements Element;
        //the amount needed to create 1 intensity of combo
        public double IntensityNeeded;
        //what percentage of element is used for conversion
        public double IntensityUsability;
    }
    //how much intensity to become that tile
    public double Threshold;
    //how much of the ingredients will be converted
    public double ConversionRate;

    /*
    private void OnEnable()
    {
        if (Ingredient.Count > 0 && Ingredient != null)
        {
            for (int i = 0; i < Ingredient.Count; i++)
            {
                //initialize the intensity inside the element
                Ingredient[i].ElementInfo.Intensity = Ingredient[i].IntensityNeeded;

            }

        }
    }
    */
}



/*
[CustomEditor(typeof(ElementCombo))]
public class ElementComboEditor : Editor
{
    private SerializedProperty elementsProperty;

    private void OnEnable()
    {
        elementsProperty = serializedObject.FindProperty("Ingredients");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Result"));

        // Ensure the list is not null
        if (elementsProperty != null)
        {
            // If we are using serializedObject to display the list, use this instead:
            // EditorGUILayout.PropertyField(elementsProperty, true);

            // To manually display the list with a reorderable list, use this instead:
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(elementsProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Threshold"));

        serializedObject.ApplyModifiedProperties();
    }
}
*/