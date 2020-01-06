using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class LoginUI : MonoBehaviour
{
    public Vector2 containOriPos;
    public float delayTime = 0.05f;
    [SerializeField]
    RectTransform gameTextContainer;
    [SerializeField]
    TextMeshProUGUI expandStatus;
    [SerializeField]
    TMP_InputField username, password;
    bool isExpand;
    public string Username { get { return username.text; } }
    public string Password { get { return password.text; } }
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
        DatabaseManager.VerifyAccount(Username, Password);
        //StartCoroutine(LoginToDB());
    }
    
}
