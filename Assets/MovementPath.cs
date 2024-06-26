using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
    [SerializeField] int pathBlockIndex = -1;
    private void Start()
    {
        pathBlockIndex = transform.GetSiblingIndex();
    }
}
