using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallphasethrough : MonoBehaviour
{

    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).GetComponent<MeshCollider>().convex = true;
            parent.transform.GetChild(i).GetComponent<MeshCollider>().isTrigger = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
