using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public bool IsReady { get; private set; }
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
        PlayerName.text = PhotonPlayer.NickName;
        
        SetKickButton();

        if (PhotonPlayer != PhotonNetwork.player)
            ReadyButton.enabled = false;
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
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }
    
    //Kick Player
    public void Kick()
    {
        LobbyManager.PhotonView.RPC("RPC_OnLeftRoom", PhotonPlayer);
    }    

}
