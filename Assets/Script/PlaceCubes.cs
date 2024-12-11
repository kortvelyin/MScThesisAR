using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCubes : MonoBehaviour
{
    public string[] transforms;
    private Vector3 pos;
    private Vector3 rot;
    private Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(Placing(transforms));
    }


    public IEnumerator Placing(string[] transfromArray)
    {
        yield return new WaitForSeconds(0.1f);
        transform.localPosition = JsonUtility.FromJson<Vector3>(transfromArray[0]);
        pos= transform.localPosition;
        transform.localRotation = JsonUtility.FromJson<Quaternion>(transfromArray[1]);
        rot = transform.localEulerAngles;
        transform.localScale = JsonUtility.FromJson<Vector3>(transfromArray[2]);
        scale = transform.localScale;
    }

    private void Update()
    {
        if(transform.localPosition!= pos)
        {
            transform.localPosition = pos;
            transform.localEulerAngles = rot;
            transform.localScale = scale;
        }
    }
}
