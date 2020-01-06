using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameManager")]
public class GameManager : ScriptableObject
{
    public string databaseIP = "";
    public UserData userData;
    public string lobbyScene;

    void OnEnable()
    {
        userData = new UserData();
    }
    
}
