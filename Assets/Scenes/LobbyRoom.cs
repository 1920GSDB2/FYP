using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(Button))]
public class LobbyRoom : MonoBehaviour
{
    public string RoomId { get; private set; }
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI Owner;
    public TextMeshProUGUI GameMode;
    public TextMeshProUGUI PlayerNumber;
    public bool Updated { get; set; }
    Button EnterRoomButton;
    //Hashtable customProperties = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        EnterRoomButton = GetComponent<Button>();
        EnterRoomButton.onClick.AddListener(delegate { EnterRoom(RoomId); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetRoom(string roomId, string name, int playerNum)
    {
        RoomId = roomId;
        RoomName.text = name;
        PlayerNumber.text = playerNum.ToString() + "/8";
    }
    public void EnterRoom(string roomId)
    {
        LobbyManager.instance.JoinRoom(roomId);
        //if(PhotonNetwork.GetRoomList()[0].n)
        //if (PhotonNetwork.JoinRoom(roomName))
        //{
        //    print("Join room: "+roomName);          
        //}
        //else
        //{
        //    print("Join room failed.");
        //}
    }
    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }
}
