using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Changes on the Building parts, right now only the colors can be changed
/// Called: LayerLoader.cs in LayerInfoToLayer()
/// </summary>
public class Changes : MonoBehaviour
{
    //Original data
    [HideInInspector]
    public Color ogColor;
    [HideInInspector]
    public Material ogMaterial;
    [HideInInspector]
    public Transform ogTransform;
    Color changeA;

    // Start is called before the first frame update
    void Start()
    {
        ogColor=gameObject.GetComponent<Renderer>().material.color;
        ogMaterial = gameObject.GetComponent<Material>();
        ogTransform = gameObject.transform;
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
