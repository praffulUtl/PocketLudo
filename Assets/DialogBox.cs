using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    [SerializeField] GameObject loadPnl;
    [SerializeField] TextMeshProUGUI text;
    public void Show(string Message)
    {
        Debug.Log("showing message :" +Message);
        text.text = Message;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        if (loadPnl != null)
            loadPnl.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Hide();
    }
}
