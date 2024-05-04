using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//spawning the keys
public class KeySpawner : MonoBehaviour
{
    public GameObject key;
    public float spawnRate = 10;
    public CharacterScript owner;
    private float timer = 0;
    public string user;
    public GameObject userBar;
    // Start is called before the first frame update
    void Start()
    {
        //owner = GameObject.GetComponent<PlayerScript>();
        SpawnKey();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnKey();
            timer = 0;
        }

    }

    void SpawnKey()
    {
        //make the key
        GameObject InstantKey = Instantiate(key, transform.position, transform.rotation);
        //set whether AI keys or player keys. This will separate to separate arrays in logicscript. 
        //rest is just to initialize the keyscript in key object
        InstantKey.GetComponent<KeyScript>().user = user;
        InstantKey.tag = "ActivatedKey";
        InstantKey.GetComponent<KeyScript>().userBar = userBar;
        InstantKey.GetComponent<KeyScript>().owner= owner;
        //add to the array
        owner.ArrayKeys(true, InstantKey);
      
    }

}
