using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGreen4 : MonoBehaviour
{
    public static string greenPlayerColName4;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            greenPlayerColName4 = collision.gameObject.name;
            //if (greenPlayerColName4 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        greenPlayerColName4 = "none";

    }
}
