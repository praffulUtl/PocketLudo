using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [Header("Profile section")]
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] RawImage image;
    [Header("Edit section")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button pickImageBt;

    private void Start()
    {
        
    }

    void LoadProfileData()
    {
    }
}
