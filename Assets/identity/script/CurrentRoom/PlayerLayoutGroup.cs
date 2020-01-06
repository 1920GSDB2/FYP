using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject playerListingPrefab;
    private GameObject PlayerListingPrefab
    {
        get { return playerListingPrefab; }
    }
    private List<PlayerListing> playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings
    {
        get { return playerListings; }
    }
    private void OnMasterClientSwitched(PhotonPlayer photonPlayer) {
        PhotonNetwork.LeaveRoom();
    }
    // called bys photon
    private void OnJoinedRoom() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        MainCanvasManger.Instance.CurrentRoomCanvas.transform.SetAsLastSibling();
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++) {
            PlayerJoinedRoom(photonPlayers[i]);
        }
        
    }   
    // called by photon
    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer) {
        PlayerJoinedRoom(photonPlayer);
    }
    
    // called by photon
    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer) {
        PlayerLeftRoom(photonPlayer);
    }
    private void PlayerJoinedRoom(PhotonPlayer photonPlayer) {
        if (photonPlayer == null)
            return;

         PlayerLeftRoom(photonPlayer);
        GameObject playerListingObj = Instantiate(playerListingPrefab);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);

        PlayerListings.Add(playerListing);

    }
    private void PlayerLeftRoom(PhotonPlayer photonPlayer) {
        int index = PlayerListings.FindIndex(a => a.PhotonPlayer == photonPlayer);
        if (index != -1) {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }
    public void OnClickRoomState() {
        if (!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;
    }
    public void OnClickLeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }
}
