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
/// Clearing LayerList
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
    public int openModelCount=0;

    
    public GameObject selectedGo;
    
    public Vector3 coordinateSystemPos;
    public bool isCoordinateSystemSet = false;
    public Color loadedButton;
    public TMP_Text changeText;

    /// <summary>
    /// Handling all controller button press
    /// Press funtions for touch
    ///      /// Changing color
    ///      ///Choosing GameObject
    /// Handling Layers UI
    /// Calling layer list
    /// Calling Loading Layer
    /// GetProjectNameByID
    /// IDtoName
    /// IsPointerOverGameObject()
    /// </summary>

    protected override void Awake()
    {
        base.Awake();
        

    }

    void Start()
    {
        arCamera = arSessionOrigin.Camera;
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
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
        if (Input.touchCount > 0) //&& !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Touch touch = Input.GetTouch(0);

            // Check if touch is over a UI element first
            if (IsPointerOverGameObject())//EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("Touch on UI, ignoring interaction with AR objects.");
                return;
            }
            // Only process the touch when it begins
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                var touchPosition = touch.position;

                if (isInBuildMode)// && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    BuildBlocks();

                    Debug.Log("Sensed Something In Build"); //it should never get here in android
                }
                else// if (!isInBuildMode && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    Ray ray = arCamera.ScreenPointToRay(touchPosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                    {
                        Debug.Log("Touch sensed");
                        contactService.commCube.GetComponent<Image>().color = Color.white; // blue if hit
                        GameObject hitObject = hit.transform.gameObject;
                        if (selectedGo != hitObject)
                        {
                            if (selectedGo != null)
                            {
                                Debug.Log("emission out");
                                selectedGo.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                                selectedGo.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * 0.0f);
                                
                            }
                            selectedGo = hitObject;
                            selectedGo.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.cyan * 0.4f);
                            
                            Debug.Log("emission on");
                            if (notesManager.gOname)
                                notesManager.gOname.GetComponentInChildren<TMP_Text>().text = selectedGo.name;
                            notesManager.gOpos.GetComponentInChildren<TMP_Text>().text = selectedGo.transform.localPosition.ToString();
                        }

                        if (loaderSc.isInColorMode)
                        {
                            Debug.Log("In color Mode");
                            Changes changesComponent = selectedGo.GetComponentInChildren<Changes>();
                            if (changesComponent!=null)
                            {
                                Debug.Log("had changes onit");
                                changesComponent.ChangeColor();
                                
                            }
                         
                        }

                        
                    }
                }
            }
        }

    }


    protected override void OnPress(Vector3 position) => isPressed = true;

    protected override void OnPressCancel() => isPressed = false;
    public void OnLayerListDis()
    {
        changeText.text = "Layer List";
        for (var i = savedContent.transform.childCount - 1; i >= 0; i--)
        {
            if(savedContent.transform.GetChild(i).gameObject.GetComponent<Image>().color != Color.green)
            Destroy(savedContent.transform.GetChild(i).gameObject);
        }
    }

    public void OnLayerEn()
    {
        changeText.text = "Open Layers";
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
                var nN = Instantiate(listItem, savedContent.transform);
                nN.transform.SetSiblingIndex(openModelCount);
                nN.transform.GetComponentInChildren<TMP_Text>().text = layer.layername + " " + layer.start;
                nN.gameObject.name = layer.model;
                nN.gameObject.AddComponent<LoadLayer>().data = layer.model;
                nN.gameObject.GetComponent<LoadLayer>().data2 = layer.layername;
                nN.gameObject.GetComponent<LoadLayer>().btn = nN;
                nN.onClick.AddListener(() => nN.gameObject.GetComponent<LoadLayer>().Loading());

                for (int i = 0; i < openModelCount; i++)
                {
                    if (savedContent.transform.GetChild(i).gameObject.GetComponentInChildren<TMP_Text>().text == nN.transform.GetComponentInChildren<TMP_Text>().text)
                    {
                        nN.transform.GetComponentInChildren<Image>().color = Color.gray;
                    }
                }
            }
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
    public void LoadingLayer(string layerName, string layerModel, Button button)
    {
        Debug.Log("button color: " + button.transform.GetComponent<Image>().color);
        if (button.transform.GetComponent<Image>().color==Color.white || button.transform.GetComponent<Image>().color == loadedButton)
        {
            button.transform.GetComponent<Image>().color = Color.green;
            if (DoesTagExist(layerName))
            {
                var parentObject=loaderSc.LayerJsonToLayerBegin(layerName, layerModel);
                openModelCount++;
                button.GetComponent<LoadLayer>().loadedParent = parentObject;
                Debug.Log("addlistener was added: " + button.name+" count of objects: "+parentObject.transform.childCount );
            }

        }
        else if (button.transform.GetComponent<Image>().color == Color.green)
        {
            button.transform.GetComponent<Image>().color = loadedButton;
            openModelCount--;
            Debug.Log("UnLoading was called");
            if(button.GetComponent<LoadLayer>().loadedParent)
            {
                 foreach (var child in button.GetComponent<LoadLayer>().loadedParent.transform.GetComponentsInChildren<Transform>())
                    {
                        Destroy(child.gameObject);
                    }
                        Destroy(button.GetComponent<LoadLayer>().loadedParent);
            }
           
        }
    }

    /*
    public void UnLoad()
    {
        Debug.Log("UnLoad was called");
        GameObject.Find("Building").GetComponent<Build>().openModelCount--;
        foreach (var child in loadedParent.transform.GetComponentsInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
        Destroy(loadedParent);
        Debug.Log("data2: " + data2 + " data: " + data);
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => this.gameObject.GetComponent<LoadLayer>().Loading());
    }
    */


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
      /*
        if (isInBuildMode)
        {
            
           // if (interactor.TryGetCurrent3DRaycastHit(out intHit))
            {
                
               var cube= Instantiate(buildingBlock, intHit.point, Quaternion.identity,loaderSc.userParentObject.transform);
                cube.tag="Cube";
            }
        }
      */
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

    public static bool IsPointerOverGameObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        //check touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;
        }

        return false;
    }
}
