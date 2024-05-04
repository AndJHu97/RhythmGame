using System;
using System.Collections.Generic;
using UnityEngine;
using static ElementCombo;
using static ElementInteractionManager;

//when one element boost or weak
public struct SingleData
{
    public double mUse;
    public double mStr;
    public int mTag;
    public double aConv;
    public int aTag;
    public bool markMod;
    public bool markAdj;
    public double mIntensityChange;
    public double aIntensityChange;
}

//When two elements combine to form another element
public struct ComboData
{
    public double mUse1;
    public double mNeed1;
    public int mTag1;
    public double mUse2;
    public double mNeed2;
    public int mTag2;
    public double aConv;
    public int aTag;
    public bool markMod1;
    public bool markMod2;
    public bool markAdj;
    public double mIntensityChange1;
    public double mIntensityChange2;
    public double aIntensityChange;
}
public class AttackTileInteractionTask
{
    

    //InteractionData
    private List<SingleData> singleInteract = new();
    private List<ComboData> comboInteract = new();
    private double[] attackTotalUseability = new double[(int)ElementInfo.AllElements.size];
    private double[] tileTotalUseability = new double[(int)ElementInfo.AllElements.size];
    private List<ElementInfo> attackElementsList = new();
    private List<ElementInfo> tileElementsList = new();
    private int singlePos = -1;
    private int comboPos = -1;
    private List<TypeInteractionInfo> AllInteractions = new();

    public AttackTileInteractionTask(List<ElementInfo> attackElementsList, List<ElementInfo> tileElementsList, List<TypeInteractionInfo> AllInteractions)
    {
        this.attackElementsList = attackElementsList;
        this.tileElementsList = tileElementsList;
        this.AllInteractions = AllInteractions;
    }

    public void SingleUpdateModifier(double mUse, double mStr, int mTag)
    {
        if (singlePos == -1)
        {
            singleInteract.Add(new SingleData());
            singlePos++;
        }

        SingleData tempData = singleInteract[singlePos];
        tempData.mUse = mUse;
        tempData.mStr = mStr;
        tempData.mTag = mTag;
        singleInteract[singlePos] = tempData;
    }

    public void SingleUpdateModMark(bool markMod)
    {
        if (singlePos == -1)
        {
            singleInteract.Add(new SingleData());
            singlePos++;
        }

        SingleData tempData = singleInteract[singlePos];
        tempData.markMod = markMod;
        singleInteract[singlePos] = tempData;
    }

    public void SingleUpdateAdjusted(double aConv, int aTag)
    {
        if (singlePos == -1)
        {
            singleInteract.Add(new SingleData());
            singlePos++;
        }
        SingleData tempData = singleInteract[singlePos];
        tempData.aConv = aConv;
        tempData.aTag = aTag;
        singleInteract[singlePos] = tempData;
    }

    public void SingleUpdateAdjMark(bool markAdj)
    {
        if (singlePos == -1)
        {
            singleInteract.Add(new SingleData());
            singlePos++;
        }
        SingleData tempData = singleInteract[singlePos];
        tempData.markAdj = markAdj;
        singleInteract[singlePos] = tempData;
    }

    public void SingleAdd()
    {
        singlePos++;
        singleInteract.Add(new SingleData());
    }

    public void ComboUpdateMod1(double mUse1, double mNeed1, int mTag1)
    {
        if (comboPos == -1)
        {
            comboInteract.Add(new ComboData());
            comboPos++;
        }
        ComboData tempData = comboInteract[comboPos];
        tempData.mUse1 = mUse1;
        tempData.mNeed1 = mNeed1;
        tempData.mTag1 = mTag1;
        comboInteract[comboPos] = tempData;
    }

    public void ComboUpdateMarkMod1(bool markMod1)
    {
        if (comboPos == -1)
        {
            comboInteract.Add(new ComboData());
            comboPos++;
        }
        ComboData tempData = comboInteract[comboPos];
        tempData.markMod1 = markMod1;
        comboInteract[comboPos] = tempData;
    }

    public void ComboUpdateMod2(double mUse2, double mNeed2, int mTag2)
    {
        if (comboPos == -1)
        {
            comboInteract.Add(new ComboData());
            comboPos++;
        }
        ComboData tempData = comboInteract[comboPos];
        tempData.mUse2 = mUse2;
        tempData.mNeed2 = mNeed2;
        tempData.mTag2 = mTag2;
        comboInteract[comboPos] = tempData;
    }

