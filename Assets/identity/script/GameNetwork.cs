using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetwork : MonoBehaviour
{
    private PhotonView PhotonView;
    private int PlayersInGame = 0;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
        
    }
    private void Start()
    {
        Debug.Log("new scene "+PhotonNetwork.isMasterClient);
    }
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        print("LOAD SCENE");
        if (scene.name == "Game")
        {
            playerLoadGame();
        }
    }
    private void playerLoadGame() {
        print("PlayerLoadGame() ");
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }
    private void MasterLoadedGame()
    {
        // PlayersInGame = 1;
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
        
        print("player " + PlayersInGame);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    

    [PunRPC]
    private void RPC_LoadedGameScene(PhotonPlayer photonPlayer)
    {
        PlayersInGame++;
        print("player++." + PlayersInGame);
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game scene.");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }
    [PunRPC]
    private void RPC_Test()
    {
        print("test");
    }
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        float randomValue = Random.Range(0, 5);
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "newPlayer"), Vector3.up * randomValue, Quaternion.identity, 0);
    }
}
