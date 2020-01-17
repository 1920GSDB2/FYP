using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameManager")]
public class GameManager : ScriptableObject
{
    public string databaseIP = "";
    public byte MaxRoomPlayer;
    public UserData userData;
    public string lobbyScene;
    public float setupTime, playtingTime, extraTime;        //Sync to photon
    public List<Hero> heroTypes = new List<Hero>();         //List of total heroes

    void OnEnable()
    {
        userData = new UserData();
    }
    
}
