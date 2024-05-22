using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Newtonsoft.Json;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using UnityEngine.XR.Interaction.Toolkit;
//using Microsoft.MixedReality.Toolkit.Experimental.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class UIfunctions : MonoBehaviour
{
    //public GameObject gObject;
    bool toggle=true;
  //  NonNativeKeyboard keyboardSC;
    private TMP_InputField inField;

    private void Start()
    {
        // keyboardSC= GameObject.Find("NonNativeKeyboard").GetComponent<NonNativeKeyboard>();
      /*  if(GetComponent<TMP_InputField>())
        {
        inField = GetComponent<TMP_InputField>();
        inField.onSelect.AddListener(x=>OpenKeyboard());
        }*/
        
    }
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

 /*   public void SwitchInput()
    {
        keyboardSC = GameObject.Find("NonNativeKeyboard").GetComponent<NonNativeKeyboard>();
        keyboardSC.InputField = this.gameObject.GetComponent<TMP_InputField>();
    }


    public void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inField;
        NonNativeKeyboard.Instance.PresentKeyboard(inField.text);
    }*/
}
