using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGreen3 : MonoBehaviour
{
    public static string greenPlayerColName3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "blocks")
        {
            greenPlayerColName3 = collision.gameObject.name;
            //if (greenPlayerColName4 == "")
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        greenPlayerColName3 = "none";

    }
}
