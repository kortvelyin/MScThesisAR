using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLayer : MonoBehaviour
{
    /// <summary>
    /// Attqached to the button of the UI list to load data
    /// </summary>

    public string data;
    public string data2;
    public Button btn;

    public void Loading()
    {
        GameObject.Find("Building").GetComponent<Build>().LoadingLayer(data2,data, transform.gameObject.GetComponent<Button>());
    }
   
}
