using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Changes on the Building parts, right now only the colors can be changed
/// Called: LayerLoader.cs in LayerInfoToLayer()
/// </summary>
public class Changes : MonoBehaviour
{
    //Original data
    //[HideInInspector]
    public Color ogColor;
    //[HideInInspector]
    public Material ogMaterial;
    //[HideInInspector]
    public Transform ogTransform;
    public Color gotColor;


    public void StartChanges()
    {
        //Debug.Log("Changes script started");
        ogMaterial = gameObject.GetComponentInChildren<Renderer>().material;
        ogColor = ogMaterial.color;
        gameObject.GetComponentInChildren<Renderer>().material.color = gotColor;
        ogTransform = gameObject.transform;
        //Debug.Log("Changes object name: " + gameObject.name + " .og color" + GetComponent<Changes>().ogColor.ToString());
    }



   public void ChangeColor()
    {
        if(gameObject.GetComponentInChildren<Renderer>().material.color == ogColor)
        {
            gameObject.GetComponentInChildren<Renderer>().material.color = Color.green;
        }
        else if (gameObject.GetComponentInChildren<Renderer>().material.color == Color.green)
        {
            gameObject.GetComponentInChildren<Renderer>().material.color = Color.yellow;
        }
        else if (gameObject.GetComponentInChildren<Renderer>().material.color == Color.yellow)
        {
            gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        }
        else if (gameObject.GetComponentInChildren<Renderer>().material.color == Color.red)
        {
            gameObject.GetComponentInChildren<Renderer>().material.color = ogColor;

        }
       

    }
}
