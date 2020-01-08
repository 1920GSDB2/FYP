using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{
    public GameManager gameManager;
    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }
    private PhotonView PhotonView;
    private int PlayersInGame = 0;
 

    // Use this for initialization
    private void Awake()
    {
        Instance = this;    
        PhotonView = GetComponent<PhotonView>();

        if (gameManager.userData.name.Equals(""))
            PlayerName = "Player " + Random.Range(1000, 9999);
        else
            PlayerName = gameManager.userData.name;

        // PhotonNetwork.sendRate = 60;
        //  PhotonNetwork.sendRateOnSerialize = 30;

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }


    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
        }
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(1);
    }
    /* private void MasterLoadedGame()
     {
        // PlayersInGame = 1;
         PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
         PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
         print("player " + PlayersInGame);
     }

     private void NonMasterLoadedGame()
     {
         PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
     }

     [PunRPC]
     private void RPC_LoadGameOthers()
     {
         PhotonNetwork.LoadLevel(1);
     }

     [PunRPC]
     private void RPC_LoadedGameScene(PhotonPlayer photonPlayer)
     {       
         PlayersInGame++;
         print("player++."+PlayersInGame);
         if (PlayersInGame == PhotonNetwork.playerList.Length)
         {
             print("All players are in the game scene.");
             //PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
         }
     }
     [PunRPC]
     private void RPC_Test()
     {
         print("test");    
     }
     [PunRPC]
     private void RPC_CreatePlayer() {
         float randomValue = Random.Range(0, 5);
         PhotonNetwork.Instantiate(Path.Combine("Prefabs","newPlayer"),Vector3.up*randomValue,Quaternion.identity,0);
     }*/
    // Update is called once per frame

}
