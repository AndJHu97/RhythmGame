using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterScript : MonoBehaviour
{
    public string Name;
    public double Health;
    public double Attack;
    public double Defense;
    public ElementInfo.AllElements Type;
    public GameObject Move1;
    public GameObject Move2;
    public GameObject Move3;
    public GameObject Move4;

    //Set to script of moves in Start method
    protected AttackScript move1Script;
    protected AttackScript move2Script;
    protected AttackScript move3Script;
    protected AttackScript move4Script;
   
    public TextMeshProUGUI textDisplay;
    public GameObject keyBar;
    public TextMeshProUGUI healthDisplay;
    public TextMeshProUGUI keyPressesDisplay;
    public GameObject DamageTextPrefab;
    public Canvas CanvasPrefab;
    protected List<GameObject> listOfKeys = new();
    protected float attackSpawnOffset = 1.5f;
    
    //get from Keyspawner the list of keys to keep track

    public void Start()
    {
        //get attack script of each move pre-fab
        move1Script = Move1.GetComponent<AttackScript>();
        move2Script = Move2.GetComponent<AttackScript>();
        move3Script = Move3.GetComponent<AttackScript>();
        move4Script = Move4.GetComponent<AttackScript>();
    }
    public void ArrayKeys(bool isAdd, GameObject key)
    {

        if (isAdd == true)
        {

            listOfKeys.Add(key);
        }
        else
        {
            listOfKeys.RemoveAt(0);

        }


    }

    //create attack from the player move script
    public void CreateAttack(GameObject move)
    {
        //Show text
        AttackScript attackScript = move.GetComponent<AttackScript>();
        textDisplay.text = attackScript.Text;
        Vector3 attackPosition = Vector3.zero;
        //had to define to flip for enemy stationary attack
        int attackStationaryLocation = attackScript.Location;

        //check where the position is at
        if (gameObject.tag == "Enemys")
        {
            attackPosition = transform.position - transform.right * attackSpawnOffset;
        }
        else
        //player
        {
            attackPosition = transform.position + transform.right * attackSpawnOffset;
        }

        //if stationary and enemy, then flip
        if(gameObject.tag == "Enemys" && attackScript.IsStationary)
        {
            attackStationaryLocation = 4 - attackScript.Location;
            Debug.Log("Rainstorm location: " + attackStationaryLocation);
        }

        //If stationary, starts at different location
        if (attackScript.IsStationary)
        {
            if(attackStationaryLocation == 1)
            {
                attackPosition = GameObject.FindWithTag("Tile1").transform.position;
            }
            else if(attackStationaryLocation == 2)
            {
                attackPosition = GameObject.FindWithTag("Tile2").transform.position;
            }
            else
            {
                attackPosition = GameObject.FindWithTag("Tile3").transform.position;
            }
        }

        GameObject attackInstance = Instantiate(move, attackPosition, Quaternion.identity);

       

        attackInstance.layer = LayerMask.NameToLayer("AttackLayer");

        //who launched it
        attackInstance.tag = gameObject.tag;

    }

    public void DamageCalc(double damage, double damageChance)
    {
        damageChance /= 100.0;
        damage = Math.Max(damage, 0.0);
        double randomValue = UnityEngine.Random.value;
        if (randomValue <= damageChance)
        {
            Health -= damage;
            //round to 2 decimal places
            Health = Math.Round(Health, 2);
            ShowDamageText(damage);
        }

        healthDisplay.text = Health.ToString();

    }

    private void ShowDamageText(double damageAmount)
    {
        // Instantiate damage text prefab as a child of the Canvas
        GameObject damageTextObject = Instantiate(DamageTextPrefab, transform.position, Quaternion.identity, CanvasPrefab.transform);

        // Get the TextMeshProUGUI component from the instantiated damage text object
        TextMeshProUGUI damageTextComponent = damageTextObject.GetComponent<TextMeshProUGUI>();

        // Set the text to display the damage amount
        damageTextComponent.text = "-" + damageAmount.ToString();

        // Optionally, you can customize the appearance of the text here
    }

    //register key click. True if at right place
    public bool CheckKeyLocation(GameObject lowestKey)
    {
        //get the char bar dimensions
        SpriteRenderer barSr;
        SpriteRenderer keySr = lowestKey.GetComponent<SpriteRenderer>();
        barSr = keyBar.GetComponent<SpriteRenderer>();

        //wtf
        var keyHeight = (keySr.sprite.bounds.extents.y * 2) * lowestKey.transform.localScale.y;

        var barHeight = (barSr.sprite.bounds.extents.y * 2) * keyBar.transform.localScale.y;

        //if the key is at right place when clicking
        return (lowestKey.transform.position.y - keyHeight / 2) < (keyBar.transform.position.y - barHeight / 2);


    }

    public virtual void MoveSelection() { }

  

 

    private void Update()
    {
        /*
        if (CompareTag("Players"))
        { 
            PlayerMove(); 
        }
        else if (CompareTag("Enemys"))
        {
            EnemyMove();
        }
        */

    }
}
