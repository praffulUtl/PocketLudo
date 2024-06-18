using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRed4 : MonoBehaviour
{
    public static string redPlayerColName4;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            redPlayerColName4 = collision.gameObject.name;
            //if (redPlayerColName4 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        redPlayerColName4 = "none";
    }

}
