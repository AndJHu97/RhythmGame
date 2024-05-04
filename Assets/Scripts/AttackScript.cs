using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using TMPro;


public class AttackScript : MonoBehaviour
{
    
    //[HideInInspector] public GameObject CurrentMove;
    public ElementInteractionManager elementInteractionManager;
    private Rigidbody2D rb;

    //For stationary
    private double trackTimeToAttack;
    private bool StartCollision = false;
    private bool CheckTileCollision = false;
    private Collider2D objectCollided;

    [Header("Basic")]
    public string Name;
    public string Press;
    public int DamageChance;
    public float Damage;
    public float Block;
    public int BlockChance;
    public string Text;


    [Header("Attack-Attack Interactions")]
    public AttackAttackInteractions AttackAttackInteraction;

    public List<ElementInfo> ElementInfos;

    //the main elements that will be calculated to do damage. 
    //Re-input the intensity which would be used as the original intensity or using it as a base for percentage. Like current intensity/baseintensity * damage
    //typically just write the original intensity from ElementInfos, but just gives more control over how to calculate damage. Also easier for me to store values lol
    public List<ElementInfo> MainElements;

    [Header("Movement")]
    public float SpeedX;
    public float SpeedY;
    public double Height;

    //If the attack doesn't move
    public bool IsStationary;
    //which tile the attack is on
    public int Location;
    //The total time the attack lasts for
    public double TotalTime;
    //The amount of time it takes to do damage and element effects to tile
    public double TimeToAttack;
    

    [Header("Tile Effects")]
    public TileEffects TileEffect1;

    public TileEffects TileEffect2;

    public TileEffects TileEffect3;

    [System.Serializable]
    public class TileEffects
    {
        public bool isInteractable;
        //need to list the elements again so it knows what elements has what specific tile effects
        public ElementInfo.AllElements[] ElementEffectOnTile;
        public double[] TileStart;
        public double[] StatusStrength;
    }

    [System.Serializable]
    public class AttackAttackInteractions 
    {
        public ElementInfo.AllElements[] ElementEffectOnOtherAttack;
        public double[] StatusStrength;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //If enemy fire, then flip direction
        if(gameObject.tag == "Enemys")
        {
            rb.velocity = new Vector2(-SpeedX, 0f);
            // Get the current local rotation
            Vector3 rotation = transform.localEulerAngles;

            // Flip the sprite by rotating it 180 degrees along the Z-axis
            rotation.z += 180f;

            // Apply the new rotation
            transform.localEulerAngles = rotation;
            //Debug.Log("Enemy attack");
        }
        else
        {
            rb.velocity = new Vector2(SpeedX, SpeedY);
        }
        
        //Stationary, to not change TimeToAttack 
        if (IsStationary)
        {
            trackTimeToAttack = TimeToAttack;
        }

    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        CharacterCollision(collisionObject);
        //For stationary attacks on character
        StartCollision = true;
        AttackCollision(collisionObject);
    }

    //when hit character
    private void CharacterCollision(Collider2D collisionObject)
    {

        //if object collided with is on character layer
        if (collisionObject.gameObject.layer == LayerMask.NameToLayer("CharacterLayer"))
        {
            //for stationary attacks. It won't continuously check collision if unmoving so need to save the character being collided with
            objectCollided = collisionObject;
            //Moving or Stationary and trackTimeToAttack is ready
            if (!IsStationary || (trackTimeToAttack <= 0 && IsStationary))
            {
                //if attacking the opponent
                if ((collisionObject.gameObject.CompareTag("Enemys") && gameObject.CompareTag("Players")) || (collisionObject.gameObject.CompareTag("Players") && gameObject.CompareTag("Enemys")))
                {
                    //get the script of the character
                    CharacterScript collisionObjectScript = collisionObject.gameObject.GetComponent<CharacterScript>();

                    double elementDamageModifier = 1.0;
                    for (int i = 0; i < MainElements.Count; i++)
                    {
                        for (int j = 0; j < ElementInfos.Count; j++)
                        {
                            //finding the MainElements in ElementInfos to calculate elementDamageModifier by dividing mainElements original intensity
                            if (ElementInfos[j].Element == MainElements[i].Element)
                            {
                                elementDamageModifier *= (ElementInfos[j].Intensity / MainElements[i].Intensity);
                                break;
                            }
                        }
                    }
                    //damage calculations
                    collisionObjectScript.DamageCalc(Damage * elementDamageModifier, DamageChance);


                    //Moving object specific code
                    if(!IsStationary) 
                    { 
                        //Destroys gameObject automatically after damage
                        Destroy(gameObject); 
                    }
                    //Stationary object specific code
                    else
                    {
                        //Resets the TimeToAttack
                        trackTimeToAttack = TimeToAttack;
                    }
                }
            }
        }
    }

    

