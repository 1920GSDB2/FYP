using ExitGames.Client.Photon;
using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public Main.GameManager GameManager;
    public static ChatManager Instance;
    public TMP_InputField SendMessageField;
    public TextMeshProUGUI DisplayMessageField;
    public Button SendButon;
    protected internal ChatSettings chatAppSettings;

    public ChatClient ChatClient;
    public string UserName;
    public string RoomId;
    public string[] FriendsList;                //Loaded from database
    public string[] ChannelsToJoinOnConnect;    //Usually is room Id
    public int HistoryLengthToFetch;
    public GameObject FriendListUiItemtoInstantiate;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SendButon.onClick.AddListener(delegate { OnClickSend(); });
        ChatClient = new ChatClient(this);

        UserName = "player#" + Random.Range(1000, 9999); 
        if (GameManager.userData.name != null && !GameManager.userData.name.Equals(""))
        {
            UserName = GameManager.userData.name;
        }
        Connect();
    }
    // Update is called once per frame
    void Update()
    {
        ChatClient.Service();
    }

    public void OnClickSend()
    {
        if (this.SendMessageField != null)
        {
            this.SendChatMessage(this.SendMessageField.text);
            this.SendMessageField.text = "";
        }
    }

    public int TestLength = 2048;
    private byte[] testBytes = new byte[2048];

    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }
        if ("test".Equals(inputLine))
        {
            if (this.TestLength != this.testBytes.Length)
            {
                this.testBytes = new byte[this.TestLength];
            }

            this.ChatClient.SendPrivateMessage(this.ChatClient.AuthValues.UserId, this.testBytes, true);
        }


        bool doingPrivateChat = this.ChatClient.PrivateChannels.ContainsKey(this.RoomId);
        string privateChatTarget = string.Empty;
        if (doingPrivateChat)
        {
            // the channel name for a private conversation is (on the client!!) always composed of both user's IDs: "this:remote"
            // so the remote ID is simple to figure out

            string[] splitNames = this.RoomId.Split(new char[] { ':' });
            privateChatTarget = splitNames[1];
        }
        //UnityEngine.Debug.Log("selectedChannelName: " + selectedChannelName + " doingPrivateChat: " + doingPrivateChat + " privateChatTarget: " + privateChatTarget);


        if (doingPrivateChat)
        {
            this.ChatClient.SendPrivateMessage(privateChatTarget, inputLine);
        }
        else
        {
            this.ChatClient.PublishMessage(this.RoomId, inputLine);
        }
    }

    public void Connect()
    {
        Debug.Log("Trying to Connect to Chatting Server");
        ChatClient = new ChatClient(this);
        string chatAppId = ChatSettings.Instance.AppId;
        this.ChatClient.Connect(chatAppId, "0.0.0", new Photon.Chat.AuthenticationValues(UserName));
    }
    public void DebugReturn(DebugLevel level, string message)
    {

        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state.ToString());
    }

    public void OnConnected()
    {
        Debug.Log("Chatting server is connect");
        if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
        {
            this.ChatClient.Subscribe(ChannelsToJoinOnConnect, HistoryLengthToFetch);
        }
        this.ChatClient.AddFriends(FriendsList);
        this.ChatClient.SetOnlineStatus(ChatUserStatus.Online);
        if (this.FriendsList != null && this.FriendsList.Length > 0)
        {
            this.ChatClient.AddFriends(this.FriendsList); // Add some users to the server-list to get their status updates

            // add to the UI as well
            foreach (string _friend in this.FriendsList)
            {
                if (this.FriendListUiItemtoInstantiate != null && _friend != this.UserName)
                {
                    this.InstantiateFriendButton(_friend);
                }

            }

        }
        
    }

    public void OnDisconnected()
    {

    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}{1}={2}, ", msgs, senders[i], messages[i]);
        }
        Debug.Log("Channel Name:" + channelName + "\n" + "Messages: " + msgs);

        ChatChannel channel = null;
        bool found = this.ChatClient.TryGetChannel(channelName, out channel);
        DisplayMessageField.text = channel.ToStringMessages();

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        ChatChannel ch = this.ChatClient.PrivateChannels[channelName];
        foreach (object msg in ch.Messages)
        {
            Debug.Log(msg.ToString());
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }

    public void OnSubscribed(string[] channels, bool[] results)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }

    
    private void InstantiateFriendButton(string friendId)
    {
        GameObject fbtn = (GameObject)Instantiate(this.FriendListUiItemtoInstantiate);
        fbtn.gameObject.SetActive(true);
        FriendItem _friendItem = fbtn.GetComponent<FriendItem>();

        _friendItem.FriendId = friendId;

        fbtn.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);

        //this.friendListItemLUT[friendId] = _friendItem;
    }
    
}
