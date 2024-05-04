using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ElementCombo;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Profiling.LowLevel;
using UnityEditor.SceneManagement;
using System.Xml.Serialization;
using JetBrains.Annotations;

public class ElementInteractionManager : MonoBehaviour
{
    
    //Each list item is the element of interest
    public List<TypeInteractionInfo>  AllInteractions = new List<TypeInteractionInfo>();

    //Combo (as ingredient), boost, weak elements
    [System.Serializable]
    public class TypeInteractionInfo {
        //element of interest
        public ElementInfo.AllElements ElementOfInterest;
        [Header("Ingredients as Combo")]
        public List<ElementCombo> ComboWithEOI = new();
        [Header("Boost")]
        public List<SingleInteractions> BoostElements = new();
        [Header("Weakness")]
        public List<SingleInteractions> WeakElements= new();
    }
    
    //The properties in each boost and weak
    [System.Serializable]
    public class SingleInteractions
    {
        public ElementInfo.AllElements Element;
        public double InteractionStrength;
        public double InteractionUseability;
    }

    //
    public class ElementsComparer : IComparer<ElementInfo>
    {
        public int Compare(ElementInfo x, ElementInfo y)
        {
            // Compare elements by their intensity in descending order
            return y.Intensity.CompareTo(x.Intensity);
        }
    }

    //Calculate new list of element
    public List<ElementInfo> SortLargeToSmallElementList(List<ElementInfo> elementList)
    {
        elementList.Sort(new ElementsComparer());
        return elementList;
    }
    
    public ElementInfo ReturnMainElement(List<ElementInfo> elementList)
    {
        if (elementList.Count > 0)
        {
            int elementCutoff = 30;
            elementList = SortLargeToSmallElementList(elementList);
            if (elementList[0].Intensity > elementCutoff)
            {
                return elementList[0];
            }
        }
        
        return null;
    }

    //Return back the interaction between tile and attack
    public void InteractTileAttack(List<ElementInfo> attackElementsList, List<ElementInfo> tileElementsList)
    {
        AttackTileInteractionTask attackTileInteractionTask = new(attackElementsList, tileElementsList, AllInteractions);
        //get all the data to calculate intensity 
        attackTileInteractionTask.CalculateInteractions();
        //calculate the intensity change
        attackTileInteractionTask.CalculateIntensityChange();
        //add all the intensities together to attack and tile elements
        attackTileInteractionTask.AddFinalIntensities();

        //If like water is 0.01 intensity, might as well make it 0. Do it for attack and tile
        for(int i = 0; i < attackElementsList.Count; i++)
        {
            if (attackElementsList[i].Intensity < 1)
            {
                attackElementsList.RemoveAt(i);
            }
        }

        for(int i = 0; i < tileElementsList.Count; i++)
        {
            if (tileElementsList[i].Intensity < 1)
            {
                tileElementsList.RemoveAt(i);
            }
        }
    }

    //interactions within the tile
    public void WithinTileInteract(List<ElementInfo> tileElementsList)
    {
        tileElementsList = SortLargeToSmallElementList(tileElementsList);

        for(int i = 0; i < tileElementsList.Count; i++)
        {
            List<ElementInfo> elementOfInterest = new List<ElementInfo> { tileElementsList[0] };
            tileElementsList.RemoveAt(0);

            //use AttackTileINteractions because I'm too lazy to build a new one
            AttackTileInteractionTask TileInteractionTask = new(elementOfInterest, tileElementsList, AllInteractions);
            //get all the data to calculate intensity 
            TileInteractionTask.CalculateInteractions();
            //calculate the intensity change
            TileInteractionTask.CalculateIntensityChange();
            //add all the intensities together to attack and tile elements
            TileInteractionTask.AddFinalIntensities();

            tileElementsList.Add(elementOfInterest[0]);
        }

        //if intensity too small, then delete it
        for (int i = 0; i < tileElementsList.Count; i++)
        {
            if (tileElementsList[i].Intensity < 1)
            {
                tileElementsList.RemoveAt(i);
            }
        }
    }


