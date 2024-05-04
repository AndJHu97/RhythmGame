using Mono.Cecil.Cil;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//NOT USING
public class LogicScript : MonoBehaviour
{
    
   
   
    public TextMeshProUGUI playerHealthDisplay;
    public TextMeshProUGUI AIText;
    public TextMeshProUGUI AIHealthDisplay;
    public string playerState;
    public GameObject AIBar;
    public GameObject playerBar;
    public MoveSets playerMove;
    //public MoveSets AIMove;
    private double AIHealth;
    private double playerHealth;
   
    private readonly List<GameObject> AIKeys = new();
   

    

 

    

   
    private void DamageCalc(int damage, double damageChance, int block, double blockChance, TextMeshProUGUI HealthDisplay, ref double Health)
    {
        double randomValue = UnityEngine.Random.value;
        if (randomValue <= blockChance)
        {
            damage -= block;
        }

        if (damage < 0) { damage = 0; }

        randomValue= UnityEngine.Random.value;
        if (randomValue <= damageChance)
        {
            Health -= damage;
        }
        HealthDisplay.text = Health.ToString();

    }

 


    private void Update()
    {
        //PlayerAttackDuration();
        //AIAttackDuration();
        //PlayerAttack();
        //EnemyAttack();
      


        
    }

    private void Start()
    {
    }
}
