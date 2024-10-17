using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking;
//using static UnityEditor.Experimental.GraphView.GraphView;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

using Newtonsoft.Json.Serialization;
using Unity.VisualScripting;

[Serializable]
public class LayerItem
{
    public string objectType="";
    public string name="";
    public string transform=null;
    public Color color=new Color();
}
public class LayerLoader : MonoBehaviour
{


    public List<GameObject> prefabs;
    public GameObject userParentObject;
    ContactService contactService;
    authManager authMSc;
    Build buildSc;
    public bool isInColorMode;
    public TMP_Text layerTitleText;

    //url is a different thing, its an assetbundle, and a url instead of LayerItem

    private void Start()
    {
        buildSc = GameObject.Find("Building").GetComponent<Build>();
        
        userParentObject = new GameObject();
        userParentObject.transform.position = new Vector3(0, 0, 0);
        userParentObject.transform.rotation= Quaternion.EulerRotation(0, 0,0);
        userParentObject.name = "userID";
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
       // LayerToServer("showroom");

    }

    ////listprojects, string on gO, klick and turn that to blocks
    ///list projects
    //klicks projects, get item, call LayerJsonToLayerBegin(string layerName, string layer)


    public List<GameObject> LayerJsonToLayerBegin(string layerName, string layer)
    {
        if(GameObject.Find("CoordinateSystem"))
        Debug.Log("layer: "+layer);
        var parentObject = new GameObject(layerName);
        List<GameObject> loadedObjects = new List<GameObject>();


        // coordinate reset for the showroom to be in the right place
        userParentObject.transform.parent = GameObject.Find("CoordinateSystem").transform;
        userParentObject.transform.localPosition = new Vector3(0, 0, 0);
        userParentObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        parentObject.transform.parent = userParentObject.transform;
        parentObject.transform.localPosition= new Vector3(0, 0, 0);
        parentObject.transform.localRotation= Quaternion.Euler(0, 0, 0);
        if (layer.Contains("layeritem"))
        {
            Debug.Log("it is indeed full of layeritems");
            
            
            LayerInfoToLayer(layer, parentObject, layerName);
        }
        else if(layer.Contains("Item"))//with jsonHelper
        {
            Debug.Log("it is indeed full of items");

            if (layer.Contains("objectType"))
                loadedObjects=LayerInfoToLayer(layer, parentObject, layerName);
        }
        else if (layer.Contains("htt"))
        {
            StartCoroutine(GetAssetBundle(parentObject,layer));
        }
        else
        {
            Debug.Log("Couldn't find layer type");
        }
        return loadedObjects;

    }

    public List<GameObject> LayerInfoToLayer(string model, GameObject parentObject, string layerName)
    {
        List<GameObject> loadedObjects = new List<GameObject>();
        var layerItemList = JsonHelper.FromJson<string>(model);
       
        GameObject item = null;
        foreach (var layerItem in layerItemList)
        {
            LayerItem lItem = JsonUtility.FromJson<LayerItem>(layerItem);
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (lItem.objectType.Contains(prefabs[i].name))
                {
                    item = Instantiate(prefabs[i]);
                    loadedObjects.Add(item);
                    parentObject.tag = layerName; //so that this gets destroyed too
                    item.tag = layerName;
                    item.name = lItem.objectType;
                    item.transform.parent = parentObject.transform;
                    var transfromArray= JsonHelper.FromJson<String>(lItem.transform);

                    item.transform.localPosition = JsonUtility.FromJson<Vector3>(transfromArray[0]);
                    //Debug.Log(item.gameObject.GetInstanceID().ToString()+item.name + item.transform.localPosition.ToString());
                    item.transform.localRotation = JsonUtility.FromJson<Quaternion>(transfromArray[1]);
                    item.transform.localScale = JsonUtility.FromJson<Vector3>(transfromArray[2]);
                    if(prefabs[i].name=="Cube")
                    {
                        Debug.Log("I WAS CALLED");
                        item.AddComponent<PlaceCubes>().transforms=transfromArray;
                    }
                    //Debug.Log("Pos: " + transfromArray[0] + "got Pos: " + item.transform.localPosition.ToString() + " Rot: " + transfromArray[1] + " got Rot: " + item.transform.localRotation.ToString());
                    if (item.GetComponent<Renderer>())
                        item.AddComponent<Changes>().ogMaterial = item.GetComponent<Renderer>().material;
                    /*else if (item.GetComponentInChildren<Renderer>())
                    {
                        item.transform.GetChild(1).AddComponent<Changes>().ogMaterial = item.transform.GetChild(1).GetComponent<Renderer>().material;
                    }*/
                    item.GetComponentInChildren<Renderer>().material.color = lItem.color;
                    //Debug.Log(item.gameObject.GetInstanceID().ToString() + item.name + item.transform.localPosition.ToString());

                }
            }
        }

