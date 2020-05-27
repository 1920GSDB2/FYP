using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TFT
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public Camera MainCamera;
        public int loadingTime = 15;
        public GameObject loadingPanel;

        [Header("Manager")]
        public Main.GameManager MainGameManager;
        public LevelManager LevelManager;        //Player Level State
        public RoundManager RoundManager;

        [Header("Player Personal Data")]
        public PlayerHero PlayerHero;           //Hero List for Player
        private PlayerArena selfPlayerArena;     //Game Arena for Player
        public PlayerArena SelfPlayerArena
        {
            get { return selfPlayerArena; }
            set
            {
                selfPlayerArena = value;
                //selfPlayerArenaChange?.Invoke(this, EventArgs.Empty);
                foreach (Transform heroList in SelfPlayerArena.SelfArena.HeroList.transform)
                {
                    heroList.GetComponent<HeroPlace>().isSelect = true;
                }
                foreach (Transform gameBoard in SelfPlayerArena.SelfArena.GameBoard.transform)
                {
                    gameBoard.GetComponent<HeroPlace>().isSelect = true;
                }
            }
        }
        public HeroPlace[] Place;               //I don't know

        [Header("Game Time Data")]
        public float PeriodTime;                //Whole Period Time of Game Status
        private float remainTime;               //Remaining Time of Period Time
        public float RemainTime                 //Getter and Setter of Remaining Time
        {
            get { return remainTime; }
            set
            {
                if (PhotonNetwork.isMasterClient)
                {
                    NetworkManager.PhotonView.RPC("RPC_Time_Sync", PhotonTargets.Others, value);
                }
                if (value <= 0)
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        ChangeGameStatus();
                        NetworkManager.PhotonView.RPC("RPC_ChangeStatus", PhotonTargets.Others, GameStatus, LastGameStatus);
                    }
                }
                else
                {
                    remainTime = value;
                }
            }
        }

        [Header("Game Status Data")]
        public GameStatus LastGameStatus;       //Last Game Status for Switching next Status after Transittng
        [SerializeField]
        private GameStatus gameStatus;          //Current Game Status
        public GameStatus GameStatus            //Getter and Setter of Current Game Status
        {
            get { return gameStatus; }
            set
            {
                gameStatus = value;
                if(value == GameStatus.Readying)
                {
                    readying?.Invoke(this, EventArgs.Empty);
                }
                if (value != GameStatus.Transiting)
                {
                    statusChange?.Invoke(this, EventArgs.Empty);
                }
                //Check the Next Status wheather Playing, to Match the Players' Opponent
                else if (value == GameStatus.Transiting && LastGameStatus == GameStatus.Readying)
                {
                    //Matching the opponent
                    if (PhotonNetwork.isMasterClient)
                    {
                        if (RoundManager.CurrentOpponentType == OpponentType.Player)
                            NetworkManager.Instance.MatchPlayerOpponent();
                        else
                            NetworkManager.Instance.MonsterBattle();
                    }
                    return;
                }
                
                //#region Switch Player's Gameboard's Heroes' Status
                //foreach (NetworkHero gbHero in PlayerHero.GameBoardHeroes)
                //{
                //    //Find the Hero by Using NetworkHero
                //    Hero modifyHero = GetPlayerHero(gbHero);
                //    Debug.Log("Modify Hero Name: " + modifyHero.gameObject.name);
                //    switch (value)
                //    {
                //        case GameStatus.Readying:
                //            modifyHero.HeroStatus = HeroStatus.Standby;
                //            break;
                //        case GameStatus.Playing:
                //            modifyHero.HeroStatus = HeroStatus.Fight;
                //            break;
                //        case GameStatus.Comping:
                //            modifyHero.HeroStatus = HeroStatus.Fight;
                //            break;
                //    }
                //}
                //#endregion

            }
        }

        public EventHandler readying, selfPlayerArenaChange, statusChange;

        [SerializeField]
        Hero LaterUpgradeHero;
        [SerializeField]
        NetworkHero LaterUpgradeNetworkHero;

        void Awake()
        {
            Instance = this;

            LevelManager = new LevelManager(MainGameManager.TFTExpCurve);
            RoundManager = GetComponent<RoundManager>();

            #region Initialization Game Status
            GameStatus = GameStatus.Readying;
            PeriodTime = MainGameManager.readyingTime;
            #endregion

        }

        void Start()
        {
            PlayerHero = new PlayerHero();
            remainTime = PeriodTime + loadingTime;
            SoundSettingManager.Instance.BackGroundSound = 0;
            SoundSettingManager.Instance.GameSound = 0;
            SoundSettingManager.Instance.UiSound = 0;
            if (PhotonNetwork.isMasterClient)
            {
                StartCoroutine(StartLoading(loadingTime));
            }
            readying += OnReadying;
            
        }
        
        private void OnDestroy()
        {
            readying -= OnReadying;
            PlayerHero.BuffList.addBuff -= OnBuffChange;
            PlayerHero.BuffList.upgradeBuff -= OnBuffChange;
            PlayerHero.BuffList.removeBuff -= OnBuffChange;
        }

        public void AddPlayerHeroListener(PlayerHero _playerHero)
        {
            _playerHero.BuffList.addBuff += OnBuffChange;
            _playerHero.BuffList.upgradeBuff += OnBuffChange;
            _playerHero.BuffList.removeBuff += OnBuffChange;
        }
        public void OnBuffChange(object sender, BuffListenEventArgs e)
        {
            BuffersManager.Instance.OnBuffChange(e);
        }
        public void OnAddBuff(object sender, BuffListenEventArgs e)
        {
            Debug.Log("Add Buff Listener: ");
        }

        public void OnUpdrageBuff(object sender, BuffListenEventArgs e)
        {

            Debug.Log("Upgrade Buff Listener: ");
        }

        public void OnRemoveBuff(object sender, BuffListenEventArgs e)
        {
            Debug.Log("Remove Buff Listener: ");

        }

        public void OnReadying(object sender, EventArgs e)
        {
            if (LaterUpgradeHero != null)
            {
                CheckHeroLevelUp(LaterUpgradeNetworkHero);
                //Debug.Log("LaterUpgradeHero Pos: "+ LaterUpgradeNetworkHero.position);
                NetworkManager.Instance.SyncPlayerHero(LaterUpgradeNetworkHero, SyncHeroMethod.RemoveHero);
                PhotonNetwork.Destroy(LaterUpgradeHero.GetComponent<PhotonView>());
                LaterUpgradeHero = null;
                LaterUpgradeNetworkHero = null;
            }
        }

        /// <summary>
        /// Check whether player can buy a hero.
        /// </summary>
        /// <param name="_hero"></param>
        /// <returns></returns>
        public bool BuyHero(Hero _hero)
        {
            #region  Check whether hero list empty
            for (int i = 0; i < SelfPlayerArena.SelfArena.HeroList.childCount; i++)
            {
                if (SelfPlayerArena.SelfArena.HeroList.GetChild(i).childCount == 0)
                {
                    //_hero.gameObject.transform.parent = SelfPlayerArena.SelfArena.HeroList.GetChild(i);
                    //_hero.gameObject.transform.localPosition = Vector3.zero;
                    _hero.GetComponent<PhotonView>().RPC("RPC_AddToHeroList", PhotonTargets.All, NetworkManager.Instance.PosId, i);

                    //Check hero weather level up
                    NetworkHero networkHero = CheckHeroLevelUp(new NetworkHero(_hero.name, i, _hero.HeroLevel));

                    //Cannot level up
                    if (networkHero.HeroLevel == HeroLevel.Level1)
                    {
                        NetworkManager.Instance.SyncPlayerHero(networkHero, SyncHeroMethod.AddHero);
                    }
                    //Can level up and destroy extra hero
                    else
                    {
                        PhotonNetwork.Destroy(_hero.GetComponent<PhotonView>());
                    }

                    return true;
                }
            }
            #endregion

            //Cannot buy hero and destroy extra hero
            //DestroyImmediate(_hero.gameObject);

            return false;
        }

        /// <summary>
        /// Return level upgraded hero or origin hero
        /// </summary>
        /// <param name="_hero"></param>
        /// <returns></returns>
        public NetworkHero CheckHeroLevelUp(NetworkHero _hero)
        {
            //If the hero reach the highest level, stop hero level up
            if (_hero.HeroLevel == HeroLevel.Level3) return _hero;

            // Check same type and level hero wheather exist in two
            List<NetworkHero> heroes = PlayerHero.UsableHeroes;
            List<NetworkHero> sameLvHeroes = new List<NetworkHero>();
            foreach (NetworkHero hero in heroes)
                if (hero.name.Equals(_hero.name) && hero.HeroLevel == _hero.HeroLevel) sameLvHeroes.Add(hero);

            if (sameLvHeroes.Count < 2) return _hero;
            bool containGBHero = sameLvHeroes.Intersect(PlayerHero.GameBoardHeroes).Any();

            NetworkHero temp, del;

            if (sameLvHeroes.Intersect(PlayerHero.GameBoardHeroes).Any())
            {
                //Level up heroes contains any gameboard hero and game status is not readying
                //Don't remove hero now
                if (GameStatus != GameStatus.Readying)
                {
                    Debug.Log("LaterUpgradeHero Set Position: " + _hero.position);
                    LaterUpgradeHero = NetworkManager.Instance.GetHeroByNetworkHero(_hero, 0);
                    LaterUpgradeNetworkHero = _hero;
                    //LaterUpgradeHero = NetworkManager.Instance.GetHeroByNetworkHero(_hero);
                    return _hero;
                }
                //Level up heroes contains any gameboard hero and game status is readying
                //Remove hero which is not in gameboard
                else
                {
                    NetworkHero a = sameLvHeroes[0];
                    NetworkHero b = sameLvHeroes[1];
                    //a does not exist in gameboard
                    if (!PlayerHero.GameBoardHeroes.Contains(a))
                    {
                        { del = a; temp = b; }
                        Debug.Log("Del Pos: " + del.position);
                        NetworkManager.Instance.SyncPlayerHero(del, SyncHeroMethod.RemoveHero);
                        PhotonNetwork.Destroy(SelfPlayerArena.SelfArena.HeroList.GetChild(del.position).GetChild(0).gameObject);

                        temp.HeroLevel++;
                        Debug.Log("Temp Pos: " + temp.position);
                        NetworkManager.Instance.SyncPlayerHero(temp, SyncHeroMethod.HeroUpgrade);
                        SelfPlayerArena.SelfArena.GameBoard.GetChild(temp.position)
                            .GetChild(0).GetComponent<Hero>().photonView.RPC("RPC_Upgrade", PhotonTargets.All);
                    }
                    // Both exist in gameboard, delete the smaller position id one
                    else
                    {
                        if (a.position < b.position) { del = a; temp = b; }
                        else { del = b; temp = a; }
                        Debug.Log("Del Pos: " + del.position);
                        NetworkManager.Instance.SyncPlayerHero(del, SyncHeroMethod.RemoveHero);
                        PhotonNetwork.Destroy(SelfPlayerArena.SelfArena.GameBoard.GetChild(del.position).GetChild(0).gameObject);

                        temp.HeroLevel++;
                        Debug.Log("Temp Pos: " + temp.position);
                        NetworkManager.Instance.SyncPlayerHero(temp, SyncHeroMethod.HeroUpgrade);
                        SelfPlayerArena.SelfArena.GameBoard.GetChild(temp.position)
                            .GetChild(0).GetComponent<Hero>().photonView.RPC("RPC_Upgrade", PhotonTargets.All);
                    }
                }
            }
            //Both does not exist in gameboard, delete the larger position id one
            else
            {
                NetworkHero a = sameLvHeroes[0];
                NetworkHero b = sameLvHeroes[1];
                if (a.position > b.position) { del = a; temp = b; }
                else { del = b; temp = a; }

                Debug.Log("Del Pos: " + del.position);
                NetworkManager.Instance.SyncPlayerHero(del, SyncHeroMethod.RemoveHero);
                PhotonNetwork.Destroy(SelfPlayerArena.SelfArena.HeroList.GetChild(del.position).GetChild(0).gameObject);

                temp.HeroLevel++;
                Debug.Log("Temp Pos: " + temp.position);
                NetworkManager.Instance.SyncPlayerHero(temp, SyncHeroMethod.HeroUpgrade);
                SelfPlayerArena.SelfArena.HeroList.GetChild(temp.position)
                    .GetChild(0).GetComponent<Hero>().photonView.RPC("RPC_Upgrade", PhotonTargets.All);
            }

           
            return CheckHeroLevelUp(temp);
        }

        /// <summary>
        /// Put or take hero from the gameboard
        /// </summary>
        /// <param name="hero"></param>
        public void ChangeHeroPos(Hero _hero)
        {
            //Debug.Log("Last Position: [" + _hero.LastHeroPlace.PlaceId + "], current position [" + _hero.HeroPlace.PlaceId + "].");
            SyncMoveHero moveHeroMethod = SyncMoveHero.Undefined;

            #region Check Hero Moving Type
            //Move Hero in Same Hero Place Type
            if (_hero.HeroPlace.gameObject.name.Equals(_hero.LastHeroPlace.gameObject.name))
            {
                moveHeroMethod = SyncMoveHero.MoveHero;
            }
            //Remove Hero from Gameboard
            else if (_hero.HeroPlace.gameObject.name.Equals("Square"))
            {
                moveHeroMethod = SyncMoveHero.RemoveGameboard;
                //TeamFlag.Instance.GameboardCard.text = (PlayerHero.GameBoardHeroes.Count - 1).ToString();
            }
            //Add Hero to GameBoard
            else /*if(PlayerHero.GameBoardHeroes.Count < LevelManager.Level)*/
            {
                moveHeroMethod = SyncMoveHero.AddGameboard;
                //TeamFlag.Instance.GameboardCard.text = (PlayerHero.GameBoardHeroes.Count + 1).ToString();
            }
            #endregion
            Debug.Log("Moving method: " + moveHeroMethod);
            //Sync to all players
            //if (moveHeroMethod != SyncMoveHero.Undefined)
            NetworkManager.Instance.SyncPlayerHeroPlace(_hero, moveHeroMethod);
        }

        /// <summary>
        /// Compile the NetworkHero to Hero from PlayHero and GameBoard
        /// </summary>
        /// <param name="_networkHero"></param>
        /// <returns></returns>
        public Hero GetPlayerHero(NetworkHero _networkHero)
        {
            return SelfPlayerArena.SelfArena.GameBoard.GetChild(_networkHero.position).GetChild(0).GetComponent<Hero>();
        }

        /// <summary>
        /// Change the game status, while the time is 0
        /// </summary>
        private void ChangeGameStatus()
        {
            //When the transiting time is finish, it will determine the next status of game
            if (GameStatus == GameStatus.Transiting)
            {
               // Debug.Log("change game berfre Game st" + GameStatus);
                //Use the last game status to switch the next game status
                switch (LastGameStatus)
                {
                    case GameStatus.Readying:
                        PeriodTime = MainGameManager.playingTime;
                        GameStatus = GameStatus.Playing;
                        break;
                    case GameStatus.Playing:
                        //(move to transiting status )If some players' heroes are not die yet, it will switch to extra time status
                                        
                        PeriodTime = MainGameManager.readyingTime;
                        GameStatus = GameStatus.Readying;
                        if (PhotonNetwork.isMasterClient)
                        {
                            RoundManager.RoundUp();
                            NetworkManager.PhotonView.RPC("RPC_RoundUp", PhotonTargets.Others);
                        }
                        break;
                    case GameStatus.Comping:
                        PeriodTime = MainGameManager.readyingTime;
                        GameStatus = GameStatus.Readying;
                        if (PhotonNetwork.isMasterClient)
                        {
                            RoundManager.RoundUp();
                            NetworkManager.PhotonView.RPC("RPC_RoundUp", PhotonTargets.Others);
                        }
                        break;
                }
             //   Debug.Log("change game after Game st" + GameStatus);
            }
            //Change game status to Transiting
            else
            {
                if (GameStatus == GameStatus.Playing && !NetworkManager.Instance.BattleFinish())
                {
                    PeriodTime = MainGameManager.compingTime;
                    GameStatus = GameStatus.Comping;
                }
                else
                {
                  //  Debug.Log("transit before Game st" + GameStatus);
                    if (GameStatus == GameStatus.Comping&&!NetworkManager.Instance.BattleFinish()) {
                        Debug.Log("increase time no yet finish!!");
                          NetworkManager.PhotonView.RPC("overTimeFinish", PhotonTargets.All);

                    }
                    LastGameStatus = GameStatus;
                    PeriodTime = MainGameManager.transitionTime;
                    GameStatus = GameStatus.Transiting;
                    //Debug.Log("transit after Game st" + GameStatus);
                }
            }

            remainTime = PeriodTime;
        }
        public void finishWave() {          
            if(LastGameStatus==GameStatus.Readying)
            RemainTime = 0;
        }
        public void FinishLoading()
        {
            loadingPanel.SetActive(false);
            SoundSettingManager.Instance.BackGroundSound = 1;
            SoundSettingManager.Instance.GameSound = 1;
            SoundSettingManager.Instance.UiSound = 1;
        }
        public IEnumerator StartLoading(int loadingTime)
        {
            yield return new WaitForSeconds(loadingTime);
            NetworkManager.PhotonView.RPC("RPC_FinishLoading", PhotonTargets.All);
        }
    }
}
