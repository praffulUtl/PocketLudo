using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGreen1 : MonoBehaviour
{
    public static string greenPlayerColName1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            greenPlayerColName1 = collision.gameObject.name;
            //if (greenPlayerColName1 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        greenPlayerColName1 = "none";

    }
}
