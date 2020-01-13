using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LobbyRoom : MonoBehaviour
{

    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI Owner;
    public TextMeshProUGUI GameMode;
    public TextMeshProUGUI PlayerNumber;
    public bool Updated { get; set; }
    Button EnterRoomButton;

    // Start is called before the first frame update
    void Start()
    {
        EnterRoomButton = GetComponent<Button>();
        EnterRoomButton.onClick.AddListener(delegate { EnterRoom(RoomName.text); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setRoomName(string roomName) {
        RoomName.text = roomName;
    }
    public void EnterRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            print("Join room: "+roomName);          
        }
        else
        {
            print("Join room failed.");
        }
    }
    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }
}
