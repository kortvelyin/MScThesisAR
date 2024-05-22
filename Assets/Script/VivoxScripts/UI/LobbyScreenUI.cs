using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class LobbyScreenUI : MonoBehaviour
{
    public Button LogoutButton;
    public GameObject LobbyScreen;
    public GameObject ConnectionIndicatorDot;
    public GameObject ConnectionIndicatorText;
    LoginScreenUI loginScreen;
    private float _nextPosUpdate;
    LayerLoader loaderSc;
    Build buildSc;

    EventSystem m_EventSystem;
    Image m_ConnectionIndicatorDotImage;
    Text m_ConnectionIndicatorDotText;
    bool JoinedChannel=false;

    void Start()
    {
        loaderSc = GameObject.Find("Building").GetComponent<LayerLoader>();
        buildSc = GameObject.Find("Building").GetComponent<Build>();
        loginScreen = GameObject.Find("LoginScreenUI").GetComponent<LoginScreenUI>();
        m_EventSystem = EventSystem.current;
        if (!m_EventSystem)
        {
            Debug.LogError("Unable to find EventSystem object.");
        }
        m_ConnectionIndicatorDotImage = ConnectionIndicatorDot.GetComponent<Image>();
        if (!m_ConnectionIndicatorDotImage)
        {
            Debug.LogError("Unable to find ConnectionIndicatorDot Image object.");
        }
        m_ConnectionIndicatorDotText = ConnectionIndicatorText.GetComponent<Text>();
        if (!m_ConnectionIndicatorDotText)
        {
            Debug.LogError("Unable to find ConnectionIndicatorText Text object.");
        }

        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        VivoxService.Instance.ConnectionRecovered += OnConnectionRecovered;
        VivoxService.Instance.ConnectionRecovering += OnConnectionRecovering;
        VivoxService.Instance.ConnectionFailedToRecover += OnConnectionFailedToRecover;

        m_ConnectionIndicatorDotImage.color = Color.green;
        m_ConnectionIndicatorDotText.text = "Connected";

        _nextPosUpdate = Time.time;
        LogoutButton.onClick.AddListener(() => { LogoutOfVivoxServiceAsync(); });
        LobbyScreen.SetActive(true);
         JoinLobbyChannel();
        LogoutButton.interactable = true;
        m_EventSystem.SetSelectedGameObject(LogoutButton.gameObject, null);
        // Make sure the UI is in a reset/off state from the start.
        //loaderSc.SaveBlocks("showroom");
        OnUserLoggedOut();
        
    }

 /*   void Update()
    {
       if(VivoxService.Instance.IsLoggedIn)
       {
            Debug.Log("Vivox logged in");
        if (Time.time > _nextPosUpdate)
        {
            VivoxService.Instance.Set3DPosition(Camera.main.gameObject, VivoxVoiceManager.LobbyChannelName);
            _nextPosUpdate += 0.5f; // Only update after 0.3 or more seconds
        }
       }
    }*/

    void OnDestroy()
    {
        VivoxService.Instance.LoggedIn -= OnUserLoggedIn;
        VivoxService.Instance.LoggedOut -= OnUserLoggedOut;
        VivoxService.Instance.ConnectionRecovered -= OnConnectionRecovered;
        VivoxService.Instance.ConnectionRecovering -= OnConnectionRecovering;
        VivoxService.Instance.ConnectionFailedToRecover -= OnConnectionFailedToRecover;

        LogoutButton.onClick.RemoveAllListeners();
    }

    Task JoinLobbyChannel()
    {
        return VivoxService.Instance.JoinGroupChannelAsync(VivoxVoiceManager.LobbyChannelName, ChatCapability.TextAndAudio);
        //Channel3DProperties props = new Channel3DProperties();
        //return VivoxService.Instance.JoinPositionalChannelAsync(VivoxVoiceManager.LobbyChannelName, ChatCapability.TextAndAudio, props);
    }

    async void LogoutOfVivoxServiceAsync()
    {
        LogoutButton.interactable = false;

        await VivoxService.Instance.LogoutAsync();
        AuthenticationService.Instance.SignOut();
    }

    async void OnUserLoggedIn()
    {
        Debug.Log("USER LOGGED IN");
        LobbyScreen.SetActive(true);
        await JoinLobbyChannel();
        //LogoutButton.interactable = true;
        LogoutButton.gameObject.SetActive(false);
        m_EventSystem.SetSelectedGameObject(LogoutButton.gameObject, null);
    }

    void OnUserLoggedOut()
    {
        Debug.Log("USER LOGGED OUT");
        LogoutButton.gameObject.SetActive(false);

        
        //JoinLobbyChannel();
        //loginScreen.LoginToVivoxService();
        // LobbyScreen.SetActive(false);
    }

    void OnConnectionRecovering()
    {
        m_ConnectionIndicatorDotImage.color = Color.yellow;
        m_ConnectionIndicatorDotText.text = "Connection Recovering";
    }

    void OnConnectionRecovered()
    {
        m_ConnectionIndicatorDotImage.color = Color.green;
        m_ConnectionIndicatorDotText.text = "Connection Recovered";
    }

    void OnConnectionFailedToRecover()
    {
        m_ConnectionIndicatorDotImage.color = Color.black;
        m_ConnectionIndicatorDotText.text = "Connection Failed to Recover";
    }
}
