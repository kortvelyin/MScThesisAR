using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Vivox;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
//using VivoxUnity;
using UnityEngine.Android;

public class VivoxPlayer : MonoBehaviour
{
    [SerializeField]
    public string VoiceChannelName = "Meeting";
    //public ILoginSession LoginSession;
    private VivoxVoiceManager VoiceManager;
   // IChannelSession channelSession;
   // private int PermissionAskedCount;

    async void InitializeAsync()
    {
       // VoiceManager=VivoxVoiceManager.Instance;
        await UnityServices.InitializeAsync();
       // await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await VivoxService.Instance.InitializeAsync();

        // VoiceManager.OnUserLoggedInEvent += OnUserLoggedIn;
        // VoiceManager.OnUserLoggedOutEvent += OnUserLoggedOut;
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
    }

    public async void LoginToVivoxAsync()
    {
        InitializeAsync();

        LoginOptions options = new LoginOptions();
        options.DisplayName = null;
        Debug.Log("DisplayName: " + options.DisplayName);
        options.EnableTTS = true;
        await VivoxService.Instance.LoginAsync();
        JoinEchoChannelAsync();

    }

 /*   public void SignIntoVivox()
    {
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
        if (PermissionAskedCount == 1 && IsAndroid12AndUp())
        {
            permissionCode = GetBluetoothConnectPermissionCode();
        }
#endif
            PermissionAskedCount++;
            Permission.RequestUserPermission(permissionCode);
        }

        bool IsPermissionsDenied()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
        if (IsAndroid12AndUp())
        {
            return PermissionAskedCount == 2;
        }
#endif
            return PermissionAskedCount == 1;
        }

        //Actual code runs from here
        if (IsMicPermissionGranted())
        {
            VoiceManager.Login(transform.name.ToString());
        }
        else
        {
            if (IsPermissionsDenied())
            {
                PermissionAskedCount = 0;
                VoiceManager.Login(transform.name.ToString());
            }
            else
            {
                AskForPermissions();
                VoiceManager.Login(transform.name.ToString());      //NEED TO FIX !
            }
        }
    }*/

    public async void JoinEchoChannelAsync()
    {
        string channelToJoin = "Lobby";
        await VivoxService.Instance.JoinEchoChannelAsync(channelToJoin, ChatCapability.TextAndAudio);
    }


    private async void SendMessageAsync(string channelName="Lobby", string message="Echo channel works")
    {
        await VivoxService.Instance.SendChannelTextMessageAsync(channelName, message);
    }

    private void BindSessionEvents(bool doBind)
    {
        VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
    }

    private void OnChannelMessageReceived(VivoxMessage message)
    {
        string messageText = message.MessageText;
        string senderID = message.SenderPlayerId;
        string senderDisplayName = message.SenderDisplayName;
        string messageChannel = message.ChannelName;
        Debug.Log("Message back is: "+message.ToString());
    }

    /*void OnUserLoggedIn()
    {
        if (VoiceManager.LoginState == VivoxUnity.LoginState.LoggedIn)
        {
            Debug.Log("Successfully connected to Vivox");
            Debug.Log("Joining voice channel: " + VoiceChannelName);
            VoiceManager.JoinChannel(VoiceChannelName, ChannelType.NonPositional, VivoxVoiceManager.ChatCapability.AudioOnly);
           // _chan = VoiceManager.LoginSession.GetChannelSession(cid);
        }
        else
        {
            Debug.Log("Cannot sing into Vivox, check your credentials and token settings");
        }
    }*/

    /* void OnUserLoggedOut()
     {
         Debug.Log("Disconnecting from voice channel " + VoiceChannelName);
         VoiceManager.DisconnectAllChannels();
         Debug.Log("Disconnecting from Vivox");
         VoiceManager.Logout();
     }
    */

    void OnUserLoggedIn()
    {
        if (VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("Successfully connected to Vivox");
            Debug.Log("Joining voice channel: " + VoiceChannelName);
            VivoxService.Instance.JoinGroupChannelAsync(VoiceChannelName, ChatCapability.TextAndAudio);
        }
        else
        {
            Debug.Log("Cannot sing into Vivox, check your credentials and token settings");
        }
    }

    void OnUserLoggedOut()
    {
        Debug.Log("Disconnecting from voice channel " + VoiceChannelName);
        VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log("Disconnecting from Vivox");
        VivoxService.Instance.LogoutAsync();
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
