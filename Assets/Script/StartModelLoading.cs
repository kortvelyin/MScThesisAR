using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartModelLoading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Models").GetComponent<Button>().enabled=true;
       /* var objects=Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (var gO in  objects)
        {
            print(gO.name+" pos: "+gO.GetComponent<GameObject>().transform.position.ToString()+" localPos: "+ gO.GetComponent<GameObject>().transform.localPosition.ToString());
        }*/
    }

}
