using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameManager")]
    public class GameManager : ScriptableObject
    {
        public string databaseIP = "";
        public byte MaxRoomPlayer;
        public UserData userData;
        public string lobbyScene;
        public float readyingTime, playingTime, compingTime, transitionTime;        //Sync to photon
        public List<Hero> heroTypes = new List<Hero>();         //List of total heroes
        //public static GameManager Instance;

        void OnEnable()
        {
            userData = new UserData();
            //Instance = this;
        }

    }
}

