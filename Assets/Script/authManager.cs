using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using Unity.Services.Vivox;
using Newtonsoft.Json;

public class authManager : MonoBehaviour
{
    [HideInInspector] public Text logTxt;
    public GameObject signInDisplay;
    public TMP_Text username;
    public TMP_Text password;
    public string ipAddress= "10.1.101.181";
    [HideInInspector]
    public User userData;
    public UserRoot users;

    LoginScreenUI loginScreenUI;
    ContactService contactService;
    LayerLoader layerSc;
    NotesManager notesManager;

    private string baseUser_id;
    public GameObject idOrigin;
    public GameObject userID;
    public Transform data;

    public GameObject loggedInID;

    async void Awake()
    {
        notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();
        layerSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        contactService = GetComponent<ContactService>();
        loginScreenUI = GameObject.Find("LoginScreenUI").GetComponent<LoginScreenUI>();
        await UnityServices.InitializeAsync();
      /*  bool isSignedIn = AuthenticationService.Instance.IsSignedIn;
        if(isSignedIn)
        {
            signInDisplay.SetActive(false);
        }*/
        loggedInID = Instantiate(userID,idOrigin.transform);
        data = loggedInID.transform.Find("DATA");
        SetupEvents();
        GetUsers();

        
    }


    public void Auth(string json)
    {
         users = JsonConvert.DeserializeObject<UserRoot>(json);
        foreach (User user in users.data)
        {
            if ("Arnold A." == user.name)//(loginScreenUI.DisplayNameInput.text == user.name)
            {
                
                userData = user;
                Debug.Log("Creating UserID");

                data.transform.Find("username").gameObject.GetComponent<TMP_Text>().text = user.name;
               /* if(notesManager == null)
                    notesManager = GameObject.Find("NotesUIDocker").GetComponent<NotesManager>();
                notesManager.gOuser.transform.GetComponentInChildren<TMP_Text>().text = user.name;*/
                data.transform.Find("company").gameObject.GetComponent<TMP_Text>().text = user.company;
                data.transform.Find("job").gameObject.GetComponent<TMP_Text>().text = user.job;
                data.transform.Find("title").gameObject.GetComponent<TMP_Text>().text = user.title;
                baseUser_id = user.ID.ToString();
                if(loginScreenUI==null)
                    loginScreenUI = GameObject.Find("LoginScreenUI").GetComponent<LoginScreenUI>();
                SignIn();
                loginScreenUI.LoginToVivoxService();
               
            }
            
        }
    }

    public void GetUsers()
    {
        // StartCoroutine(contactService.GetRequest("http://localhost:3000/users"));
        //StartCoroutine(contactService.GetRequest("http://10.1.101.181:3000/users"));
        //ipAddress = "10.1.101.181";
        string uri = "http://" + ipAddress + ":3000/users";
        Debug.Log(uri);
        StartCoroutine(contactService.GetRequest(uri));
    }

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            
        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };


        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };


        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };

    }

    public async void SignUpP()
    {
        Debug.Log(username.text + "  " + password.text);
        await SignUpWithUsernamePassword(username.text, password.text);
    }
    public async void SignInP ()
    {
        await SignInWithUsernamePassword(username.text, password.text);
    }    

    public async void SignIn()
    {
        await SignInAnonymouslyAsync();
        await UnityServices.InitializeAsync();
       // await AuthenticationService.Instance.SignInAnonymouslyAsync();

       

    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public async Task SignUpWithUsernamePassword(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }


   public async Task SignInWithUsernamePassword(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    public string GetUserNameByID(int id)
    {
        return users.data[id - 1].name;
    }
}

    