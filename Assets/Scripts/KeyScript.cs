using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//the keys from the user/AI
public class KeyScript : MonoBehaviour
{
    private GameObject selfObject;
    //public GameObject bar;
    [SerializeField] private float moveSpeed = 50;
    public SpriteRenderer barSr;
    //public GameObject playerText;
    public string user;
    public GameObject userBar;

    public CharacterScript owner;
    // Start is called before the first frame update
    void Start()
    {
      
        barSr = userBar.GetComponent<SpriteRenderer>();

        selfObject = this.gameObject;

       // playerText = GameObject.FindGameObjectWithTag("PlayerMove");


    }


    // Update is called once per frame
    void Update()
    {

        //moving the key
        transform.position = transform.position + (Vector3.down * moveSpeed) * Time.deltaTime;

        //get the dimension of the key
        SpriteRenderer charSr = selfObject.GetComponent<SpriteRenderer>();

        var charHeight = (charSr.sprite.bounds.extents.y * 2) * selfObject.transform.localScale.y;

        var barHeight = (barSr.sprite.bounds.extents.y * 2) * userBar.transform.localScale.y;

        //Destroys once it reaches end of the charBar
        if ((transform.position.y + charHeight / 2) < userBar.transform.position.y - barHeight / 2)
        {
            //destroys it in the array
            owner.ArrayKeys(false, gameObject);
            Destroy(gameObject);

        }



    }
}
