using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScript : CharacterScript
{
    //As an AI. In percentage
    public double AICorrectPress;
    private int tryingForMove = 3;
    private int currentNumOfPresses = 0;

    public override void MoveSelection()
    {
        currentNumOfPresses++;
        //get what move is being done
        if (currentNumOfPresses == move1Script.Press.Length && tryingForMove == 1)
        {
            CreateAttack(Move1);
            
            //resets the move aiming for
            tryingForMove = UnityEngine.Random.Range(1, 5);
            
            currentNumOfPresses = 0;
        }
        else if (currentNumOfPresses == move2Script.Press.Length && tryingForMove == 2)
        {
            CreateAttack(Move2);
            tryingForMove = UnityEngine.Random.Range(1, 5);
            
            currentNumOfPresses = 0;
        }
        else if (currentNumOfPresses == move3Script.Press.Length && tryingForMove == 3)
        {
            CreateAttack(Move3);
            tryingForMove = UnityEngine.Random.Range(1, 5);
            
            currentNumOfPresses = 0;
        }
        else if (currentNumOfPresses == move4Script.Press.Length && tryingForMove == 4)
        {   
            CreateAttack(Move4);
            tryingForMove = UnityEngine.Random.Range(1, 5);
            currentNumOfPresses = 0;
        }


    }

    public void EnemyMove()
    {
        //tryingForMove decides which attack to launch
        if(tryingForMove == -1) { tryingForMove = UnityEngine.Random.Range(1, 5); }
        if (listOfKeys.Count > 0)
        {
            GameObject lowestKey = listOfKeys.First();

            //check if not inactivated
            if (lowestKey.CompareTag("ActivatedKey"))
            {

                //if the key is at right place when clicking
                if (CheckKeyLocation(lowestKey))
                {
                    //How accurate the enemy presses the right key
                    double AIChance = AICorrectPress / 100.00;
                    double randomValue = UnityEngine.Random.value;


                    if (randomValue <= AIChance)
                    {
                        switch (tryingForMove)
                        {
                            case 1:
                                {
                                    MoveSelection();
                                    break;
                                }
                            case 2:
                                {
                                    MoveSelection();
                                    break;
                                }
                            case 3:
                                {
                                    MoveSelection();
                                    break;
                                }
                            case 4:
                                {
                                    MoveSelection();
                                    break;
                                }


                        }

                        //delete keys
                        ArrayKeys(false, lowestKey);
                        Destroy(lowestKey);

                    }
                    else
                    {
                        //if click the wrong button
                        tryingForMove = UnityEngine.Random.Range(1, 5);
                        currentNumOfPresses = 0;

                    }
                    //Inactivate key 
                    lowestKey.tag = "Untagged";


                }
            }
        }
    }

    private void Update()
    {
        EnemyMove();
    }
}
