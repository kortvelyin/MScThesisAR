using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityTracker : MonoBehaviour
{
    public string onDisable;
    public string onEnable;
    public float when=0;

    public Build buildingscript;
    void OnDisable()
    {
        Invoke(onDisable, when);
    }

    void OnEnable()
    {
        Invoke(onEnable, when);
    }

    void layerDis()
    {
        buildingscript.OnLayerListDis();
    }

    void layerEn()
    {
        buildingscript.OnLayerEn();
    }
}
