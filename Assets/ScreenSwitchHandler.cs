using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ScreenSwitchHandler : MonoBehaviour
{
    [SerializeField] ScreenComp[] screenComps;
    private void Start()
    {
        foreach (var item in screenComps)
        {
            item.button.onClick.AddListener(() => item.ShowScreen(true, HideScreens));
        }
    }
    void HideScreens()
    {
        foreach (var item in screenComps)
        {
            item.ShowScreen(false);
        }
    }
}
[Serializable]
public class ScreenComp
{
    public GameObject panel;
    public Button button;
    public void ShowScreen(bool show, Action iniAction = null)
    {
        iniAction?.Invoke();
        if(panel != null)
        panel.SetActive(show);
        button.interactable = !show;
    }
}
