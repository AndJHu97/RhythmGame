using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public ElementInteractionManager elementInteractionManager;
    public List<ElementInfo> tileElements = new();
    public ElementInfo mainElement;


    private float timer = 0f;
    private float interval = 1f; // Set the interval to 1 second

    private void Start()
    {
        List<ElementInfo> clonedTileElements = new List<ElementInfo>();
        foreach (ElementInfo element in tileElements)
        {
            ElementInfo clonedElement = new ElementInfo(element);
            clonedTileElements.Add(clonedElement);
        }

        tileElements= clonedTileElements;

        mainElement = new ElementInfo(mainElement);
    }
    public void AttackCollision(GameObject AttackMove, int indexOfElement)
    { 
        //this is the whole movesets from the attack
        //will fix later
        AttackScript currentAttack = AttackMove.GetComponent<AttackScript>();

        //Do calcutions of type intensity on tiles which takes from intensity of attack type
        //Get the type of element and keep track of it. This one is the object that is referencing original attack
        ElementInfo currentAttackElementInfo = new();
        //adjust to statusstrength and use to interact
        List<ElementInfo> adjustedAttackElementInfo = new();



        double statusStrength = 0.0;
        //Need to know which tile to know what percentage of intensity it does
        if (gameObject.CompareTag("Tile1"))
        {
            //what the hell am I doing with statusstrength. Aight I'm about to do some shit
            statusStrength = currentAttack.TileEffect1.StatusStrength[indexOfElement]/100.0;
            //find the ElementInfo element, not ElementEffectOnTile since ElementInfo has info on intensity

            int i = 0;
            while (currentAttack.ElementInfos[i].Element != currentAttack.TileEffect1.ElementEffectOnTile[indexOfElement])
            {
                i++;
            }
            ElementInfo adjustedByStatusStrengthElement = new ElementInfo(currentAttack.ElementInfos[i].Element);
            //get the amount based on status strength that is going to interact with the tiles
            adjustedByStatusStrengthElement.Intensity = currentAttack.ElementInfos[i].Intensity * statusStrength;
            adjustedAttackElementInfo.Add(adjustedByStatusStrengthElement);
            currentAttack.ElementInfos[i].Intensity *= (1 - statusStrength);
            //keep track and pass by object of the currentAttack
            currentAttackElementInfo = currentAttack.ElementInfos[i];



        }
        else if (gameObject.CompareTag("Tile2"))
        { 
            statusStrength = currentAttack.TileEffect2.StatusStrength[indexOfElement]/100.0;
            //find the ElementInfo element, not ElementEffectOnTile since ElementInfo has info on intensity
            int i = 0;
            while (currentAttack.ElementInfos[i].Element != currentAttack.TileEffect2.ElementEffectOnTile[indexOfElement])
            {
                i++;
            }
            ElementInfo adjustedByStatusStrengthElement = new ElementInfo(currentAttack.ElementInfos[i].Element);
            //get the amount based on status strength that is going to interact with the tiles
            adjustedByStatusStrengthElement.Intensity = currentAttack.ElementInfos[i].Intensity * statusStrength;
            adjustedAttackElementInfo.Add(adjustedByStatusStrengthElement);
            currentAttack.ElementInfos[i].Intensity *= (1 - statusStrength);
            //keep track and pass by object of the currentAttack
            currentAttackElementInfo = currentAttack.ElementInfos[i];
        }
        else if (gameObject.CompareTag("Tile3"))
        { 
            statusStrength = currentAttack.TileEffect3.StatusStrength[indexOfElement] / 100.0;
            //find the ElementInfo element, not ElementEffectOnTile since ElementInfo has info on intensity
            int i = 0;
            while (currentAttack.ElementInfos[i].Element != currentAttack.TileEffect3.ElementEffectOnTile[indexOfElement])
            {
                i++;
            }
            ElementInfo adjustedByStatusStrengthElement = new ElementInfo(currentAttack.ElementInfos[i].Element);
            //get the amount based on status strength that is going to interact with the tiles
            adjustedByStatusStrengthElement.Intensity = currentAttack.ElementInfos[i].Intensity * statusStrength;
            adjustedAttackElementInfo.Add(adjustedByStatusStrengthElement);
            currentAttack.ElementInfos[i].Intensity *= (1 - statusStrength);
            //keep track and pass by object of the currentAttack
            currentAttackElementInfo = currentAttack.ElementInfos[i];
        }

        

        elementInteractionManager.InteractTileAttack(adjustedAttackElementInfo, tileElements);
        
        //add back the adjustedAttackElementInfo with original
        //Potential error in that what if adjustedAttackElement has a new combo element that is in the 0 index? How do I even add new elements to attack?
        currentAttackElementInfo.Intensity += adjustedAttackElementInfo[0].Intensity;

        //seems like I'm not doing anything with this?
        mainElement = elementInteractionManager.ReturnMainElement(tileElements);

   


    }

    public void WithinTileInteract()
    {
        elementInteractionManager.WithinTileInteract(tileElements);
    }
    // Update is called once per frame
    void Update()
    {
        
        // Increment the timer by the time passed since the last frame
        timer += Time.deltaTime;

        // Check if the timer has reached the interval (1 second in this case)
        if (timer >= interval)
        {
            // Call your function every second
            WithinTileInteract();

            // Reset the timer
            timer = 0f;
        }


        //WithinTileInteract();
    }
}