    //when hit attack, does block damage to other attacks
    private void AttackCollision(Collider2D collisionObject)
    {
        //if object collided with is on attack layer
        if (collisionObject.gameObject.layer == LayerMask.NameToLayer("AttackLayer"))
        {
            //if player hits opponent or vice versa
            if ((collisionObject.gameObject.CompareTag("Enemys") && gameObject.CompareTag("Players")) || (collisionObject.gameObject.CompareTag("Players") && gameObject.CompareTag("Enemys")))
            {
                double percentBlockChance = BlockChance / 100.0;
                double randomValue = UnityEngine.Random.value;
                //Chance that it blocks the other attack
                if (randomValue < percentBlockChance)
                {
                    //Take a constant off the attack
                    collisionObject.GetComponent<AttackScript>().Damage = Math.Max(collisionObject.GetComponent<AttackScript>().Damage - Block, 0.0f);
                    

                    //Calculate elemental exchange


                    //Get the type of element and keep track of it. This one is the object that is referencing original attack
                    //ElementInfo currentAttackElementInfo = new();
                    //adjust to statusstrength and use to interact
                    List<ElementInfo> adjustedAttackElementInfo = new();

                    //go through all of them
                    for (int indexOfElement = 0; indexOfElement < AttackAttackInteraction.ElementEffectOnOtherAttack.Length; indexOfElement++)
                    {


                        //status strength of attack-attack interactions
                        double statusStrength = AttackAttackInteraction.StatusStrength[indexOfElement] / 100.0;
                        //find the ElementInfo element, not ElementEffectOnTile since ElementInfo has info on intensity
                        int i = 0;
                        while (ElementInfos[i].Element != AttackAttackInteraction.ElementEffectOnOtherAttack[indexOfElement])
                        {
                            i++;
                        }
                        ElementInfo adjustedByStatusStrengthElement = new ElementInfo(ElementInfos[i].Element);
                        //get the amount based on status strength that is going to interact with the tiles
                        adjustedByStatusStrengthElement.Intensity = ElementInfos[i].Intensity * statusStrength;
                        adjustedAttackElementInfo.Add(adjustedByStatusStrengthElement);

                        //subtract the intensity used in the elementInteractionManger and will add back the adjusted amount later
                        ElementInfos[i].Intensity *= (1 - statusStrength);
                        //keep track and pass by object of the currentAttack

                        elementInteractionManager.InteractTileAttack(adjustedAttackElementInfo, collisionObject.GetComponent<AttackScript>().ElementInfos);

                        Debug.Log("Attack Collision added back: " + adjustedAttackElementInfo[0].Intensity);

                        //add back the adjustedAttackElementInfo with original
                        ElementInfos[i].Intensity += adjustedAttackElementInfo[0].Intensity;

                    }
                }
            }
        }
    }

