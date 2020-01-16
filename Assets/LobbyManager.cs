using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public enum FunctionPanelType
    {
        StartPanel,
        LobbyPanel,
        InformationPanel
    }
    public enum UsingPanelType
    {
        MatchModePanel,
        CreateCustomPanel,
        JoinCustomPanel,
        GameRoomPanel
    }
    [Header("Function Buttons")]
    public Button StartButton;
    public Button LobbyButton;
    public Button InformationButton;

    [Header("Game Mode Buttons")]
    public Button MatchButton;
    public Button CreateCustomButton;
    public Button JoinCustomButton;

    [Header("Game Start Buttons")]
    public Button OkayButton;
    public Button BackLobbyButton;
    public Button RoomStartButton;
    public Button RoomBackLoobyButton;

    [Header("Function Panels")]
    public GameObject StartPanel;
    public GameObject LobbyPanel;
    public GameObject InformationPanel;
    GameObject[] FunctionPanelsArr = new GameObject[3];

    [Header("Using Panels")]
    public GameObject MatchModePanel;
    public GameObject CreateCustomPanel;
    public GameObject JoinCustomPanel;
    public GameObject GameRoomPanel;
    GameObject[] UsingPanelsArr = new GameObject[4];

    [Header("Room Panels")]
    public GameObject RoomListPanel;
    public GameObject RoomInfoPanel;
    public GameObject PlayerListPanel;

    [Header("Input Text")]
    public TextMeshProUGUI CreateRoomNameText;
    public TextMeshProUGUI CurrentRoomName;
    
    [Header("Prefabs")]
    public GameObject LobbyPlayer;
    public GameObject LobbyRoomPrefab;

    UsingPanelType currentModeType = UsingPanelType.MatchModePanel;
    PhotonView PhotonView;
    private List<LobbyRoom> _lobbyRoomButtons = new List<LobbyRoom>();
    private List<LobbyRoom> LobbyRoomButtons
    {
        get { return _lobbyRoomButtons; }
    }
    private List<LobbyPlayer> playerListings = new List<LobbyPlayer>();
    private List<LobbyPlayer> PlayerListings
    {
        get { return playerListings; }
    }
 
    // Start is called before the first frame update
    void Start()
    {
      
        //connect to server
        if (!PhotonNetwork.connected)
        {
            print("Connecting to server..");
            PhotonNetwork.ConnectUsingSettings("0.0.0");
        }
        PhotonView = GetComponent<PhotonView>();
        //Function Panels Manage
        FunctionPanelsArr[0] = StartPanel;
        FunctionPanelsArr[1] = LobbyPanel;
        FunctionPanelsArr[2] = InformationPanel;
        //Using Panels Manage
        UsingPanelsArr[0] = MatchModePanel;
        UsingPanelsArr[1] = CreateCustomPanel;
        UsingPanelsArr[2] = JoinCustomPanel;
        UsingPanelsArr[3] = GameRoomPanel;

        //Add Listener of Buttons
        StartButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.StartPanel); SwitchUsingPanel(UsingPanelType.MatchModePanel); });
        LobbyButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.LobbyPanel); });
        InformationButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.InformationPanel); });
        MatchButton.onClick.AddListener(delegate { SwitchUsingPanel(UsingPanelType.MatchModePanel); });
        CreateCustomButton.onClick.AddListener(delegate { SwitchUsingPanel(UsingPanelType.CreateCustomPanel); });
        JoinCustomButton.onClick.AddListener(delegate { SwitchUsingPanel(UsingPanelType.JoinCustomPanel); });
        OkayButton.onClick.AddListener(delegate { EnterRoom(); });
        BackLobbyButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.LobbyPanel); });
        RoomStartButton.onClick.AddListener(delegate { StartGame(); });
        RoomBackLoobyButton.onClick.AddListener(delegate { OnClickLeftRoom(); });
        SwitchFunctionPanel(FunctionPanelType.LobbyPanel);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonView.RPC("test", PhotonTargets.Others, "others");
                PhotonView.RPC("test", PhotonTargets.All, "All");
                PhotonView.RPC("test", PhotonTargets.OthersBuffered, "OthersBuffered");
            }
        }
    }

    #region Button Delegate
    public void SwitchFunctionPanel(FunctionPanelType _type)
    {
        foreach (GameObject panel in FunctionPanelsArr)
        {
            panel.SetActive(false);
        }
        switch (_type)
        {
            case FunctionPanelType.StartPanel:
                FunctionPanelsArr[0].SetActive(true);
                break;
            case FunctionPanelType.LobbyPanel:
                FunctionPanelsArr[1].SetActive(true);
                break;
            case FunctionPanelType.InformationPanel:
                FunctionPanelsArr[2].SetActive(true);
                break;
        }
    }

    public void SwitchUsingPanel(UsingPanelType _type)
    {
        currentModeType = _type;
        foreach(GameObject panel in UsingPanelsArr)
        {
            panel.SetActive(false);
        }
        switch (_type)
        {
            case UsingPanelType.MatchModePanel:
                UsingPanelsArr[0].SetActive(true);
                break;
            case UsingPanelType.CreateCustomPanel:
                UsingPanelsArr[1].SetActive(true);                
                break;
            case UsingPanelType.JoinCustomPanel:
                UsingPanelsArr[2].SetActive(true);
                break;
            case UsingPanelType.GameRoomPanel:
                FunctionPanelsArr[0].SetActive(false);
                UsingPanelsArr[3].SetActive(true);                
                break;
        }
    }

    public void EnterRoom()
    {
        switch (currentModeType)
        {
            case UsingPanelType.MatchModePanel:
                //...//
                break;
            case UsingPanelType.CreateCustomPanel:
                SwitchUsingPanel(UsingPanelType.GameRoomPanel);
                CreateRoom();
                //Pass Room Data To Create Room//
                break;
            case UsingPanelType.JoinCustomPanel:
                SwitchUsingPanel(UsingPanelType.GameRoomPanel);
                //Get Room Data To Join Room//
                break;
        }
    }

    public void StartGame()
    {

    }
    private void OnClickLeftRoom()
    {
        print("Leave room");
        SwitchUsingPanel(UsingPanelType.JoinCustomPanel);

        if (PhotonNetwork.isMasterClient)
        {
            print("i am master player left room");
            PhotonView.RPC("RPC_MasterLeftRoom", PhotonTargets.Others);
        }

        while (playerListings.Count != 0)
        {
            Destroy(PlayerListings[0].gameObject);
            PlayerListings.RemoveAt(0);
        }
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    //When Player connect to Master called by photon
    private void OnConnectedToMaster()
    {
        print("Connected to master.");
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.playerName = "player#" + Random.Range(1000, 9999); ;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    //When Player join Lobby called by photon
    private void OnJoinedLobby()
    {
        print("Joined lobby.");

      //  if (!PhotonNetwork.inRoom)
          //  MainCanvasManger.Instance.LobbyCanvas.transform.SetAsLastSibling();
    }

    //Create A room
    private void CreateRoom() {
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 8 };
        if (PhotonNetwork.CreateRoom(CreateRoomNameText.text, roomOptions, TypedLobby.Default))
        {
            print("create room successfully");
            print(CreateRoomNameText.text);
            CurrentRoomName.text = CreateRoomNameText.text;
        }
        else
        {
            print("create room failed");
        }
    }
    //called by photon to check if fail to create room
    private void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {
        print("create room failed: " + codeAndMessage[1]);
    }
    //called by photon to check if successfully to create room
    private void OnCreateRoom()
    {
        print("Room Create successfully.");
    }
    //When receive a room that mean someone create a room. called by photon
    private void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }
        RemoveOldRooms();
    }
    
    private void RoomReceived(RoomInfo room)
    {
        int index = LobbyRoomButtons.FindIndex(x => x.RoomName.text == room.Name);//Find the room name equal to one of element in LobbyRoomButtons List
                                                                                   //if find succesfully will return the index of position in List
        if (index == -1)//cannot find the element
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject LobbyRoomObj = Instantiate(LobbyRoomPrefab);// if cannnot find the Room,add new Room
                LobbyRoomObj.transform.SetParent(RoomListPanel.transform, false);

                LobbyRoom lobbyRoom = LobbyRoomObj.GetComponent<LobbyRoom>();
                LobbyRoomButtons.Add(lobbyRoom);
                index = (LobbyRoomButtons.Count - 1);
            }
        }
        if (index != -1) 
        {
            LobbyRoom lobbyRoom = LobbyRoomButtons[index];
            lobbyRoom.setRoomName(room.Name);
            lobbyRoom.setPlayerNumber(room.playerCount);
            lobbyRoom.Updated = true;
           
        }
    }
    
    private void OnJoinedRoom()//called by photon .When player join room
    {
        SwitchUsingPanel(UsingPanelType.GameRoomPanel);
        
        /* foreach (Transform child in transform)
         {
             Destroy(child.gameObject);
         }*/

        CurrentRoomName.text = CreateRoomNameText.text;
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
       
        //PhotonView.RPC("RPC_PlayerJoinRoom", PhotonTargets.OthersBuffered);
    }
    private void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null)
            return;

        PlayerLeftRoom(photonPlayer);//check the player exist the room;
        GameObject playerListingObj = Instantiate(LobbyPlayer);
        playerListingObj.transform.SetParent(PlayerListPanel.transform, false);

        LobbyPlayer playerListing = playerListingObj.GetComponent<LobbyPlayer>();
        playerListing.setPhotonPlayer(photonPlayer);//set information of player

        PlayerListings.Add(playerListing);

    }
    private void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        int index = PlayerListings.FindIndex(a => a.PhotonPlayer == photonPlayer);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }
    //called by photon
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }
    //called by photon
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }


    private void RemoveOldRooms()
    {
        List<LobbyRoom> removeRooms = new List<LobbyRoom>();

        foreach (LobbyRoom roomListing in LobbyRoomButtons)
        {
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;
        }

        foreach (LobbyRoom roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            LobbyRoomButtons.Remove(roomListing);
            Destroy(roomListingObj);

        }
    }

    [PunRPC]
    private void RPC_MasterLeftRoom()
    {
        print("Master Left Room");
        OnClickLeftRoom();
    }
    [PunRPC]
    private void test(string a)
    {
        print(a);
        
    }


}