    public void ComboUpdateMarkMod2(bool markMod2)
    {
        if (comboPos == -1)
        {
            comboInteract.Add(new ComboData());
            comboPos++;
        }
        ComboData tempData = comboInteract[comboPos];
        tempData.markMod2 = markMod2;
        comboInteract[comboPos] = tempData;
    }

    public void ComboUpdateAdj(double aConv, int aTag)
    {
        if (comboPos == -1)
        {
            comboInteract.Add(new ComboData());
            comboPos++;
        }
        ComboData tempData = comboInteract[comboPos];
        tempData.aConv = aConv;
        tempData.aTag = aTag;
        comboInteract[comboPos] = tempData;
    }

    public void ComboUpdateMarkAdj(bool markAdj)
    {
        if (comboPos == -1)
        {
            comboInteract.Add(new ComboData());
            comboPos++;
        }
        ComboData tempData = comboInteract[comboPos];
        tempData.markAdj = markAdj;
        comboInteract[comboPos] = tempData;
    }

    public void ComboAdd()
    {
        comboPos++;
        comboInteract.Add(new ComboData());
    }

    //add the intensity changes from last function
    public void AddFinalIntensities()
    {
        
        //Add from singleData
         for(int sItr = 0; sItr < singleInteract.Count; sItr++) 
        {
            SingleData singleData = singleInteract[sItr];
            //Know if tile or attack
            //mod is attack then affect attackElements
            if (singleData.markMod)
            {
                AddSingleIntensities(attackElementsList, tileElementsList, singleData);

            }
            else
            {
                AddSingleIntensities(tileElementsList, attackElementsList, singleData);
            }
        }

        for (int cItr = 0; cItr < comboInteract.Count; cItr++)
        {
            ComboData comboData = comboInteract[cItr];
            //if first mod is attack
            if (comboData.markMod1)
            {
                AddComboIntensities(attackElementsList, tileElementsList, comboData);
            }
            else
            { 
                AddComboIntensities(tileElementsList,attackElementsList, comboData);
            }
        }

        //Add SPREAD EFFECT
    }

    private void AddComboIntensities(List<ElementInfo> elementList1, List<ElementInfo> elementList2, ComboData comboData)
    {
        //itr to find where it is
        int listLoc1 = -1;
        int listLoc2 = -1;
        int listLocA = -1;

        //Set the right place to place the adjusted combo
        List<ElementInfo> comboAdj = new List<ElementInfo>();
        if (comboData.markAdj && comboData.markMod1)
        {
            comboAdj = elementList1;
        }
        else if (comboData.markAdj && comboData.markMod2)
        {
            comboAdj = elementList2;
        }
        //when comboData is tile and mod1 is tile
        else if (!comboData.markAdj && !comboData.markMod1)
        {
            comboAdj = elementList1;
        }
        //comboData is tile and mod2 is tile
        else if (!comboData.markAdj && !comboData.markMod2)
        {
            comboAdj = elementList2;
        }

        //check iteration
        //mod 1 itr
        for (int i = 0; i < elementList1.Count; i++)
        {
            if (elementList1[i].Element == ElementInfo.GetElementFromInt(comboData.mTag1))
            {
                listLoc1 = i;
                break;
            }
        }

        //mod 2 itr
        for (int i = 0; i < elementList2.Count; i++)
        {
            if (elementList2[i].Element == ElementInfo.GetElementFromInt(comboData.mTag2))
            {
                listLoc2 = i;
                break;
            }
        }

        //result itr
        for (int i = 0; i < comboAdj.Count; i++)
        {
            if (comboAdj[i].Element == ElementInfo.GetElementFromInt(comboData.aTag))
            {
                listLocA = i;
                break;
            }
        }

        //mod1
        if (listLoc1 == -1)
        {
            ElementInfo newElement = new ElementInfo(ElementInfo.GetElementFromInt(comboData.mTag1));
            newElement.Intensity = comboData.mIntensityChange1;
            elementList1.Add(newElement);
        }
        else 
        {
            elementList1[listLoc1].Intensity += comboData.mIntensityChange1;
        }

        //mod2
        if (listLoc2 == -1)
        {
            ElementInfo newElement = new ElementInfo(ElementInfo.GetElementFromInt(comboData.mTag2));
            newElement.Intensity = comboData.mIntensityChange2;
            elementList2.Add(newElement);
        }
        else 
        {
            //changed recently from elementList1 to 2 to help correct error. But unsure if I'm reading code right...
            elementList2[listLoc2].Intensity += comboData.mIntensityChange2;
        }

        //adj
        if (listLocA == -1)
        {
            ElementInfo newElement = new ElementInfo(ElementInfo.GetElementFromInt(comboData.aTag));
            newElement.Intensity += comboData.aIntensityChange;
            comboAdj.Add(newElement);
        }
        else 
        {
            comboAdj[listLocA].Intensity += comboData.aIntensityChange;
        }

        
    }