            if(item == null) 
            {
                contactService.commCube.GetComponent<Image>().color = Color.red;
            Debug.Log("Item was null, position: "+item.transform.localPosition.ToString());
                Debug.Log("couldnt find match in layer recreation ");
             }

        return loadedObjects;
    }


    public string TransformStringFromData(Transform transform)
    {
        string[] transformArray =new string[3];
        transformArray[0]= JsonUtility.ToJson(transform.localPosition);
        transformArray[1] = JsonUtility.ToJson(transform.localRotation);
        transformArray[2] = JsonUtility.ToJson(transform.lossyScale);

        return JsonHelper.ToJson(transformArray);
    }
    IEnumerator GetAssetBundle(GameObject parent, string layer)
    {
        var layerData = JsonConvert.DeserializeObject<Project>(layer);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(layerData.model);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            var loadAsset = bundle.LoadAllAssets();
            yield return loadAsset;

            foreach (var asset in loadAsset)
            {
                Instantiate(asset, parent.transform);
            }

        }
    }

   



    public string SaveBlocks(string layerName = "none")
    {
        //layerName = authMSc.userData.name;
        Debug.Log("LayerName: " + layerName);
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName); 
        if (blocks.Length == 0)
            return 0.ToString();
        string[] upBlocks = new string[blocks.Length];
        //int i = 0;
        
        for (int i = 0; i < blocks.Length; i++)
        {
            var postLayerI = new LayerItem();

            postLayerI.name = layerName; //tag?
            postLayerI.objectType = blocks[i].name.Replace('/', '_');
            
            postLayerI.transform = TransformStringFromData(blocks[i].transform);
            postLayerI.color = blocks[i].GetComponentInChildren<Renderer>().material.color;

            Destroy(blocks[i].gameObject);
            upBlocks[i]= JsonUtility.ToJson(postLayerI);
        }
        
        var postStrinsArr=JsonHelper.ToJson(upBlocks);
        
        Debug.Log("string jlist2: " + postStrinsArr);
        
            return postStrinsArr;
    }

    public void LayerToServer(string layerName = "demo")
    {
        layerName = layerTitleText.text;//"Arnold A.";//authMSc.userData.name;
        var doneModelArray = SaveBlocks(layerName);
        if (doneModelArray == 0.ToString())
            return;
        Debug.Log("doneModelArray: " + doneModelArray);
        
        //var jlayer = JsonUtility.ToJson(doneModelArray);
        var upProjectItem = new Project
        {
            name = "Demo",
            start = System.DateTime.Today.ToString(),
            finish = "2024.07.04.",
            layername = layerName,
            model = doneModelArray
        };

        var jProjectItem = JsonUtility.ToJson(upProjectItem);
        Debug.Log("Posting ProjectItem+ " + jProjectItem);
        StartCoroutine(contactService.PostData_Coroutine(jProjectItem, "http://"+authMSc.ipAddress+":3000/projects"));
    }


    public void ObjectColorModeToggle(Button button)
    {
        isInColorMode = !isInColorMode;
        if (isInColorMode)
        {
            button.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.6f);
        }
        else
            button.GetComponent<Image>().color = Color.white;

    }

}

   
