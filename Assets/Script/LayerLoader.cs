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


    public void LayerJsonToLayerBegin(string layerName, string layer)
    {
        if(GameObject.Find("CoordinateSystem"))
        Debug.Log("layer: "+layer);
        var parentObject = new GameObject(layerName);

        userParentObject.transform.parent = GameObject.Find("CoordinateSystem").transform;
        userParentObject.transform.localPosition = new Vector3(0, 0, 0);
        userParentObject.transform.localRotation = Quaternion.EulerRotation(0, 0, 0);

        parentObject.transform.parent = userParentObject.transform;
        parentObject.transform.localPosition= new Vector3(0, 0, 0);
        parentObject.transform.localRotation= Quaternion.EulerRotation(0, 0, 0);
        if (layer.Contains("layeritem"))
        {
            Debug.Log("it is indeed full of layeritems");
            
            
            LayerInfoToLayer(layer, parentObject, layerName);
        }
        else if(layer.Contains("Item"))//with jsonHelper
        {
            Debug.Log("it is indeed full of items");
            

            LayerInfoToLayer(layer, parentObject, layerName);
        }
        else if (layer.Contains("htt"))
        {
            StartCoroutine(GetAssetBundle(parentObject,layer));
        }
        else
        {
            Debug.Log("Couldn't find layer type");
        }

    }

    public void LayerInfoToLayer(string model, GameObject parentObject, string layerName)
    {
        var layerItemList = JsonHelper.FromJson<string>(model);
       
        GameObject item = null;
        foreach (var layerItem in layerItemList)
        {
            LayerItem lItem = JsonUtility.FromJson<LayerItem>(layerItem);
           // Debug.Log(" item type: " + layerItem);
            for (int i = 0; i < prefabs.Count; i++)
            {
                //Debug.Log(" prefabs[i].name: " + prefabs[i].name);
                if (lItem.objectType.Contains(prefabs[i].name))
                {
                       // Debug.Log("contains prefabs[i].name: " + prefabs[i].name);
                        item = Instantiate(prefabs[i]);
                    item.tag = layerName;
                    Debug.Log("item.tag: " + item.tag);
                    item.name = lItem.objectType;
                    item.transform.parent = parentObject.transform;
                    var transfromArray= JsonHelper.FromJson<String>(lItem.transform);

                    item.transform.localPosition = JsonUtility.FromJson<Vector3>(transfromArray[0]);
                    //Debug.Log("pos: " + item.transform.position);
                    item.transform.localRotation = JsonUtility.FromJson<Quaternion>(transfromArray[1]);
                    //Debug.Log("rot: " + item.transform.rotation);
                    item.transform.localScale = JsonUtility.FromJson<Vector3>(transfromArray[2]);
                   // Debug.Log("scale: " + item.transform.lossyScale);
                    item.AddComponent<Changes>().ogMaterial = item.GetComponent<Renderer>().material;
                    item.GetComponentInChildren<Renderer>().material.color = lItem.color;
                    if (lItem.color == Color.white)
                    {
                        Color changeA = item.GetComponentInChildren<Renderer>().material.color;
                        changeA.a = 0.4f;
                        item.GetComponentInChildren<Renderer>().material.color= changeA;
                    }
                }
            }
            
        }

  if(item == null) 
            {
               
                contactService.commCube.GetComponent<Renderer>().material.color = Color.red;
            Debug.Log("couldnt find match in layer recreation ");

        }

       
    }


    public string TransformStringFromData(Transform transform)
    {
        string[] transformArray =new string[3];
        transformArray[0]= JsonUtility.ToJson(transform.localPosition);
        transformArray[1] = JsonUtility.ToJson(transform.rotation);
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
        //List<string> upBlocks = new List<string>();//(new string[blocks.Length]);//new LayerItem[blocks.Length];
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
            //Debug.Log("layeriteminpost: " + upBlocks[i]);
        }
        
        var postStrinsArr=JsonHelper.ToJson(upBlocks);
        
        Debug.Log("string jlist2: " + postStrinsArr);
        
            return postStrinsArr;
    }

    public void LayerToServer(string layerName = "demo")
    {
        layerName = layerTitleText.text;//"Arnold A.";//authMSc.userData.name;
        var doneModelArray = SaveBlocks(layerName);
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

   
