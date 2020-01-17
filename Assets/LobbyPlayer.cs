using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyPlayer : MonoBehaviour
{
    public TextMeshProUGUI PlayerLevel;
    public Image PlayerIcon;
    public TextMeshProUGUI PlayerName;
    public Button ReadyButton;
    public Button KickButton;
    public TextMeshProUGUI ReadyButtonText;

    public PhotonPlayer PhotonPlayer { get; private set; }
    public bool IsReady { get; private set; }
    bool isLocal;
    // Start is called before the first frame update
    void Start()
    {
        //if (isLocal) KickButton.gameObject.SetActive(false);
        
        ReadyButton.onClick.AddListener(delegate { LobbyManager.PhotonView.RPC("RPC_Ready", PhotonTargets.All, PhotonPlayer); });
        KickButton.onClick.AddListener(delegate { Kick(); });

    }
    public void SetPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        PlayerName.text = photonPlayer.NickName;
        if (!PhotonNetwork.isMasterClient ||PhotonNetwork.masterClient == photonPlayer)
            KickButton.gameObject.SetActive(false);
        if (photonPlayer != PhotonNetwork.player)
            ReadyButton.enabled = false;
    }

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
    
    public void Kick()
    {
        //LobbyManager.PhotonView.RPC("LobbyManagr.instance.RPC_OnLeftRoom", PhotonPlayer);
        //PhotonNetwork.CloseConnection(PhotonPlayer);
        LobbyManager.PhotonView.RPC("RPC_OnLeftRoom", PhotonPlayer);
    }
    [PunRPC]
    public void RPC_OnLeftRoom(PhotonPlayer _photonPlayer)
    {
        if(PhotonNetwork.player == _photonPlayer)
        {
            LobbyManager.OnClickLeftRoom();
        }
    }
    

}
