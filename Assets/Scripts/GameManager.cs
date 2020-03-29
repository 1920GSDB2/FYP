using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;

namespace Main
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameManager")]
    public class GameManager : ScriptableObject
    {
        public string databaseIP = "";
        public byte MaxRoomPlayer;
        public UserData userData;

        public string lobbyScene;

        public int PlayerInitHP;
        public float readyingTime, playingTime, compingTime, transitionTime;        //Sync to photon
        public List<Hero> heroTypes = new List<Hero>();                             //List of total heroes
        public Item[] ItemTypes;
        public BlendItem[] BlendItemTypes;

        public int ExpPrice = 4;
        public int RefreshPrice = 2;
        
        public int[] TFTExpCurve;
        public int[] TFTMonsterRound;


        void OnEnable()
        {
            userData = new UserData();
            //Instance = this;
        }

    }
}

