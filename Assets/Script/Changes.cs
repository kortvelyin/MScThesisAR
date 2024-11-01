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
        ogMaterial = gameObject.GetComponent<Renderer>().material;
        ogColor = ogMaterial.color;
        gameObject.GetComponent<Renderer>().material.color = gotColor;
        ogTransform = gameObject.transform;
        //Debug.Log("Changes object name: " + gameObject.name + " .og color" + GetComponent<Changes>().ogColor.ToString());
    }



   public void ChangeColor()
    {
        if(gameObject.GetComponent<Renderer>().material.color == ogColor)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.green)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.yellow)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (gameObject.GetComponent<Renderer>().material.color == Color.red)
        {
            gameObject.GetComponent<Renderer>().material.color = ogColor;

        }
       

    }
}
