using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class LobbyPlayer : MonoBehaviour
{
    public Image Crown;
    public TextMeshProUGUI PlayerLevel;
    public Image PlayerIcon;
    public TextMeshProUGUI PlayerName;
    public Button ReadyButton;
    public Button KickButton;
    public TextMeshProUGUI ReadyButtonText;

 
    public PhotonPlayer PhotonPlayer { get; private set; }
    public bool IsReady;
    // Start is called before the first frame update
    void Start()
    {
        //if (isLocal) KickButton.gameObject.SetActive(false);
        
        ReadyButton.onClick.AddListener(delegate { LobbyManager.PhotonView.RPC("RPC_Ready", PhotonTargets.All, PhotonPlayer); });
        KickButton.onClick.AddListener(delegate { Kick(); });

    }

    //Setup Lobby Player
    public void SetPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        PlayerName.text = PhotonPlayer.CustomProperties["NAME"].ToString();

        bool readyProperties = (bool)photonPlayer.CustomProperties["READY_FOR_START"];
        Debug.Log(PlayerName.text + " Properties: "+readyProperties);
        if (readyProperties)
        {
            IsReady = true;
            ReadyButtonText.text = "IsReady";
        }

        SetKickButton();

        if (PhotonPlayer != PhotonNetwork.player)
        {
            ReadyButton.enabled = false;
            GetComponent<Image>().enabled = false;
        }

    }

    //Enable or Disable Kick Player Button
    public void SetKickButton()
    {
        if (!PhotonNetwork.isMasterClient || PhotonNetwork.masterClient == PhotonPlayer)
            KickButton.gameObject.SetActive(false);
        else 
            KickButton.gameObject.SetActive(true);

        if (PhotonPlayer.IsMasterClient)
            Crown.enabled = true;
        else
            Crown.enabled = false;
    }

    //Press Ready Button
    public void Ready()
    {
        IsReady = !IsReady;
        if (IsReady)
        {
            ReadyButtonText.text = "IsReady";
            //PhotonPlayer.SetCustomProperties(new Hashtable
            //{
            //    {"READY_FOR_START", "READY" }
            //});
            Hashtable playerProp = PhotonNetwork.player.CustomProperties;
            PhotonNetwork.player.SetCustomProperties(new Hashtable
            {
                {"NAME", playerProp["NAME"] },
                {"READY_FOR_START", true},
                { "Character_Name",GoogleSheetManager.Instance.Skins.currSkin}
            });
            //    = new Hashtable()
            //{
            //    {"NAME", playerName},
            //    {"READY_FOR_START", false},
            //    //{"FOCUSING", 0 },
            //    //{"ISHOST", true },
            //    //{"POSITION", 0 },
            //};
            //PhotonPlayer.CustomProperties["READY_FOR_START"] = true;
        }
        else
        {
            ReadyButtonText.text = "Ready";
            //PhotonPlayer.SetCustomProperties(new Hashtable
            //{
            //    {"READY_FOR_START", "NOT_READY" }
            //});
            // PhotonPlayer.CustomProperties["READY_FOR_START"] = "NOT_READY";
            //PhotonPlayer.CustomProperties["READY_FOR_START"] = false;
            Hashtable playerProp = PhotonNetwork.player.CustomProperties;

            PhotonNetwork.player.SetCustomProperties(new Hashtable
            {
                {"NAME", playerProp["NAME"] },
                {"READY_FOR_START", false },
                {"Character_Name",GoogleSheetManager.Instance.Skins.currSkin}
            });
        }
        LobbyManager.instance.playReadySound();
    }
    
    //Kick Player
    public void Kick()
    {
        LobbyManager.PhotonView.RPC("RPC_OnLeftRoom", PhotonPlayer);
    }    

}
