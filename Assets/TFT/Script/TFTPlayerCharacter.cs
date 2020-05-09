using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;

public class TFTPlayerCharacter : MonoBehaviour
{

    [PunRPC]
    public void RPC_SyncPlayerCharacterPosition(int posId) {
        transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().playerCharacterSlot;
       /* if (NetworkManager.Instance.posId == posId)
        {
            transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().playerCharacterSlot;
        }
        else {
            transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().opponentCharacterSlot;
        }*/
        transform.localPosition = Vector3.zero;
    }

    [PunRPC]
    public void RPC_PlayerCharacterMoveToGameBoard(int posId)
    {
        transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().opponentCharacterSlot;
     /*   if (NetworkManager.Instance.posId == posId)
        {
            transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().opponentCharacterSlot;            
        }
        else
        {
            transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().playerCharacterSlot;
        }*/
        transform.localPosition = Vector3.zero;
    }
    [PunRPC]
    public void RPC_PlayerCharacterBackToGameBoard(int posId)
    {
        if (NetworkManager.Instance.posId == posId)
            transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().playerCharacterSlot;
       // else
          //  transform.parent = NetworkManager.Instance.PlayerArenas[posId].GetComponent<PlayerArena>().opponentCharacterSlot;

        transform.localPosition = Vector3.zero;
    }
}
