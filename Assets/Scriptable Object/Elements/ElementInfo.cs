using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName = "Element", menuName = "Elements/All")]
[System.Serializable]
public class ElementInfo
{
    public enum AllElements
    {
        normal = 0,
        fire,
        water,
        ice,
        ground,
        plant,
        electric,
        cloud,
        raincloud,
        thundercloud,
        size
    }

    public AllElements Element;

    public double Intensity;

    public static AllElements GetElementFromInt(int intValue)
    {
        return (AllElements)Enum.ToObject(typeof(AllElements), intValue);
    }

    
    public ElementInfo(ElementInfo other)
    {
        this.Element = other.Element;
        this.Intensity = other.Intensity;
        // Copy other properties as needed
    }

    public ElementInfo()
    {
        // Set default values for the properties if needed
        this.Element = AllElements.normal;
        this.Intensity = 0.0;
    }

    public ElementInfo(AllElements elementEnum)
    {
        // Set default values for the properties if needed
        this.Element = elementEnum;
        this.Intensity = 0.0;
    }

}

