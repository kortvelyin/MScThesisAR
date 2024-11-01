using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingbehaviours : MonoBehaviour
{
    public float step;
    public GameObject objectToPlace;
    public GameObject objectOne;


    public void PlaceObjectBtwn()
    {
        
        objectToPlace.transform.position = Vector3.MoveTowards(objectOne.transform.position, Camera.main.transform.position, step);
    }
}
