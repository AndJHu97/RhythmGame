using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScript : CharacterScript
{
    private string currentKeyPresses = "";
    // Start is called before the first frame update
    public override void MoveSelection()
    {

        //get what move is being done
        if (currentKeyPresses == move1Script.Press)
        {
            CreateAttack(Move1);
            //resets the key presses
            currentKeyPresses = "";
            keyPressesDisplay.text = currentKeyPresses;
        }
        else if (currentKeyPresses == move2Script.Press)
        {
            CreateAttack(Move2);
            currentKeyPresses = "";
            keyPressesDisplay.text = currentKeyPresses;
        }
        else if (currentKeyPresses == move3Script.Press)
        {
            CreateAttack(Move3);
            currentKeyPresses = "";
            keyPressesDisplay.text = currentKeyPresses;
        }
        else if (currentKeyPresses == move4Script.Press)
        {
            CreateAttack(Move4);
            currentKeyPresses = "";
            keyPressesDisplay.text = currentKeyPresses;
        }

        //reset the keypresses if it fits with none of the current move sets
        string[] allMoveSets = new string[] { move1Script.Press, move2Script.Press, move3Script.Press, move4Script.Press };
        bool noMoveFits = true;

        for (int i = 0; i < allMoveSets.Length; i++)
        {
            if (currentKeyPresses.Length <= allMoveSets[i].Length)
            {
                if (currentKeyPresses == allMoveSets[i].Substring(0, currentKeyPresses.Length))
                {
                    noMoveFits = false;
                }
            }
        }

        if (noMoveFits)
        {
            currentKeyPresses = "";
            keyPressesDisplay.text = currentKeyPresses;
        }

    }

    //When player clicks on a key
    public void PlayerMove()
    {
        //for when player presses a key
        if (listOfKeys.Count > 0 && Input.anyKeyDown)
        {
            //get the dimension of the key that is closest to bottom and type of key press
            string keyPress = Input.inputString;
            GameObject lowestKey = listOfKeys.First();

            //check if not inactivated
            if (lowestKey.CompareTag("ActivatedKey"))
            {

                if (CheckKeyLocation(lowestKey))
                {
                    //add to current keyPresses
                    currentKeyPresses += keyPress;
                    keyPressesDisplay.text = currentKeyPresses;
                    MoveSelection();

                    //delete keys
                    ArrayKeys(false, lowestKey);
                    Destroy(lowestKey);


                }
                //if key is not at right place when clicking
                else
                {

                    lowestKey.tag = "Untagged";
                    textDisplay.text = "Missed!";
                    currentKeyPresses = "";
                    keyPressesDisplay.text = currentKeyPresses;
                    //changes color
                    Renderer keyRenderer = lowestKey.GetComponent<Renderer>();

                    Material originalMaterial = keyRenderer.sharedMaterial; // Get the original material from the prefab
                    Material instanceMaterial = new Material(originalMaterial); // Create a new material instance

                    instanceMaterial.color = Color.red; // Set the desired color

                    keyRenderer.material = instanceMaterial; // Assign the new material to the renderer of the prefab instance

                }
            }
            else
            {

                textDisplay.text = "Inactivated!";
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }
}
