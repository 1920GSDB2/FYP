using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class Lobby : Photon.PunBehaviour
    {
        string gameVersion = "1";
        public GameObject controlPanel;
        public GameObject progressLabel;

        void Awake()
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        // Start is called before the first frame update
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            //Connect();
        }

        public void Connect()
        {
            PhotonNetwork.playerName = Main.GameManager.Instance.userData.name;
            PlayerPrefs.SetString("PlayerName", PhotonNetwork.playerName);
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(gameVersion);
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("On call OnConnectedToMaster()");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarning("On call OnDisconnectedFromPhoton()");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("On call OnPhotonRandomJoinFailed()");
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("On call OnJoinedRoom()");
        }
    }

}
