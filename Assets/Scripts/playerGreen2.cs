using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGreen2 : MonoBehaviour
{
    public static string greenPlayerColName2;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            greenPlayerColName2 = collision.gameObject.name;
            //if (greenPlayerColName2 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        greenPlayerColName2 = "none";

    }
}
