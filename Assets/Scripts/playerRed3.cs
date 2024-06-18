using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRed3 : MonoBehaviour
{
    public static string redPlayerColName3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            redPlayerColName3 = collision.gameObject.name;
            //if (redPlayerColName3 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        redPlayerColName3 = "none";
    }

}
