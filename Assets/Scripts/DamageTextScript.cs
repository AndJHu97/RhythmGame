using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextScript : MonoBehaviour
{
    public float TimeSeconds;
    // Start is called before the first frame update
    void Start() { }
    

    // Update is called once per frame
    void Update()
    {
        if (TimeSeconds > 0)
        {
            TimeSeconds -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