    private void TileCollisionCheck()
    {

        //get tilestart array from each of the 3 tile start array. Could technically just use tileEffectsArray.TileStart but too lazy to change rn
        double[][] tileStartArray = new double[3][];
        tileStartArray[0] = TileEffect1.TileStart;
        tileStartArray[1] = TileEffect2.TileStart;
        tileStartArray[2] = TileEffect3.TileStart;
        //array of all the tileEffect
        TileEffects[] tileEffectsArray = { TileEffect1, TileEffect2, TileEffect3 };
        //index of the element within ElementEffectOnTile
        int indexOfElement = -1;
        //get the left of the first tile
        GameObject tileObject = GameObject.FindWithTag("Tile1");
        SpriteRenderer tileObjectSr = tileObject.GetComponent<SpriteRenderer>();

        SpriteRenderer attackSr = gameObject.GetComponent<SpriteRenderer>();
        Vector3 attackSrTop = transform.position + attackSr.bounds.extents.y * Vector3.up; //* attackSr.transform.localScale.y; 
        Vector3 attackSrRight = transform.position + attackSr.bounds.extents.x  * Vector3.right; //* attackSr.transform.localScale.x

        //the starting y position of the attack object
        Vector3 tileObjectSrTop = attackSrTop;
        double tileObjectHeight = (tileObjectSr.bounds.extents.y * 2);// * tileObjectSr.transform.localScale.y;
        double tileObjectWidth = (tileObjectSr.bounds.extents.x * 2); // * tileObjectSr.transform.localScale.x;

        
        Vector3 tileObjectSrLeft = tileObject.transform.position - tileObjectSr.bounds.extents.x * Vector3.right; //* tileObjectSr.transform.localScale.x 

        //Attack is moving or if trackTimeToAttack is ready and Stationary
        if (!IsStationary || (CheckTileCollision && IsStationary))
        {
            //Need to loop for 3 tiles and then loop for each element for each tile to activate tile collision

            //3 for the amount of tiles. Amount of array's needed
            for (int i = 0; i < tileEffectsArray.Length; i++)
            {
                //check if attack has tile interactions turned on
                if (tileEffectsArray[i].isInteractable)
                {
                    //Array of attack elements that affects the tile
                    ElementInfo.AllElements[] elementEffectOnTile = tileEffectsArray[i].ElementEffectOnTile;

                    //for the amount of element types it has for the tile
                    for (int j = 0; j < elementEffectOnTile.Length; j++)
                    {

                        //check if passed right of x then below y
                        if (attackSrRight.x >= (tileObjectSrLeft.x + (tileObjectWidth * tileStartArray[i][j])) && attackSrTop.y <= (tileObjectSrTop.y + tileObjectHeight * tileStartArray[i][j]))
                        {
                            //COLLIDED
                            //Check which tile you have passed
                            int newX = 1 + i;
                            string tagTile = "Tile" + newX.ToString();

                            tileObject = GameObject.FindWithTag(tagTile);

                            //get index of the ElementEffectOnTile
                            indexOfElement = j;
                            //Launch the Attack Collision code of the Tile
                            tileObject.GetComponent<TileScript>().AttackCollision(gameObject, indexOfElement);

                            //If moving, then stop interacting with tile again. Stationary keeps interacting with TimeToAttack
                            if (!IsStationary) 
                            {
                                //Don't check the tile again 
                                tileEffectsArray[i].isInteractable = false;
                            }

                            //resets back to false for stationary attacks
                            CheckTileCollision = false;
                        }

                    }
                }
            }
        }
    }

    private void SpeedYManager()
    {
        if(SpeedY > 0)
        {
            GameObject playerObject = GameObject.FindGameObjectsWithTag("Players")
            .FirstOrDefault(x => x.layer == LayerMask.NameToLayer("CharacterLayer"));

            GameObject enemyObject = GameObject.FindGameObjectsWithTag("Enemys")
            .FirstOrDefault(x => x.layer == LayerMask.NameToLayer("CharacterLayer"));

            float distance = Vector3.Distance(playerObject.transform.position, enemyObject.transform.position);

            if(gameObject.transform.position.x > distance / 2)
            {
                SpeedY = -SpeedY;
                rb.velocity = new Vector2(SpeedX, SpeedY);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        TileCollisionCheck();
        //Stationary timer
        if (IsStationary) { 
            trackTimeToAttack -= Time.deltaTime;
            TotalTime -= Time.deltaTime;

            //Since collisionchecks stops after a while not moving, need to do my own 'collision' checks
            if (StartCollision)
            {
                CharacterCollision(objectCollided);
                CheckTileCollision = true;
            }

            //If totalTime runs out then deletes object
            if(TotalTime <=0)
            {
                Destroy(gameObject);
            }
        }

        SpeedYManager();

    }
}
