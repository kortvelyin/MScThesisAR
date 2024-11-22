using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpNoteRotation : MonoBehaviour
{

    //x=y and z=-4.768373e-07
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.LookAt(Camera.main.transform.position);
    }
}
