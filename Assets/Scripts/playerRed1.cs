using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRed1 : MonoBehaviour
{
    public static string redPlayerColName1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="blocks")
        {
            redPlayerColName1 = collision.gameObject.name;
            //if(redPlayerColName1=="")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        redPlayerColName1 = "none";
        
    }
}
