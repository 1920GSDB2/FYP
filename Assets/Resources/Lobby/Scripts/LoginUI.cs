using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UI;

public enum LoginAlertType
{
    CONNECTING,
    ERROR,
    INFO
}
public class LoginUI : MonoBehaviour
{
    public static LoginUI Instance;
    public Vector2 containOriPos;
    public float delayTime = 0.05f;
    [SerializeField]
    RectTransform gameTextContainer;
    [SerializeField]
    TextMeshProUGUI expandStatus;
    [SerializeField]
    TMP_InputField username, password;

    [Header("Alert Message Box")]
    [SerializeField]
    Image AlertBg;
    [SerializeField]
    GameObject AlertContent;
    [SerializeField]
    TextMeshProUGUI AlertTextMessage;
    [SerializeField]
    TextMeshProUGUI AlertSymbol;
    [SerializeField]
    GameObject LoadingCircle;

    [SerializeField]
    private bool isLoading;
    public bool IsLoading
    {
        get { return isLoading; }
        set
        {
            isLoading = value;
            LoadingCircle.SetActive(IsLoading);
        }
    }

    bool isExpand;
    public string Username
    {
        get
        {
            string result = username.text;
            //username.text = "";
            return result;
        }
    }
    public string Password
    {
        get
        {
            string result = password.text;
            //password.text = "";
            return result;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isExpand)
        {
            gameTextContainer.anchoredPosition = Vector2.Lerp(gameTextContainer.anchoredPosition, Vector2.zero, delayTime);
        }
        else
        {
            gameTextContainer.anchoredPosition = Vector2.Lerp(gameTextContainer.anchoredPosition, containOriPos, delayTime);
        }
    }

    public void OnSwitchTextStatus()
    {
        isExpand = !isExpand;
        expandStatus.text = isExpand ? "<" : ">";
    }

    public void OnPressLogin()
    {
        IsLoading = true;
        DatabaseManager.VerifyAccount(Username, Password);
        ShowAlert("Signing In...", LoginAlertType.CONNECTING);
    }
    public void LoginFailed(string message)
    {
        ShowAlert(message, LoginAlertType.ERROR);
    }
    void ReadOnlyField(TMP_InputField field, bool b)
    {
        field.readOnly = b;

    }
    void ReadOnlyField_Login(bool b)
    {
        username.readOnly = b;
        password.readOnly = b;
    }
    void ShowAlert(string message, LoginAlertType type)
    {
        switch (type)
        {
            case LoginAlertType.CONNECTING:
                AlertBg.color = new Color(0.125f, 0.42f, 0.396f, 1);
                LoadingCircle.SetActive(true);
                StartCoroutine(UIHelper.fading(LoadingCircle.GetComponent<CanvasGroup>(), 0, 1, 0.5f));
                AlertSymbol.text = "!";
                ReadOnlyField_Login(true);
                break;
            case LoginAlertType.ERROR:
                LoadingCircle.SetActive(false);
                AlertSymbol.text = "X";
                AlertBg.color = new Color(0.643f, 0, 0);
                ReadOnlyField_Login(false);
                break;
            case LoginAlertType.INFO:
                AlertBg.color = new Color(0.125f, 0.42f, 0.396f, 1);
                LoadingCircle.SetActive(false);
                ReadOnlyField_Login(false);
                AlertSymbol.text = "!";
                break;
            default:
                AlertBg.color = new Color(0.125f, 0.42f, 0.396f, 1);
                LoadingCircle.SetActive(false);
                AlertSymbol.text = "!";
                ReadOnlyField_Login(false);
                break;
        }
        StartCoroutine(UIHelper.fading(AlertContent.GetComponent<CanvasGroup>(), 0f, 1f, 0.8f));
        AlertContent.SetActive(true);
        AlertTextMessage.text = message;
    }
}