    private void AddSingleIntensities(List<ElementInfo> elementList1, List<ElementInfo> elementList2, SingleData singleData)
    {
        //mod is attack, adj is tile and vice versa
        //mod
        //if (elementList1[singleData.mTag] == null)
        int listLoc1 = -1;
        int listLoc2 = -1;

        for(int i = 0; i < elementList1.Count; i++) 
        {
            if (elementList1[i].Element == ElementInfo.GetElementFromInt(singleData.mTag))
            {
                listLoc1 = i;
                break;
            }
        }

        for (int i = 0; i < elementList2.Count; i++)
        {
            if (elementList2[i].Element == ElementInfo.GetElementFromInt(singleData.aTag))
            {
                listLoc2 = i;
                break;
            }
        }


        if (listLoc1 == -1)
        {
            ElementInfo newElement = new ElementInfo(ElementInfo.GetElementFromInt(singleData.mTag));
            newElement.Intensity = singleData.mIntensityChange;
            elementList1.Add(newElement);
        }
        else 
        { 
            elementList1[listLoc1].Intensity += singleData.mIntensityChange; 
        }

        //adj
        //if (elementList2[singleData.aTag] == null)
        if (listLoc2 == -1)
        {
            ElementInfo newElement = new ElementInfo(ElementInfo.GetElementFromInt(singleData.aTag));
            newElement.Intensity = singleData.aIntensityChange;
            elementList2.Add(newElement);
        }
        else 
        {
            elementList2[listLoc2].Intensity += singleData.aIntensityChange;
        }

        
    }

