using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
//using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// Handling all controller button press
/// Spawning building blocks
/// Changing color
/// Choosing GameObject
/// 
/// Handling Layers UI
/// Calling layer list
/// Calling Loading Layer
/// </summary>
/// 

public class Build : PressINputBase
{
    public Transform origin;
    public Transform buildingBlock;
    public bool isInBuildMode = false; 
    public GameObject savedContent;
    public GameObject savedUI;
    public GameObject listUI;
    public Button listItem;
    LayerLoader loaderSc;
    NotesManager notesManager;
    ContactService contactService;
    authManager authMSc;
    RaycastHit intHit;
    bool isPressed;
    public XROrigin arSessionOrigin;
    public Camera arCamera;

    [HideInInspector]
    public GameObject selectedGo;
    
    public Vector3 coordinateSystemPos;
    public bool isCoordinateSystemSet = false;


    protected override void Awake()
    {
        base.Awake();
        

    }

    void Start()
    {
        arCamera = arSessionOrigin.Camera;
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        //xRManager= GetComponent<XRInteractionManager>();
        authMSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();
      
        


    }

    // Update is called once per frame
    void Update()
    {
        if (!isCoordinateSystemSet&& GameObject.Find("CoordinateSystem"))
        { 
            coordinateSystemPos = GameObject.Find("CoordinateSystem").transform.position; 
            isCoordinateSystemSet=true;
        }
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Touch touch = Input.GetTouch(0);

            // Only process the touch when it begins
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                var touchPosition = touch.position;

                if (isInBuildMode && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    BuildBlocks();

                    Debug.Log("Sensed Something In Build"); //it should never get here in android
                }
                else if (!isInBuildMode && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Ray ray = arCamera.ScreenPointToRay(touchPosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                    {
                        Debug.Log("Touch sensed");
                        contactService.commCube.GetComponent<Image>().color = Color.blue; // blue if hit
                        GameObject hitObject = hit.transform.gameObject;
                        if (selectedGo != hitObject)
                        {
                            if (selectedGo != null)
                            {
                                selectedGo.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                                selectedGo.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);
                            }
                            selectedGo = hitObject;
                            selectedGo.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.cyan * 0.4f);
                            if (notesManager.gOname)
                                notesManager.gOname.GetComponentInChildren<TMP_Text>().text = selectedGo.name;
                            notesManager.gOpos.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.localPosition.ToString();
                        }

                        if (loaderSc.isInColorMode)
                        {
                            Debug.Log("In color Mode");
                            Changes changesComponent = selectedGo.GetComponentInChildren<Changes>();
                            if (changesComponent != null)
                            {
                                changesComponent.ChangeColor();
                            }
                            else
                            {
                                if (selectedGo.GetComponent<Renderer>())
                                    selectedGo.AddComponent<Changes>().ChangeColor();
                                //else if (selectedGo.GetComponentInChildren<Renderer>())
                                   // selectedGo.transform.GetChild(1).AddComponent<Changes>().ChangeColor();
                            }
                        }

                        contactService.commCube.GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }

    }


    protected override void OnPress(Vector3 position) => isPressed = true;

    protected override void OnPressCancel() => isPressed = false;
    public void OnLayerListDis()
    {
        for (var i = savedContent.transform.childCount - 1; i >= 0; i--)
        {
            if(savedContent.transform.GetChild(i).gameObject.GetComponent<Image>().color==Color.white)
            Destroy(savedContent.transform.GetChild(i).gameObject);
        }
    }


    public void Clear()
    {
        string layerName = loaderSc.layerTitleText.text;
        if (DoesTagExist(layerName))
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName);
            foreach (var block in blocks)
            {
                Destroy(block.gameObject);
            }
        }
    }


    public void OnLayerEn()
    {
        GetLayerListByName();
    }

    public void GetLayerListByName()
    {
        StartCoroutine(contactService.GetRequest("http://"+authMSc.ipAddress+":3000/projects/byname/Demo"));///+projectNameInput.text));
        
    }

    public void ToConsole(List<Project> layers)
    {
        
        foreach (var layer in layers)
        {
            if (layer.model.Contains("objectType"))
            {
                Debug.Log("Layers: " + layer.layername);
                var nN = Instantiate(listItem, savedContent.transform);
                nN.transform.SetSiblingIndex(0);
                nN.transform.GetComponentInChildren<TMP_Text>().text = layer.layername + " " + layer.start;
                nN.gameObject.name = layer.model;
                nN.gameObject.AddComponent<LoadLayer>().data = layer.model;

                nN.gameObject.GetComponent<LoadLayer>().data2 = layer.layername;

                nN.gameObject.GetComponent<LoadLayer>().btn = nN;
                nN.onClick.AddListener(() => nN.gameObject.GetComponent<LoadLayer>().Loading());
            }
        }
    }


    public void LoadingLayer(string layerName, string layerModel, Button button)
    {
            Debug.Log("LayerName: "+layerName+" Model: "+layerModel+" Button: "+button.name);
        if (button.transform.GetComponent<Image>().color==Color.white)
        {
        button.transform.GetComponent<Image>().color = Color.green;
            //other options for tag replacement
            //create tag
            //or own dictionary?
            //create basic tags
            
            if (DoesTagExist(layerName))
            {
                Debug.Log("IN TAG");
                var loadedObjects=loaderSc.LayerJsonToLayerBegin(layerName, layerModel);
                button.onClick.AddListener(() => { UnloadLayer(loadedObjects); });
            }

        }
        else
        {
            button.transform.GetComponent<Image>().color = Color.white;
           
           
        }
    }

    public void UnloadLayer(List<GameObject> objects)
    {
        foreach(var obj in objects)
        {
            Destroy(obj.gameObject);
        }
    }

    public static bool DoesTagExist(string aTag)
    {
        try
        {
            GameObject.FindGameObjectsWithTag(aTag);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void BuildModeToggle(Button button)
    {
        isInBuildMode = !isInBuildMode;
        if (isInBuildMode)
        {
            button.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.6f);
        }
        else
            button.GetComponent<Image>().color = Color.white;

    }
    public void BuildBlocks()
    {
      
        if (isInBuildMode)
        {
            
           // if (interactor.TryGetCurrent3DRaycastHit(out intHit))
            {
                
               var cube= Instantiate(buildingBlock, intHit.point, Quaternion.identity,loaderSc.userParentObject.transform);
                cube.tag="Cube";
            }
        }
      
    }

  

    public string GetProjectNameByID(string id)
    {
        StartCoroutine(contactService.GetRequest("http://"+authMSc.ipAddress+":3000/projects/:" + id));///+projectNameInput.text));


        return id;
    }

    public string IDtoName(string json)
    { 
        var project = JsonConvert.DeserializeObject<Project>(json);
        return project.name; 
    }

    
}
