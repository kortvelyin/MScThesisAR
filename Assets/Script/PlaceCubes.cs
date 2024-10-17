using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCubes : MonoBehaviour
{
    public string[] transforms;
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(Placing(transforms));
    }


    public IEnumerator Placing(string[] transfromArray)
    {
        yield return new WaitForSeconds(0.1f);
        transform.localPosition = JsonUtility.FromJson<Vector3>(transfromArray[0]);
        transform.localRotation = JsonUtility.FromJson<Quaternion>(transfromArray[1]);
        transform.localScale = JsonUtility.FromJson<Vector3>(transfromArray[2]);
    }
}
