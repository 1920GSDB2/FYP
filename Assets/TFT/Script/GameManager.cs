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

        [Header("Manager")]
        public Main.GameManager MainGameManager;
        public LevelManager LevelManager;        //Player Level State
        public RoundManager RoundManager;

        [Header("Player Personal Data")]
        public PlayerHero PlayerHero;           //Hero List for Player
        public PlayerArena SelfPlayerArena;     //Game Arena for Player
        public HeroPlace[] Place;               //I don't know

        [Header("Game Time Data")]
        public float PeriodTime;                //Whole Period Time of Game Status
        private float remainTime;               //Remaining Time of Period Time
        public float RemainTime                 //Getter and Setter of Remaining Time
        {
            get { return remainTime; }
            set
            {
                if (value <= 0)
                {
                    ChangeGameStatus();
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

                //Check the Next Status wheather Playing, to Match the Players' Opponent
                if (value == GameStatus.Transiting && LastGameStatus == GameStatus.Readying)
                {
                    //Matching the opponent
                    if (PhotonNetwork.isMasterClient)
                        NetworkManager.Instance.MatchPlayerOpponent();
                    return;
                }

                #region Switch Player's Gameboard's Heroes' Status
                foreach (NetworkHero gbHero in PlayerHero.GameBoardHeroes)
                {
                    //Find the Hero by Using NetworkHero
                    Hero modifyHero = GetPlayerHero(gbHero);
                    Debug.Log("Modify Hero Name: " + modifyHero.gameObject.name);
                    switch (value)
                    {
                        case GameStatus.Readying:
                            modifyHero.HeroStatus = HeroStatus.Standby;
                            break;
                        case GameStatus.Playing:
                            modifyHero.HeroStatus = HeroStatus.Fight;
                            break;
                        case GameStatus.Comping:
                            modifyHero.HeroStatus = HeroStatus.Fight;
                            break;
                    }
                }
                #endregion

            }
        }

        void Awake()
        {
            Instance = this;

            LevelManager = new LevelManager(MainGameManager.TFTExpCurve);
            RoundManager = GetComponent<RoundManager>();

            #region Initialization Game Status
            GameStatus = GameStatus.Readying;
            PeriodTime = MainGameManager.readyingTime;
            RemainTime = PeriodTime;
            #endregion

        }

        void Start()
        {
            PlayerHero = new PlayerHero();
        }

        void Update()
        {

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
                    _hero.GetComponent<PhotonView>().RPC("RPC_AddToHeroList", PhotonTargets.All, NetworkManager.Instance.posId, i);

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

                        DestroyImmediate(_hero.gameObject);
                    }

                    return true;
                }
            }
            #endregion

            //Cannot buy hero and destroy extra hero
            DestroyImmediate(_hero.gameObject);

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
            if (_hero.HeroLevel == HeroLevel.Level3)
                return _hero;

            #region Check same type and level hero wheather exist in two
            NetworkHero[] heroes = PlayerHero.UsableHeroes.ToArray();
            int sameLvCount = 0;
            NetworkHero sameLvHero = _hero;
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i].name.Equals(_hero.name) && heroes[i].HeroLevel == _hero.HeroLevel)
                {
                    sameLvCount++;
                    if (sameLvCount >= 2)
                    {
                        #region Remove Hero
                        DestroyImmediate(SelfPlayerArena.SelfArena.HeroList.GetChild(heroes[i].position).GetChild(0).gameObject);
                        NetworkManager.Instance.SyncPlayerHero(heroes[i], SyncHeroMethod.RemoveHero);
                        #endregion

                        #region Upgrade Hero
                        sameLvHero.HeroLevel++;
                        SelfPlayerArena.SelfArena.HeroList.GetChild(sameLvHero.position).GetChild(0).GetComponent<Hero>().HeroLevel++;
                        NetworkManager.Instance.SyncPlayerHero(heroes[i], SyncHeroMethod.HeroUpgrade);
                        #endregion

                        return CheckHeroLevelUp(sameLvHero);
                    }
                    sameLvHero = heroes[i];
                }
            }
            #endregion
            return _hero;
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
                //Use the last game status to switch the next game status
                switch (LastGameStatus)
                {
                    case GameStatus.Readying:
                        PeriodTime = MainGameManager.playingTime;
                        GameStatus = GameStatus.Playing;
                        break;
                    case GameStatus.Playing:
                        //If some players' heroes are not die yet, it will switch to extra time status
                        if (false/*If Player Has Hero Not Die (Not Finsih)*/)
                        {
                            PeriodTime = MainGameManager.compingTime;
                            GameStatus = GameStatus.Comping;
                        }
                        else
                        {
                            PeriodTime = MainGameManager.readyingTime;
                            GameStatus = GameStatus.Readying;
                            RoundManager.RoundUp();
                        }
                        break;
                    case GameStatus.Comping:
                        PeriodTime = MainGameManager.readyingTime;
                        GameStatus = GameStatus.Readying;
                        RoundManager.RoundUp();
                        break;
                }
            }
            //Change game status to Transiting
            else
            {
                LastGameStatus = GameStatus;
                PeriodTime = MainGameManager.transitionTime;
                GameStatus = GameStatus.Transiting;
            }

            remainTime = PeriodTime;
        }
    }
}
