using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerYellow1 : MonoBehaviour
{
    public static string yellowPlayerColName1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            yellowPlayerColName1 = collision.gameObject.name;
            //if (bluePlayerColName1 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        yellowPlayerColName1 = "none";

    }
}
