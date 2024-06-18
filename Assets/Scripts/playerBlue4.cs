using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBlue4 : MonoBehaviour
{
    public static string bluePlayerColName4;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            bluePlayerColName4 = collision.gameObject.name;
            //if (bluePlayerColName1 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        bluePlayerColName4 = "none";

    }
}
