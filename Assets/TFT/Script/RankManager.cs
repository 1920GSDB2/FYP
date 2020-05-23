using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TFT
{
    public class RankManager : MonoBehaviour
    {
        public static RankManager Instance;
        public Main.GameManager MainGameManager;
        private NetworkManager NetworkManager;

        //public List<string> PlayersName;
        public Dictionary<string, PlayerRank> PlayersCollection = new Dictionary<string, PlayerRank>();

        public Transform RankList;
        public GameObject PlayerInfo;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            NetworkManager = NetworkManager.Instance;
            try
            {
                PlayerCollectionSetup();
            }
            catch (NullReferenceException) { }
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Setup player ranking data, called by NetworkManager
        /// </summary>
        /// <param name="_initHP"></param>
        public void PlayerCollectionSetup()
        {
            //foreach (string PlayerName in NetworkManager.PlayersId)
            //{
            //    PlayerInfo newPlayerInfo = Instantiate(PlayerInfo, RankList).GetComponent<PlayerInfo>();
            //    bool _isRemote = PhotonNetwork.playerName.Equals(PlayerName);
            //    PlayerRank newPlayerRank = new PlayerRank(PlayerName, MainGameManager.PlayerInitHP, newPlayerInfo, _isRemote);
            //    if(newPlayerRank!=null)
            //    PlayersCollection.Add(PlayerName, newPlayerRank);
            //}
            for (int i = 0; i < NetworkManager.PlayersId.Length; i++)
            {
                PlayerInfo newPlayerInfo = Instantiate(PlayerInfo, RankList).GetComponent<PlayerInfo>();
                bool _isRemote = PhotonNetwork.playerName.Equals(NetworkManager.PlayersId[i]);
                PlayerRank newPlayerRank = new PlayerRank(NetworkManager.PlayerName[i], NetworkManager.PlayersId[i],
                    MainGameManager.PlayerInitHP, newPlayerInfo, _isRemote);
                if (newPlayerRank != null)
                    PlayersCollection.Add(NetworkManager.PlayersId[i], newPlayerRank);
            }
        }

        /// <summary>
        /// It will be called by player while he/she lost in that round.
        /// </summary>
        /// <param name="_deductHP"></param>
        public void Lost(int _deductHP)
        {
            NetworkManager.PhotonView.RPC("RankChange", PhotonTargets.All, PhotonNetwork.playerName, _deductHP);
        }

        /// <summary>
        /// Called by PunRPC in NetworkManager
        /// </summary>
        /// <param name="_player"></param>
        /// <param name="_value"></param>
        public void DeductHP(string _player, int _value)
        {
            PlayersCollection[_player].DeductHP(_value);
            SortInfoPos();
        }

        private void SortInfoPos()
        {
            for(int i =0; i< RankList.childCount; i++)
            {
                for(int j = i + 1; j < RankList.childCount; j++)
                {
                    PlayerInfo playerI = RankList.GetChild(i).GetComponent<PlayerInfo>();
                    PlayerInfo playerJ = RankList.GetChild(j).GetComponent<PlayerInfo>();
                    if (playerJ.CurrHp > playerI.CurrHp)
                    {
                        Transform temp = RankList.GetChild(j);
                        RankList.GetChild(j).SetSiblingIndex(i);
                        temp.SetSiblingIndex(j);
                    }
                }
            }
        }
    }

}