    //calculate intensity change with all elements in attack and tile 
    public void CalculateIntensityChange()
    {
        //Single Interact
        for (int sItr = 0; sItr < singleInteract.Count; sItr++)
        {
            SingleData singleData= singleInteract[sItr]; 
            //Modifier

            //get the new percent
            //ADD SPREAD EFFECT LATER
            double modUseability = singleData.mUse;
            double totalUseability;
            double originalIntensity = 0.0;

            //if mod is attack, then get the intensity of attack and totalUseability
            if (singleData.markMod)
            {
                for (int i = 0; i < attackElementsList.Count; i++)
                {
                    if ((int)attackElementsList[i].Element == singleData.mTag)
                    {
                        originalIntensity = attackElementsList[i].Intensity;
                        break;
                    }
                }
                //originalIntensity = attackElementsList[singleData.mTag].Intensity;
                totalUseability = attackTotalUseability[singleData.mTag];
            }
            //mod from tile intensity and totalUseability
            else
            {
                for(int i = 0; i < tileElementsList.Count; i++) 
                {
                    if ((int)tileElementsList[i].Element == singleData.mTag)
                    { 
                        originalIntensity= tileElementsList[i].Intensity;
                        break;
                    }
                }
                //originalIntensity = tileElementsList[singleData.mTag].Intensity;
                totalUseability = tileTotalUseability[singleData.mTag];
            }
            double newPercent = EquationCalculate(totalUseability);
            //calculate the intensity change
            double mIntensityChange = modUseability / totalUseability * newPercent / 100.0 * originalIntensity;
            singleData.mIntensityChange= -mIntensityChange;
            //calculate adj
            double mStr = singleData.mStr;
            double aIntensityChange = mIntensityChange * mStr * singleData.aConv;
            singleData.aIntensityChange= aIntensityChange;
            //reupload values
            singleInteract[sItr] = singleData;
        }

        //combo
        for(int cItr = 0; cItr < comboInteract.Count; cItr++) 
        { 
            ComboData comboData = comboInteract[cItr];
            double modUseability1 = comboData.mUse1;
            double modUseability2 = comboData.mUse2;
            double totalUseability1; 
            double totalUseability2;
            double originalIntensity1 = 0.0;
            double originalIntensity2 = 0.0;

            //if 1 and adj are attacks, 2 is tile
            if (comboData.markMod1)
            {
                totalUseability1 = attackTotalUseability[comboData.mTag1];
                totalUseability2 = tileTotalUseability[comboData.mTag2];

                //find the attackElement that is being used for mTag1
                for (int i = 0; i < attackElementsList.Count; i++)
                {
                    if ((int)attackElementsList[i].Element == comboData.mTag1)
                    {
                        originalIntensity1 = attackElementsList[i].Intensity;
                        break;
                    }
                }
                //find tileElement that is being used for Tag2
                for (int i = 0; i < tileElementsList.Count; i++)
                {
                    if ((int)tileElementsList[i].Element == comboData.mTag2)
                    {
                        originalIntensity2 = tileElementsList[i].Intensity;
                        break;
                    }
                }
                //originalIntensity2 = tileElementsList[comboData.mTag2].Intensity;

            }
            //1 and adj are tiles, 2 is attack
            else 
            {
                totalUseability1 = tileTotalUseability[comboData.mTag1];
                totalUseability2 = attackTotalUseability[comboData.mTag2];

                //find tileElement that is being used for Tag1
                for (int i = 0; i < tileElementsList.Count; i++)
                {
                    if ((int)tileElementsList[i].Element == comboData.mTag1)
                    {
                        originalIntensity2 = tileElementsList[i].Intensity;
                        break;
                    }
                }
                //find the attackElement that is being used for mTag2
                for (int i = 0; i < attackElementsList.Count; i++)
                {
                    if ((int)attackElementsList[i].Element == comboData.mTag2)
                    {
                        originalIntensity1 = attackElementsList[i].Intensity;
                        break;
                    }
                }
                //originalIntensity2 = attackElementsList[comboData.mTag2].Intensity;
            }
            //ADD SPREAD TO TOTALUSEABILITY
            double newPercent1 = EquationCalculate(totalUseability1);
            double newPercent2 = EquationCalculate(totalUseability2);

            double need1 = comboData.mNeed1;
            double need2 = comboData.mNeed2;
            //calculate the intensity change
            double mIntensityChange1 = modUseability1 / totalUseability1 * newPercent1 / 100.0 * originalIntensity1;
            double mIntensityChange2 = modUseability2 / totalUseability2 * newPercent2 / 100.0 * originalIntensity2;

            
            double mConverted1 = mIntensityChange1 / need1;
            double mConverted2 = mIntensityChange2 / need2;
            double smallerConverted;
            
            //check which ingredient is converting the fewer amount and set it to that. 

            //2 is smaller
            if (mConverted1 > mConverted2)
            {
                smallerConverted = mConverted2;
                //set mConverted1 set to mConverted2. So, need to change how much intensityChange1 needs to be so mConverted1 = mConverted2

                mIntensityChange1 = mConverted2 / modUseability1 * totalUseability1 / newPercent1 * 100.0 / originalIntensity1 * need1;
            }
            else if(mConverted1 < mConverted2) 
            {
                smallerConverted = mConverted1;
                //set mConverted2 set to mConverted1. So, need to change how much intensityChange2 needs to be so mConverted2 = mConverted1
                mIntensityChange2 = mConverted1 / modUseability2 * totalUseability2 / newPercent2 * 100.0 / originalIntensity2 * need2;
            }

            //if equal
            else
            {
                smallerConverted = mConverted1;
            }

            double aIntensityChange = smallerConverted * comboData.aConv;

            //set all
            comboData.mIntensityChange1 = -mIntensityChange1;
            comboData.mIntensityChange2 = -mIntensityChange2;
            comboData.aIntensityChange = aIntensityChange;
            //upload all the data to the list
            comboInteract[cItr] = comboData;
        }
        //Add spreading effect

    }

    private double EquationCalculate(double totalUseability)
    {
        return 100.0 / (1 + Mathf.Exp((float)(-0.04 * (totalUseability - 90))));
    }

    public void CalculateInteractions()
    {
        for (int attItr = 0; attItr < attackElementsList.Count; attItr++)
        {
            //mark EOI, mark recipe results
            CalculateInteractionsPerElement(attackElementsList[attItr].Element, tileElementsList, true);
        }
        //tile on attack
        for (int tileItr = 0; tileItr < tileElementsList.Count; tileItr++)
        {
            //mark interact/attack
            CalculateInteractionsPerElement(tileElementsList[tileItr].Element, attackElementsList, false);
        }

        for(int i = 0; i < comboInteract.Count; i++) 
        {
            Debug.Log("Combo number: " + i + "Element Tag 1: " + comboInteract[i].mTag1 + " Useability " + comboInteract[i].mUse1 + "Element Tag 2: " + comboInteract[i].mTag2 + " Useability " + comboInteract[i].mUse2);
        }
    }

