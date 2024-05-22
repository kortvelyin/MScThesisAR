using System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Services.Vivox;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using Unity.VisualScripting;
using Unity.Services.Authentication;
using UnityEngine.Networking;
using Newtonsoft.Json;
//using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Net;
using UnityEngine.Experimental.Playables;
using Unity.Services.Core;
using System.Threading.Tasks;
using TMPro;



public class LoginScreenUI : MonoBehaviour
{
    public GameObject lobbyScreenUI;
    public Button LoginButton;
    public TMP_InputField DisplayNameInput;
    //public InputField DisplayNameInput;
    public GameObject LoginScreen;
     NotesManager nM;
     Build build;
    authManager authM;
    
    const int k_DefaultMaxStringLength = 15;

    int m_PermissionAskedCount;
    EventSystem m_EventSystem;

    void Start()
    {
        authM = GameObject.Find("AuthManager").GetComponent<authManager>();
        var  goodPlace = GameObject.Find("LoginScreenUI");
        if (this == goodPlace.GetComponent<LoginScreenUI>())
        {
            //build = new Build();
            // Destroy(NotesEmpty.GetComponent<LoginScreenUI>());

            // nM= FindObjectOfType<NotesManager>();
            m_EventSystem = FindObjectOfType<EventSystem>();
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;

#if !(UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID)
        DisplayNameInput.interactable = false;
#else
        
       DisplayNameInput.onEndEdit.AddListener((string text) => { LoginToVivoxService(); });
#endif
        LoginButton.onClick.AddListener(() => { LoginToVivoxService(); });

        
        var systInfoDeviceName = String.IsNullOrWhiteSpace(SystemInfo.deviceName) == false ? SystemInfo.deviceName : Environment.MachineName;
            DisplayNameInput.text = "Arnold A.";//Environment.MachineName.Substring(0, Math.Min(k_DefaultMaxStringLength, Environment.MachineName.Length));
           // DisplayNameInput.text = Environment.MachineName.Substring(0, Math.Min(k_DefaultMaxStringLength, Environment.MachineName.Length));

           // authM.GetUsers();
            OnUserLoggedOut();
        }
            
    }

    void OnDestroy()
    {
        VivoxService.Instance.LoggedIn -= OnUserLoggedIn;
        VivoxService.Instance.LoggedOut -= OnUserLoggedOut;

      //  LoginButton.onClick.RemoveAllListeners();
#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
      //  DisplayNameInput.onEndEdit.RemoveAllListeners();
#endif
    }

    void ShowLoginUI()
    {
        if(LoginScreen.activeInHierarchy == false)
        {
            LoginScreen.SetActive(true);
        }
        if(LoginButton.IsInteractable()==false)
        {
            LoginButton.interactable = true;
        }
        
        //m_EventSystem.SetSelectedGameObject(LoginButton.gameObject, null);

    }

    void HideLoginUI()
    {
        LoginScreen.SetActive(false);
    }

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
    bool IsAndroid12AndUp()
    {
        // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
        const int android12VersionCode = 31;
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

        return buildSdkVersion >= android12VersionCode;
    }

    string GetBluetoothConnectPermissionCode()
    {
        if (IsAndroid12AndUp())
        {
            // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
            AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
            string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

            return permissionCode;
        }

        return "";
    }
#endif

    bool IsMicPermissionGranted()
    {
        bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (IsAndroid12AndUp())
        {
            // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission for all features to work
            isGranted &= Permission.HasUserAuthorizedPermission(GetBluetoothConnectPermissionCode());
        }
#endif
        return isGranted;
    }

    void AskForPermissions()
    {
        string permissionCode = Permission.Microphone;

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (m_PermissionAskedCount == 1 && IsAndroid12AndUp())
        {
            permissionCode = GetBluetoothConnectPermissionCode();
        }
#endif
        m_PermissionAskedCount++;
        Permission.RequestUserPermission(permissionCode);
    }

    bool IsPermissionsDenied()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
        if (IsAndroid12AndUp())
        {
            return m_PermissionAskedCount == 2;
        }
#endif
        return m_PermissionAskedCount == 1;
    }

    public void GetUsers()
    {
        //StartCoroutine(NotesManager.GetRequest("http://localhost:3000/users"));
        //LoginToVivoxService();
        Debug.Log("LoginToVivoxService() was called");
    }




    public void LoginToVivoxService()
    {
         //VivoxService.Instance.InitializeAsync();
        m_EventSystem = FindObjectOfType<EventSystem>();
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        Debug.Log("LoginToVivoxService() was called");
        if (IsMicPermissionGranted())
        {
            Debug.Log("MicPermissionGranted");
            // The user authorized use of the microphone.
            LoginToVivox();
        }
        else
        {
            // We do not have the needed permissions.
            // Ask for permissions or proceed without the functionality enabled if they were denied by the user
            if (IsPermissionsDenied())
            {
                Debug.Log("LoginToVivoxService() permissiondenied");
                m_PermissionAskedCount = 0;
                LoginToVivox();
            }
            else
            {
                AskForPermissions();
            }
        }
    }

    async void LoginToVivox()
    {
        Debug.Log("LoginToVivox() was called");
        LoginButton.interactable = false;
        if(authM==null)
            authM = GameObject.Find("AuthManager").GetComponent<authManager>();
            //var correctedDisplayName = Regex.Replace(DisplayNameInput.text, "[^a-zA-Z0-9_-]", "");
            DisplayNameInput.text = authM.userData.name; //correctedDisplayName.Substring(0, Math.Min(correctedDisplayName.Length, 30));
        //DisplayNameInput.text = correctedDisplayName.Substring(0, Math.Min(correctedDisplayName.Length, 30));
        Debug.Log("DisplayNameInput.text: " + DisplayNameInput.text);
        Debug.Log("correctedDisplayName: " + Regex.Replace(DisplayNameInput.text, "[^a-zA-Z0-9_-]", ""));

        if (string.IsNullOrEmpty(DisplayNameInput.text))
        {
            Debug.LogError("Please enter a display name.");
            return;
        }

        await VivoxVoiceManager.Instance.InitializeAsync(DisplayNameInput.text);
        var loginOptions = new LoginOptions()
        {
            DisplayName = DisplayNameInput.text,
            ParticipantUpdateFrequency = ParticipantPropertyUpdateFrequency.FivePerSecond,
            EnableTTS = true
    };
      
       // await VivoxService.Instance.InitializeAsync();
       // await VivoxService.Instance.LoginAsync();
        
        await VivoxService.Instance.LoginAsync(loginOptions);
        
        lobbyScreenUI.SetActive(true);
    }

    public async void LogoutOfVivoxAsync()
    {
        await VivoxService.Instance.LogoutAsync();
    }

    void OnUserLoggedIn()
    {
        HideLoginUI();
    }

    void OnUserLoggedOut()
    {
        //LoginToVivoxService();
        ShowLoginUI();
    }
}
