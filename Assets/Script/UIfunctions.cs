using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Newtonsoft.Json;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIfunctions : MonoBehaviour
{
    bool toggle=true;
    private TMP_InputField inField;

  
    public void ToggleGO(GameObject go)
    {

        go.SetActive(!go.activeSelf);
    }

    public void SwitchOff(GameObject go)
    {
        if(go.activeSelf)
        {
            go.SetActive(false);
        }
        
    }

    public void ToggleColor(UnityEngine.UI.Button button)
    {
        if (toggle)
        {
            button.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            toggle = !toggle;
        }
        else
        {
            button.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
            toggle = !toggle;
        }
    }

 
}
