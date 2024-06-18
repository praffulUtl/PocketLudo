using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerYellow3 : MonoBehaviour
{
    public static string yellowPlayerColName3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            yellowPlayerColName3 = collision.gameObject.name;
            //if (bluePlayerColName1 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        yellowPlayerColName3 = "none";

    }
}
