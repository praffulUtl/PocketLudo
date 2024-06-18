using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRed2 : MonoBehaviour
{
    public static string redPlayerColName2;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            redPlayerColName2 = collision.gameObject.name;
            //if (redPlayerColName2 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        redPlayerColName2 = "none";
    }

}
