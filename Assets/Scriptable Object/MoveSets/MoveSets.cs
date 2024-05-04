using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
//CHANGE TO PREFAB
//SEEMS LIKE I JUST PUT THIS IN THE ATTACK SCRIPT
[CreateAssetMenu(fileName = "Combat", menuName = "MoveSet")]
public class MoveSets : MonoBehaviour
{
    [Header("Basic")]
    public string Name;
    public string Press;
    public int DamageChance;
    public int Damage;
    public int Block;
    public int BlockChance;
    public string Text;
    public GameObject AttackPrefab;
    /*
    [Header("Elements")]
    
    public Elements[] ElementType;
    public double[] ElementIntensity;
    */
    public List<ElementInfo> ElementInfo;

    //[Header("Type Interactions")]
    //public List<TypeInteractions> BoostElements = new();
    //public List<TypeInteractions> WeakElements = new();

    [Header("Movement")]
    public float SpeedX;
    public float SpeedY;
    public double Distance;
    public double Height;

    [Header("Tile Effects")]
    public TileEffects TileEffect1;

    public TileEffects TileEffect2;

    public TileEffects TileEffect3;

    [System.Serializable]
    public class TileEffects
    {
        public bool isInteractable;
        public ElementInfo.AllElements[] TileElement;
        public double[] TileStart;
        public double[] StatusStrength;
    }


    /*
    public class TypeInteractions
    {
        public Elements ElementInteractions;
        public double InteractionStrength;
    }
    */
    private void OnEnable()
    {
        /*
        //ElementIntensity is just used in this case. Will not use again in code. Just for ease of access
        if(ElementType.Length > 0 && ElementType != null)
        {
            for(int i = 0; i < ElementType.Length; i++)
            {
                ElementType[i].Intensity = ElementIntensity[i];

            }

        }
        */

        //PlaceHolderIntensity is just used in this case. Will not use again in code. Just for ease of access

    }
}

    /*
    public void CopyMoveSets(GameObject other)
    {
        Name = other.Name;
        Press = other.Press;
        DamageChance = other.DamageChance;
        Damage = other.Damage;
        Block = other.Block;
        BlockChance = other.BlockChance;
        Text = other.Text;
        AttackPrefab = other.AttackPrefab;
       
        for (int i = 0; i < other.ElementInfo.Count; i++)
        {
            ElementInfo[i] = new ElementInfo(other.ElementInfo[i]);
        }

        SpeedX = other.SpeedX;
        SpeedY = other.SpeedY;
        Distance = other.Distance;
        Height = other.Height;

        TileEffect1 = other.TileEffect1;
        TileEffect2 = other.TileEffect2;
        TileEffect3 = other.TileEffect3;
    }


}
    */
