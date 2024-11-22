using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContactService : MonoBehaviour
{
    NotesManager notesManager;
    authManager authM;
    Build build;
    DB dB;
    string jnote;
    public GameObject commCube;
    public 

     void Start()
    {
        authM = GameObject.Find("AuthManager").GetComponent<authManager>();
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();
        build = GameObject.Find("Building").GetComponent<Build>();
        
    }
    

    //this became something very different from the name, but it's a crossroad for 
    //could be a double string list with url and function to call
    public void JsonToDB(string json, string url)
    {
        if (authM == null)
            authM = GameObject.Find("AuthManager").GetComponent<authManager>();
        Debug.Log("json to db: "+url);
        if (url.Contains("http://" + authM.ipAddress + ":3000/notes/byproject/"))
        {
            Debug.Log("demo notes were found: ");
            //notelist by project
            var notes = JsonConvert.DeserializeObject<NotesRoot>(json);
            notesManager.ToConsole(notes.data);
            commCube.GetComponent<Image>().color = Color.green;

        }
        else if (url.Contains("http://"+authM.ipAddress+":3000/notes/"))
                {
                    var notes = JsonConvert.DeserializeObject<Notes>(json);
                    //the update
                    commCube.GetComponent<Image>().color = Color.green;
                    
                }
        else if (url=="http://" + authM.ipAddress + ":3000/projects/byname/Demo")
        {
            Debug.Log("demo project was found: ");
            //layers for the Demo project
            if (build == null)
                build = GameObject.Find("Building").GetComponent<Build>();
            var projects = JsonConvert.DeserializeObject<ProjectRoot>(json);
            build.ToConsole(projects.data);
            commCube.GetComponent<Image>().color = Color.green;

        }
        else if(url.Contains("http://" +authM.ipAddress+":3000/notes"))
                {
                //notelist
                    var notes = JsonConvert.DeserializeObject<NotesRoot>(json);
                    notesManager.ToConsole(notes.data);
                    commCube.GetComponent<Image>().color = Color.green;
                    
                }
           
            else if (url.Contains("http://" +authM.ipAddress+":3000/projects"))
                {
                //list of layers with projects
                    var projects = JsonConvert.DeserializeObject<ProjectRoot>(json);
                    //list these, string on gO, klick andturn that to blocks
                    commCube.GetComponent<Image>().color = Color.green;
                    
                }
            
            else if (url.Contains("http://" + authM.ipAddress + ":3000/users"))
                {
                //user list
                    
                    authM.Auth(json);
                    commCube.GetComponent<Image>().color = Color.green;
                    
                }
            
            
            else
                {
                    commCube.GetComponent<Image>().color = Color.black;
                    Debug.Log("Couldn't find type: " + url);
                    
                }
    }


    public IEnumerator PostData_Coroutine(string json, string uri)
    {
        Debug.Log("In post: "+json+" url: "+uri);
        commCube.GetComponent<Image>().color = Color.yellow;
        //jnote= JsonUtility.ToJson(note);
        using (UnityWebRequest www = UnityWebRequest.Post(uri, json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Form upload failed: "+www.result);
                commCube.GetComponent<Image>().color = Color.red;
            }
            else
            {
                Debug.Log("Form upload complete!: "+ json);
                commCube.GetComponent<Image>().color = Color.green;
            }
        }
    }


   public IEnumerator GetRequest(string uri)
    {
        
        commCube.GetComponent<Image>().color = Color.yellow;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log(string.Format("Something went wrong: {0}", webRequest.error));
                    commCube.GetComponent<Image>().color = Color.red;
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("In GetRequestt: " + webRequest.downloadHandler.text);
                    JsonToDB(webRequest.downloadHandler.text, uri);

                    
                  

                    break;

            }
        }
    }

       public   IEnumerator UploadNotes(string json, int pk, string uri)
    {
        //var newNote = UpdateNote(oldnote);
        //Debug.Log("NewNote: " + newNote.ToString());
        //var jNewNote = JsonUtility.ToJson(newNote);
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
        using (UnityWebRequest www = UnityWebRequest.Put(uri, json))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Upload complete!");
            }
        }
    }
}
