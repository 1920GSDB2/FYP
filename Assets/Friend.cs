using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Friend : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI FriendNameText, StatusLabel;
    public string FriendName
    {
        get { return FriendNameText.text; }
        set
        {
            FriendNameText.text = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Button.onClick.AddListener(delegate { StartChat(); });
    }


    public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
    {
        string _status;

        switch (status)
        {
            case 1:
                _status = "Invisible";
                break;
            case 2:
                _status = "Online";
                break;
            case 3:
                _status = "Away";
                break;
            case 4:
                _status = "Do not disturb";
                break;
            case 5:
                _status = "Looking For Game/Group";
                break;
            case 6:
                _status = "Playing";
                break;
            default:
                _status = "Offline";
                break;
        }

        StatusLabel.text = _status;

    }
}