    //update EOI with interacting Elements
    private void CalculateInteractionsPerElement(ElementInfo.AllElements elementOfInterest, List<ElementInfo> interactingElements, bool isEOIAttack)
    {
        //finding the elementcombo type interaction info for the EOI
        for (int i = 0; i < AllInteractions.Count; i++)
        {
            //get the info
            if (AllInteractions[i].ElementOfInterest == elementOfInterest)
            {
                //now go through the interacting elements
                for (int j = 0; j < interactingElements.Count; j++)
                {
                    //RECIPES
                    for (int rec = 0; rec < AllInteractions[i].ComboWithEOI.Count; rec++)
                    {
                        //loop through all the ingredients in the recipe
                        for (int ing = 0; ing < AllInteractions[i].ComboWithEOI[rec].Ingredient.Count; ing++)
                        {
                            //get the element in the ingredients for recipe
                            
                            Ingredients ElementIngredient = AllInteractions[i].ComboWithEOI[rec].Ingredient[ing];
                            //if the interacting element is matched in recipe and not the same as the EOI (Or else it will be like oh EOI is water and you're being attacked by water? Might as well activate the cloud combo)
                            if (interactingElements[j].Element == ElementIngredient.Element && interactingElements[j].Element != elementOfInterest)
                            {
                                ComboAdd();
                                //get the useability, strength, conversion of recipe to combo list with EOI as attack
                                GetRecipeInteraction(AllInteractions[i].ComboWithEOI[rec], elementOfInterest, isEOIAttack);
                                //don't need to check more of this recipe
                                break;
                            }
                        }

                    }

                    //BOOST
                    for (int boost = 0; boost < AllInteractions[i].BoostElements.Count; boost++)
                    {
                        if (interactingElements[j].Element == AllInteractions[i].BoostElements[boost].Element)
                        {
                            SingleAdd();
                            SingleInteractions boostElements = AllInteractions[i].BoostElements[boost];
                            BoostWeakCalculations(elementOfInterest, boostElements, +1.0, isEOIAttack);
                        }
                        
                    }

                    //WEAK
                    for (int weak = 0; weak < AllInteractions[i].WeakElements.Count; weak++)
                    {
                        if (interactingElements[j].Element == AllInteractions[i].WeakElements[weak].Element) 
                        {
                            SingleAdd();
                            SingleInteractions weakElements = AllInteractions[i].WeakElements[weak];
                            BoostWeakCalculations(elementOfInterest, weakElements, -1.0, isEOIAttack);
                        }
                    }

                }
            }
        }
    }

    private void BoostWeakCalculations(ElementInfo.AllElements EOI, SingleInteractions bwElements, double conv, bool isEOIAttack)
    {
        
        //EOI is attack (attack is being boosted)
        if (isEOIAttack)
        {
            SingleUpdateModMark(false);
            SingleUpdateAdjMark(true);
            //the interacting is tile
            tileTotalUseability[(int)bwElements.Element] += bwElements.InteractionUseability;
        }
        else
        {
            SingleUpdateModMark(true);
            SingleUpdateAdjMark(false);
            //interacting is attack
            attackTotalUseability[(int)bwElements.Element] += bwElements.InteractionUseability;
        }

        //add modifier
        SingleUpdateModifier(bwElements.InteractionUseability, bwElements.InteractionStrength, (int)bwElements.Element);
        //add the adjusted
        SingleUpdateAdjusted(conv, (int)EOI);
    }


    private void GetRecipeInteraction(ElementCombo recipe, ElementInfo.AllElements elementOfInterest, bool isEOIAttack)
    {
        List<Ingredients> ingredients = recipe.Ingredient;

        //Add all the ingredients from recipe to row
        for (int ing = 0; ing < ingredients.Count; ing++)
        {
            
            //if this is attack element
            if ((ingredients[ing].Element == elementOfInterest && isEOIAttack) || (ingredients[ing].Element != elementOfInterest && !isEOIAttack))
            {
                ComboUpdateMod1(ingredients[ing].IntensityUsability, ingredients[ing].IntensityNeeded, (int)ingredients[ing].Element);
                ComboUpdateMarkMod1(true);
                attackTotalUseability[(int)ingredients[ing].Element] += ingredients[ing].IntensityUsability;
            }
            //not attack element
            else
            {
                ComboUpdateMod2(ingredients[ing].IntensityUsability, ingredients[ing].IntensityNeeded, (int)ingredients[ing].Element);
                ComboUpdateMarkMod2(false);
                tileTotalUseability[(int)ingredients[ing].Element] += ingredients[ing].IntensityUsability;
            }

           
        }


        ComboUpdateAdj(recipe.ConversionRate, (int)recipe.Result);
        //if EOI is attack, make recipe marked
        if(isEOIAttack) 
        {
            ComboUpdateMarkAdj(true);
        }
        else 
        { 
            ComboUpdateMarkAdj(false);
        }
    }

}