    /*
    //calculate usability, strength, conversion of boostweakness, and combo
    private void CalculateInteractionsPerElement(ElementInfo elementOfInterest, List<ElementInfo> interactingElements, ref List<List<double[]>> boostweakUseStr, ref List<List<double[]>> comboUseNeedConv, ref double[] totalUseabilityForEachElement)
    {
        //to access the correct array row in the boostweakEffStr 
        int rowUseability = 0;
        int rowStrength = 1;
        //store totalEfficiency for all
        //double[] totalEfficiencyForEachElement = new double[(int)ElementInfo.AllElements.size + 1];
        //finding the elementcombo type interaction info for the EOI
        for (int i = 0; i < AllInteractions.Count; i++)
        {
            //get the info
            if (AllInteractions[i].ElementOfInterest == elementOfInterest) 
            { 
                //now go through the interacting elements
                for(int j = 0; j < interactingElements.Count; j++) 
                {
                    //RECIPES
                    for (int rec = 0; rec < AllInteractions[i].ElementIngredientInCombo.Count; rec++)
                    {
                        //loop through all the ingredients in the recipe
                        for (int ing = 0; ing < AllInteractions[i].ElementIngredientInCombo[rec].Ingredient.Count; ing++)
                        {
                            //get the element in the ingredients for recipe
                            Ingredients ElementIngredient = AllInteractions[i].ElementIngredientInCombo[rec].Ingredient[ing];

                            //if the interacting element is matched in recipe
                            if (interactingElements[j] == ElementIngredient.ElementInfo)
                            {
                                //get the efficiency, strength, conversion of recipe to combo list
                                GetRecipeInteraction(ref comboUseNeedConv, AllInteractions[i].ElementIngredientInCombo[rec], ref totalUseabilityForEachElement);
                                //don't need to check more of this recipe
                                break;
                            }
                        }
                        
                    }

                    //BOOST
                    for (int boost = 0; boost < AllInteractions[i].BoostElements.Count; boost++)
                    {
                        //create array
                        double[] newBoostArray = new double[2];
                        newBoostArray[rowUseability] = AllInteractions[i].BoostElements[boost].InteractionEfficiency;
                        newBoostArray[rowStrength] = AllInteractions[i].BoostElements[boost].InteractionStrength;

                        //create new row for list
                        List<double[]> newRow = new((int)ElementInfo.AllElements.size + 1);
                        newRow[(int)AllInteractions[i].BoostElements[boost].ElementInteractions.Element] = newBoostArray;
                        //add to item being boosted
                        newRow[(int)elementOfInterest.Element] = new double[] { +1 };

                        //add to the list of the element of interest 
                        boostweakUseStr.Add(newRow);
                        //add to totalEfficiency by the element that is doing the boosting
                        totalUseabilityForEachElement[(int)AllInteractions[i].BoostElements[boost].ElementInteractions.Element] += newBoostArray[rowUseability];
                    }

                    //WEAK
                    for(int weak = 0; weak < AllInteractions[i].WeakElements.Count; weak++) 
                    {
                        //create array
                        double[] newWeakArray = new double[2];
                        newWeakArray[rowUseability] = AllInteractions[i].WeakElements[weak].InteractionEfficiency;
                        newWeakArray[rowStrength] = AllInteractions[i].WeakElements[weak].InteractionStrength;

                        //create new row for list
                        List<double[]> newRow = new((int)ElementInfo.AllElements.size + 1);
                        newRow[(int)AllInteractions[i].WeakElements[weak].ElementInteractions.Element] = newWeakArray;
                        //add to item being boosted to check if it is positive or negative
                        newRow[(int)elementOfInterest.Element] = new double[] { -1 };

                        //add to the list of the element of interest 
                        boostweakUseStr.Add(newRow);
                        //add to totalEfficiency by the element that is doing the boosting
                        totalUseabilityForEachElement[(int)AllInteractions[i].BoostElements[weak].ElementInteractions.Element] += newWeakArray[rowUseability];
                    }

                }
            }
        }
    }

    //Overload with storing in EOI separate interaction data, totalUseability, and mark. Can mark EOI, interacting, and/or reciperesult. tileAttack 
    private void CalculateInteractionsPerElement(ElementInfo elementOfInterest, List<ElementInfo> interactingElements, ref List<List<double[]>> boostweakUseStr, ref List<List<double[]>> comboUseNeedConv, ref double[] totalUseabilityForEOI, ref double[] totalUseabilityForInteracting, bool markEOI, bool markInteracting, bool markRecipeResult)
    {
        //to access the correct array row in the boostweakEffStr 
        int rowUseability = 0;
        int rowStrength = 1;
        //store totalEfficiency for all
        //double[] totalEfficiencyForEachElement = new double[(int)ElementInfo.AllElements.size + 1];
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
                    for (int rec = 0; rec < AllInteractions[i].ElementIngredientInCombo.Count; rec++)
                    {
                        //loop through all the ingredients in the recipe
                        for (int ing = 0; ing < AllInteractions[i].ElementIngredientInCombo[rec].Ingredient.Count; ing++)
                        {
                            //get the element in the ingredients for recipe
                            Ingredients ElementIngredient = AllInteractions[i].ElementIngredientInCombo[rec].Ingredient[ing];

                            //if the interacting element is matched in recipe
                            if (interactingElements[j] == ElementIngredient.ElementInfo)
                            {
                                //get the efficiency, strength, conversion of recipe to combo list
                                GetRecipeInteraction(ref comboUseNeedConv, AllInteractions[i].ElementIngredientInCombo[rec], elementOfInterest, ref totalUseabilityForEOI, ref totalUseabilityForInteracting, markEOI, markInteracting, markRecipeResult);
                                //don't need to check more of this recipe
                                break;
                            }
                        }

                    }

                    //BOOST
                    for (int boost = 0; boost < AllInteractions[i].BoostElements.Count; boost++)
                    {
                        double[] newBoostArray;
                        //Check if interacting type that is boosting is supposed to be marked
                        if (markInteracting)
                        {
                            newBoostArray = new double[3];
                            //mark with extra size array
                            newBoostArray[2] = markType;
                        }
                        else
                        {
                            //create array
                            newBoostArray = new double[2];
                        }
                        newBoostArray[rowUseability] = AllInteractions[i].BoostElements[boost].InteractionEfficiency;
                        newBoostArray[rowStrength] = AllInteractions[i].BoostElements[boost].InteractionStrength;

                        //create new row for list
                        List<double[]> newRow = new((int)ElementInfo.AllElements.size + 1);
                        newRow[(int)AllInteractions[i].BoostElements[boost].ElementInteractions.Element] = newBoostArray;
                        //add to item being boosted. Will mark EOI if needed
                        if (markEOI)
                        {
                            newRow[(int)elementOfInterest.Element] = new double[] { +1, markType };
                        }
                        else
                        {
                            newRow[(int)elementOfInterest.Element] = new double[] { +1 };
                        }

                        //add to the list of the element of interest 
                        boostweakUseStr.Add(newRow);
                        //add to totalEfficiency by the interacting list
                        totalUseabilityForInteracting[(int)AllInteractions[i].BoostElements[boost].ElementInteractions.Element] += newBoostArray[rowUseability];
                    }

                    //WEAK
                    for (int weak = 0; weak < AllInteractions[i].WeakElements.Count; weak++)
                    {
                        double[] newWeakArray;
                        //Check if interacting type that is boosting is supposed to be marked
                        if (markInteracting)
                        {
                            newWeakArray = new double[3];
                            //mark with extra size array
                            newWeakArray[2] = markType;
                        }
                        else
                        {
                            //create array
                            newWeakArray = new double[2];
                        }
                        newWeakArray[rowUseability] = AllInteractions[i].WeakElements[weak].InteractionEfficiency;
                        newWeakArray[rowStrength] = AllInteractions[i].WeakElements[weak].InteractionStrength;

                        //create new row for list
                        List<double[]> newRow = new((int)ElementInfo.AllElements.size + 1);
                        newRow[(int)AllInteractions[i].WeakElements[weak].ElementInteractions.Element] = newWeakArray;
                        //add to item being boosted. Will mark EOI if needed
                        if (markEOI)
                        {
                            newRow[(int)elementOfInterest.Element] = new double[] { +1, markType };
                        }
                        else
                        {
                            newRow[(int)elementOfInterest.Element] = new double[] { +1 };
                        }

                        //add the new row to the list
                        boostweakUseStr.Add(newRow);
                        //add to totalEfficiency by the element that is doing the boosting
                        totalUseabilityForInteracting[(int)AllInteractions[i].BoostElements[weak].ElementInteractions.Element] += newWeakArray[rowUseability];
                    }

                }
            }
        }
    }

    //returns the efficiency, strength, conversion of the list
    private void GetRecipeInteraction(ref List<List<double[]>> comboUseNeedConv, ElementCombo recipe, ElementInfo elementOfInterest, ref double[] totalUseabilityForEOI, ref double[] totalUseabilityForInteracting, bool markEOI, bool markInteracting, bool markRecipeResult)
    {
        List<Ingredients> ingredients = recipe.Ingredient;
        
        //for adding newRow everytime;
        List<double[]> newRow = new((int)ElementInfo.AllElements.size + 1);
        //array to add to newRow
        double[] newUseStrArray;

        //Add all the ingredients from recipe to row
        for (int ing = 0; ing < ingredients.Count; ing++) 
        {
            //if ing is EOI and markEOI or if not EOI and mark Interact
            if ((ingredients[ing].ElementInfo == elementOfInterest && markEOI) || (ingredients[ing].ElementInfo != elementOfInterest && markInteracting))
            {
                newUseStrArray = new double[3];
                newUseStrArray[2] = markType;
            }
            else 
            { 
                newUseStrArray= new double[2];
            }
            //efficiency. 
            newUseStrArray[0] = ingredients[ing].IntensityUsability;
            //strength
            newUseStrArray[1] = ingredients[ing].IntensityNeeded;
            //make newRow list from it.
            newRow[(int)ingredients[ing].ElementInfo.Element] = newUseStrArray;

            //add to totalEfficiency for the element of this ingredient and add to right one: EOI or interacting
            if (ingredients[ing].ElementInfo == elementOfInterest)
            { 
                totalUseabilityForEOI[(int)ingredients[ing].ElementInfo.Element] += newUseStrArray[0];
            }else
            {
                totalUseabilityForInteracting[(int)ingredients[ing].ElementInfo.Element] += newUseStrArray[0];
            }
            
        }

        //add result form recipe to newRow
        double[] newConversion;
        if (markRecipeResult)
        {
            newConversion = new double[2];
            newConversion[1] = markType;
        }
        else
        { 
            newConversion = new double[1];
        }
        newConversion[0] = recipe.ConversionRate;
        newRow[(int)recipe.Result.Element] = newConversion;
        //add the newRow to the existing combo list
        comboUseNeedConv.Add(newRow);

    }

    private void GetRecipeInteraction(ref List<List<double[]>> comboEffUseConv, ElementCombo recipe, ref double[] totalUseabilityForEachElement)
    {
        List<Ingredients> ingredients = recipe.Ingredient;

        //for adding newRow everytime;
        List<double[]> newRow = new((int)ElementInfo.AllElements.size + 1);
        //array to add to newRow
        double[] newUseStrArray = new double[2];

        //Add all the ingredients from recipe to row
        for (int ing = 0; ing < ingredients.Count; ing++)
        {
            //efficiency. 
            newUseStrArray[0] = ingredients[ing].IntensityUsability;
            //strength
            newUseStrArray[1] = ingredients[ing].IntensityNeeded;
            //make newRow list from it.
            newRow[(int)ingredients[ing].ElementInfo.Element] = newUseStrArray;

            //add to totalEfficiency for the element of this ingredient
            totalUseabilityForEachElement[(int)ingredients[ing].ElementInfo.Element] += newUseStrArray[0];
        }

        //add result form recipe to newRow
        double[] newConversion = new double[1];
        newConversion[0] = recipe.ConversionRate;
        newRow[(int)recipe.Result.Element] = newConversion;
        //add the newRow to the existing combo list
        comboEffUseConv.Add(newRow);

    }
    */
}
