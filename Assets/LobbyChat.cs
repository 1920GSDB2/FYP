using ExitGames.Client.Photon;
using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyChat : MonoBehaviour, IChatClientListener
{
    public ChatClient ChatClient;
    public string UserName { get; set; }
    public string RoomId;
    public string[] FriendsList;                //Loaded from database
    public string[] ChannelsToJoinOnConnect;    //Usually is room Id
    public int HistoryLengthToFetch;


    public void Connect()
    {
        ChatClient = new ChatClient(this);
        string chatAppId = ChatSettings.Instance.AppId;
        this.ChatClient.Connect(chatAppId, "1.0", new Photon.Chat.AuthenticationValues(UserName));
    }
    public void DebugReturn(DebugLevel level, string message)
    {
       
        throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
        {
            this.ChatClient.Subscribe(ChannelsToJoinOnConnect, HistoryLengthToFetch);
        }
        this.ChatClient.AddFriends(FriendsList);
        this.ChatClient.SetOnlineStatus(ChatUserStatus.Online);

        throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
