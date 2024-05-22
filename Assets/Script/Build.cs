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

         if (Pointer.current==null||isPressed==false)
            {
                return;
            }

         var touchPosition = Pointer.current.position.ReadValue();
        
        if (isInBuildMode && !EventSystem.current.IsPointerOverGameObject())
        {
            BuildBlocks();

           
                Debug.Log("Sensed Something 0");
               // var hitPose = m_Hits[0].pose;
                //m_Hits[0].trackable.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                /*if(spawnedObject==null)
                {
                    spawnedObject = Instantiate(spawnedPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    spawnedObject.transform.position = hitPose.position;
                    spawnedObject.transform.rotation=hitPose.rotation;
                }*/
            
           
            
        } 
        else if (!isInBuildMode )//&& interactor.TryGetCurrent3DRaycastHit(out intHit))
        {
        
                Ray ray = arCamera.ScreenPointToRay(touchPosition);
                if (Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity) && !EventSystem.current.IsPointerOverGameObject())
                {

                    contactService.commCube.GetComponent<Image>().color = Color.blue;                // blue if hit
                        if (selectedGo != hit.transform.gameObject)
                        {
                            if (selectedGo != null)
                            {
                        selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f); }
                            selectedGo = hit.transform.gameObject;
                            selectedGo.transform.gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.cyan * 0.4f);
                            if (notesManager.gOname)
                                notesManager.gOname.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.gameObject.name;
                            notesManager.gOpos.GetComponentInChildren<TMP_Text>().text = (selectedGo.transform.localPosition).ToString();
                        }
                        else
                        {
                            if (loaderSc.isInColorMode)
                            {
                                if (selectedGo.GetComponent<Changes>())
                                {
                                    selectedGo.GetComponent<Changes>().ChangeColor();
                                }
                                else
                                {
                                    selectedGo.AddComponent<Changes>().ChangeColor();
                                }
                            }
                        }

                    contactService.commCube.GetComponent<Image>().color = Color.white;
                }
                
            
        }
    }


    protected override void OnPress(Vector3 position) => isPressed = true;

    protected override void OnPressCancel() => isPressed = false;
    public void OnLayerListDis()
    {
        for (var i = savedContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(savedContent.transform.GetChild(i).gameObject);
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
           
            Debug.Log("Layers: "+layer.layername);
            var nN = Instantiate(listItem, savedContent.transform);
           nN.transform.SetSiblingIndex(0);
            nN.transform.GetComponentInChildren<TMP_Text>().text = layer.layername+" " +layer.start;
            nN.gameObject.name = layer.model;
            nN.gameObject.AddComponent<LoadLayer>().data = layer.model;
            
            nN.gameObject.GetComponent<LoadLayer>().data2 = layer.layername;
            
            nN.gameObject.GetComponent<LoadLayer>().btn = nN;
            nN.onClick.AddListener(() => nN.gameObject.GetComponent<LoadLayer>().Loading());
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
                loaderSc.LayerJsonToLayerBegin(layerName, layerModel);
                
            }
        }
        else
        {
            button.transform.GetComponent<Image>().color = Color.white;
            //check for tag if tag doesnt exist, dont bother
            if (DoesTagExist(layerName))
            {
                GameObject[] blocks = GameObject.FindGameObjectsWithTag(layerName);
                foreach (var block in blocks)
                {
                    Destroy(block.gameObject);
                }
            }
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
