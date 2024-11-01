using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;


//using UnityEngine.UIElements;
/// <summary>
/// Handling the Note creation, listing, communication with DB
/// Everything that has to do with the notes. 
/// Notes class--> Notes.cs
/// NotesRoot class -->NotesRoot.cs (this is for the api list call)  
/// </summary>
public struct TableTypes
{
    public string urlContains;
    public string type;
}
public class NotesManager : MonoBehaviour
{
    public GameObject headCamera;
    public GameObject newNote; //UI
    public GameObject savedNotes; //UI
    public float step=1;
    public Button savedNotePrefab; //UI listitem
    public GameObject savedContext;//UI list content
    public GameObject oneContext;//UI simple note view data content
    public TableTypes[] tableTypes;
    public Button saveB; //UI savebutton
    public Button colorGO;//coloring on/off switch
    [HideInInspector]
    public Button gOname; //selected gameobject UI
    public Button gOpos;//note pos based on selected go UI
    public Button gOuser;//note creator based on selected go UI
    public Button layerNote; //Note pop ups 
    
    public TMP_InputField textNote; //UI text
    public TMP_InputField titleNote;//UI text
    private string baseUser_id;
    private TouchScreenKeyboard keyboard;
    public GameObject parentObject;
    Button lN;
    //scripts
    LoginScreenUI loginScreenUI;
    authManager authSc;
    ContactService contactService;
    Build buildSc;
    LayerLoader loaderSc;


    // Start is called before the first frame update
    void Start()
    {
       
        loginScreenUI = GameObject.Find("LoginScreenUI").GetComponent<LoginScreenUI>();
        authSc = GameObject.Find("AuthManager").GetComponent<authManager>();
        contactService = GameObject.Find("AuthManager").GetComponent<ContactService>();
        buildSc = GameObject.Find("Building").GetComponent<Build>();
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();

        gOuser = Instantiate(savedNotePrefab, oneContext.transform);
        gOuser.transform.GetComponentInChildren<TMP_Text>().text = authSc.userData.name;
        gOname = Instantiate(savedNotePrefab, oneContext.transform);
        gOpos = Instantiate(savedNotePrefab, oneContext.transform);
        parentObject = new GameObject("layerNote");
        //loaderSc.userParentObject.transform.parent = GameObject.Find("CoordinateSystem").transform;
        //parentObject.transform.parent = loaderSc.userParentObject.transform;
    }

   

     
    public void ToConsole(List<Notes> notes) //list out notes
    {
        GameObject[] noteLayer = GameObject.FindGameObjectsWithTag("Note");
        foreach (var noteItem in noteLayer)
        {
            Destroy(noteItem.gameObject);
        }
        Debug.Log("Notes: " + notes.ToString());
        foreach(var note in notes)
        {
           
            var nN = Instantiate(savedNotePrefab, savedContext.transform);
            nN.transform.SetSiblingIndex(0);
            nN.transform.GetComponentInChildren<TMP_Text>().text = authSc.GetUserNameByID(Int32.Parse(note.user_id)) + " " + note.title;
            nN.onClick.AddListener(() => ShowNote(JsonUtility.ToJson(note)));

            //The pop ups of the notes as a layer throughout the bulding
            if (GameObject.Find("CoordinateSystem"))
            {
                var trans = JsonHelper.FromJson<string>(note.position);
                
                if (JsonConvert.DeserializeObject<Vector3>(trans[0])!=Vector3.zero &&note.gobject!="Select Object" && note.gobject!= "ARTrackedImage 00000000000BF9A0-0000000000000000")
                {
                    Debug.Log("the vector3: " + JsonConvert.DeserializeObject<Vector3>(trans[0]));
                    
                    loaderSc.userParentObject.transform.parent = GameObject.Find("CoordinateSystem").transform;

                    parentObject.transform.parent = loaderSc.userParentObject.transform;
                    lN = Instantiate(layerNote,parentObject.transform);//make the pop ups of notes
                    lN.gameObject.tag = "Note";
                    //var trans = JsonHelper.FromJson<string>(note.position);

                    lN.AddComponent<PopUpNoteRotation>();
                    lN.transform.GetChild(0).transform.Find("NoteTitle").GetComponent<TMP_Text>().text = note.title;
                    lN.transform.GetChild(0).transform.Find("Creator").GetComponent<TMP_Text>().text = "Creator: " + authSc.GetUserNameByID(Int32.Parse(note.user_id));
                    lN.onClick.AddListener(() => ShowNote(JsonUtility.ToJson(note)));
                    lN.transform.localPosition = JsonConvert.DeserializeObject<Vector3>(trans[0]);
                    lN.transform.localRotation = JsonConvert.DeserializeObject<Quaternion>(trans[1]);
                   
                }
            }
        }
    }


   

  
    private void ToConsole(string msg)
    {
        Debug.Log(msg);
    }

 


