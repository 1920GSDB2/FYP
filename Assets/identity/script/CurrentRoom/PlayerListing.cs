
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
  public PhotonPlayer PhotonPlayer { get; private set; }
    [SerializeField]
    private Text playerNameText;
    private Text PlayerNameText
    {
        get { return playerNameText; }
    }
    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer) {
        PhotonPlayer = photonPlayer;
        PlayerNameText.text = photonPlayer.NickName;
    }
}