    public void AddNoteToDB()
    {    
        var goN = Instantiate(layerNote, parentObject.transform);
        Transform transform= gameObject.transform;
        string[] trans = new String[2];
        if (buildSc.selectedGo != null)
        {
            goN.transform.localPosition = Vector3.MoveTowards(buildSc.selectedGo.transform.localPosition, Camera.main.transform.localPosition, step);
            trans[0] = JsonUtility.ToJson(goN.transform.localPosition);
            goN.AddComponent<PopUpNoteRotation>();
            trans[1] = JsonUtility.ToJson(goN.transform.localRotation);
        }
        else
            trans[0] = JsonUtility.ToJson(Vector3.zero);

        
        
        
        var jtrans = JsonHelper.ToJson(trans);
        Notes note = new()
        {
            user_id = authSc.userData.ID.ToString(),//AuthenticationService.Instance.PlayerId,
            title = titleNote.text,//+" Projectname" + System.DateTime.Now.ToString(),
            text = textNote.text,
            gobject = gOname.GetComponentInChildren<TMP_Text>().text,
            project_id = "30",
            
            position = jtrans//jsoned transform
        };

        titleNote.text = "";
        textNote.text = "";
        gOuser.GetComponentInChildren<TMP_Text>().text = authSc.userData.name;
        gOname.GetComponentInChildren<TMP_Text>().text = "Select Object";
        gOpos.GetComponentInChildren<TMP_Text>().text = "Select Object";
        var jnote = JsonUtility.ToJson(note);

        if (buildSc.selectedGo != null)
        {
            goN.onClick.AddListener(() => ShowNote(jnote));
            goN.tag = "Note";
            goN.transform.GetChild(0).transform.Find("NoteTitle").GetComponent<TMP_Text>().text = note.title;
            goN.transform.GetChild(0).transform.Find("Creator").GetComponent<TMP_Text>().text = "Creator: " + authSc.GetUserNameByID(Int32.Parse(note.user_id));
        }
        StartCoroutine(contactService.PostData_Coroutine(jnote, "http://"+authSc.ipAddress+":3000/notes"));
    }



    //Create the note list in the handy thingy
    public void OnGetNotesbyname()
    {
        StartCoroutine(contactService.GetRequest("http://"+authSc.ipAddress+":3000/notes"));
    }


    public void OnGetOneNotebyID(string id)
    {
    }

    public void OnGetNoteListbyProjectID(string id)
    {
        StartCoroutine(contactService.GetRequest("http://"+authSc.ipAddress+":3000/notes/byproject/" + id));
    }

    public void GetProjectList()
    {
        Debug.Log("GetProjectList()");
    }



    public void CallNoteUpdate(Notes note)
    {
        if (note != null)
        {
            StartCoroutine(contactService.UploadNotes("note", 11,"uri"));
        }
    }
  

 
    public void ChangeTab(TMP_Text text)
    {
        if(savedNotes.activeSelf)
        {
            savedNotes.SetActive(false);
           
            newNote.SetActive(true);
            gOuser.transform.GetComponentInChildren<TMP_Text>().text = authSc.userData.name;
            gOname.GetComponentInChildren<TMP_Text>().text = "Select Object";
            gOpos.transform.GetComponentInChildren<TMP_Text>().text = "Select Object";
            textNote.text = "";
            titleNote.text = "";
            for (var i = savedContext.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(savedContext.transform.GetChild(i).gameObject);
            }
            text.text = "Notes";
        }
        else
        { newNote.SetActive(false);
        savedNotes.SetActive(true);
            OnGetNoteListbyProjectID("30");
            text.text = "Note";
        }
    }



    public Notes UpdateNote(Notes oldNote)
    {

        Notes note = new Notes
        {
            //Id= oldNote.Id,
            user_id=oldNote.user_id,
            title = "Projectname" + System.DateTime.Now.ToString(),
            text = "just updated",
            gobject = oldNote.gobject,
            project_id = oldNote.project_id,
            position = oldNote.position
        };
        return note;

    }

    public void ShowNote(string jnote)
    {
        newNote.SetActive(true);
        
        savedNotes.SetActive(false) ;
        for (var i = savedContext.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(savedContext.transform.GetChild(i).gameObject);
        }
        var oldNote = JsonConvert.DeserializeObject<Notes>(jnote);

        titleNote.text=oldNote.title;
        textNote.text = oldNote.text;
        
        gOname.GetComponentInChildren<TMP_Text>().text = oldNote.gobject;
       
        if (oldNote.position != null && oldNote.position.Contains("Item"))
        {
            var trans = JsonHelper.FromJson<string>(oldNote.position);
            gOpos.transform.GetComponentInChildren<TMP_Text>().text = trans[0];
        }
        else
        {
            gOpos.transform.GetComponentInChildren<TMP_Text>().text = new Vector3(0, 0, 0).ToString();
        }

        gOuser.transform.GetComponentInChildren<TMP_Text>().text = authSc.GetUserNameByID(Int32.Parse(oldNote.user_id));
    }


   

}
