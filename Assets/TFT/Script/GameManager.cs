using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TFT
{
    public class GameManager : MonoBehaviour
    {
        public int[] playerPosition;
        public PlayerHero[] PlayerHeroes;
        public GameObject[] PlayerArenas;
        public static PhotonView PhotonView;
        [Header("Player Personal Data")]
        public PlayerHero PlayerHero;
        public PlayerArena SelfPlayerArena;
        public int playerId, posId;

        #region Game Time Period
        public float PeriodTime;
        private float remainTime;
        public float RemainTime
        {
            get { return remainTime; }
            set
            {
                if(!PhotonNetwork.connected || PhotonNetwork.isMasterClient)
                {
                    if (value <= 0)
                    {
                        ChangeStatus();
                    }
                    else
                    {
                        remainTime = value;
                        PhotonView.RPC("RPC_SyncRemainTIme", PhotonTargets.Others, RemainTime);
                    }
                }
            }
        }
        #endregion

        public GameStatus LastGameStatus;
        private GameStatus gameStatus;
        public GameStatus GameStatus
        {
            get { return gameStatus; }
            set
            {
                gameStatus = value;
                if(value == GameStatus.Transiting && LastGameStatus == GameStatus.Readying)
                {
                    //Matching the component
                    return;
                }
                foreach (NetworkHero gbHero in PlayerHero.GameBoardHeroes)
                {
                    Hero modifyHero = GetPlayerHero(gbHero);
                    switch (value)
                    {
                        case GameStatus.Readying:
                        
                            //modifyHero.transform.parent = modifyHero.HeroPlace.transform;
                            //modifyHero.transform.localPosition = Vector3.zero;
                            //modifyHero.transform.eulerAngles = Vector3.zero;
                            modifyHero.HeroStatus = HeroStatus.Standby;
                            break;
                        case GameStatus.Playing:
                            //modifyHero.transform.parent = null;
                            modifyHero.HeroStatus = HeroStatus.Fight;
                            break;
                    }
                }

            }
        }
        public Main.GameManager MainGameManager;
        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            
            //PlayerArenas = GameObject.FindGameObjectsWithTag("PlayerArena");
            PlayerHero = new PlayerHero();

            PhotonNetworkSetup();

            PeriodTime = MainGameManager.readyingTime;
            RemainTime = PeriodTime;    
        }

        void FixedUpdate()
        {
            ////Loop player's heroes
            //for (int i = 0; i < PlayerHeroes.Length; i++)
            //{
            //    if (i == playerId) continue;
            //    for (int j = 0; j < playerPosition.Length; j++)
            //    {
            //        //Get the exact game arena position
            //        if(i == playerPosition[j])
            //        {
            //            GamePlace enemyArena = PlayerArenas[j].GetComponent<PlayerArena>().EnemyArena;
            //        }
            //    }
            //}
        }

        private void ChangeStatus()
        {
            if (GameStatus == GameStatus.Transiting)
            {
                switch (LastGameStatus)
                {
                    case GameStatus.Readying:
                        PeriodTime = MainGameManager.playingTime;
                        GameStatus = GameStatus.Playing;
                        break;
                    case GameStatus.Playing:
                        PeriodTime = MainGameManager.compingTime;
                        GameStatus = GameStatus.Comping;
                        break;
                    case GameStatus.Comping:
                        PeriodTime = MainGameManager.readyingTime;
                        GameStatus = GameStatus.Readying;
                        //change timer count from countdown to countup
                        break;
                }
            }
            else
            {
                LastGameStatus = GameStatus;
                PeriodTime = MainGameManager.transitionTime;
                GameStatus = GameStatus.Transiting;

            }
            PhotonView.RPC("RPC_SyncGameStatus", PhotonTargets.Others, GameStatus, PeriodTime);
            remainTime = PeriodTime;
        }

        private void PhotonNetworkSetup()
        { 
            PhotonView = GetComponent<PhotonView>();
            PlayerHeroes = new PlayerHero[PhotonNetwork.playerList.Length];
            Debug.Log("Network player count: " + PhotonNetwork.playerList.Length);
            if (PhotonNetwork.isMasterClient)
                SetupNetworkPlayer();
            
        }

        /// <summary>
        /// Rearrange player gameboard position, it is implemented by master client
        /// </summary>
        /// <returns></returns>
        public void SetupNetworkPlayer()
        {
            #region Setup All Player ID (Follow master client arrangement)
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                PhotonView.RPC("RPC_SetPlayerId", PhotonNetwork.playerList[i], i);
                Debug.Log("Player Name: " + PhotonNetwork.playerList[i].NickName + ", Id: " + i);
            }
            #endregion

            #region Set Player Random Position
            PhotonView.RPC("RPC_SetupPlayerPosition", PhotonTargets.All, GetRearrangeData(PhotonNetwork.playerList.Length));
            #endregion
        }

        /// <summary>
        /// Check whether player can buy a hero.
        /// </summary>
        /// <param name="_hero"></param>
        /// <returns></returns>
        public bool BuyHero(Hero _hero)
        {
            for(int i = 0; i < SelfPlayerArena.SelfArena.HeroList.childCount; i++)
            {
                //Check whether hero list empty
                if (SelfPlayerArena.SelfArena.HeroList.GetChild(i).childCount == 0)
                {
                    _hero.gameObject.transform.parent = SelfPlayerArena.SelfArena.HeroList.GetChild(i);
                    _hero.gameObject.transform.localPosition = Vector3.zero;
                    NetworkHero networkHero = CheckHeroLevelUp(new NetworkHero(_hero.name, i, _hero.HeroLevel));
                    Debug.Log("Position: " + i);
                    if (networkHero.HeroLevel == HeroLevel.Level1)
                    {
                        //PlayerHero.UsableHeroes.Add(networkHero);
                        PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All, posId,
                            playerId, networkHero.name, networkHero.position, networkHero.HeroLevel, SyncHeroMethod.AddHero);
                    }
                    else
                    {
                        Debug.Log("Level Up And Destroy");
                        DestroyImmediate(_hero.gameObject);
                    }
                    return true;
                }
            }
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
            if (_hero.HeroLevel == HeroLevel.Level3)
                return _hero;
            NetworkHero[] heroes = PlayerHero.UsableHeroes.ToArray();
            int sameLvCount = 0;
            NetworkHero sameLvHero = _hero;
            for(int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i].name.Equals(_hero.name) && heroes[i].HeroLevel == _hero.HeroLevel)
                {
                    sameLvCount++;
                    if (sameLvCount >= 2)
                    {
                        //DestroyImmediate(heroes[i].gameObject);
                        #region Remove Hero
                        DestroyImmediate(SelfPlayerArena.SelfArena.HeroList.GetChild(heroes[i].position).GetChild(0).gameObject);
                        PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All, posId,
                            playerId, heroes[i].name, heroes[i].position, heroes[i].HeroLevel, SyncHeroMethod.RemoveHero);
                        #endregion

                        #region Upgrade Hero
                        sameLvHero.HeroLevel++;
                        SelfPlayerArena.SelfArena.HeroList.GetChild(sameLvHero.position).GetChild(0).GetComponent<Hero>().HeroLevel++;
                        PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All, posId,
                            playerId, sameLvHero.name, sameLvHero.position, sameLvHero.HeroLevel, SyncHeroMethod.HeroUpgrade);
                        #endregion

                        return CheckHeroLevelUp(sameLvHero);
                    }
                    sameLvHero = heroes[i];
                }
            }
            return _hero;
        }

        /// <summary>
        /// Put or take hero from the gameboard
        /// </summary>
        /// <param name="hero"></param>
        public void ChangeHeroPos(ref Hero _hero)
        {
            Debug.Log("Last Position: [" + _hero.LastHeroPlace.PlaceId + "], current position [" + _hero.HeroPlace.PlaceId + "].");
            SyncMoveHero moveHeroMethod;
            if (_hero.HeroPlace.gameObject.name.Equals(_hero.LastHeroPlace.gameObject.name))
            {
                moveHeroMethod = SyncMoveHero.MoveHero;
            }
            else if (_hero.HeroPlace.gameObject.name.Equals("Square"))
            {
                moveHeroMethod = SyncMoveHero.RemoveGameboard;
            }
            else
            {
                moveHeroMethod = SyncMoveHero.AddGameboard;
            }
            PhotonView.RPC("RPC_SyncPlayerHeroPlace", PhotonTargets.All, posId,
                playerId, _hero.name, _hero.LastHeroPlace.PlaceId, _hero.HeroLevel, _hero.HeroPlace.PlaceId, moveHeroMethod);
        }

        /// <summary>
        /// Compile the NetworkHero to Hero from PlayHero and GameBoard
        /// </summary>
        /// <param name="_networkHero"></param>
        /// <returns></returns>
        public Hero GetPlayerHero(NetworkHero _networkHero)
        {
            return SelfPlayerArena.SelfArena.GameBoard.GetChild(_networkHero.position).GetComponent<Hero>();
        }

        public void SetPlayerComponent()
        {
            //Get the surival players list
            int[] component = GetRearrangeData(PhotonNetwork.playerList.Length);
        }

        /// <summary>
        /// Rearrange integer array data
        /// </summary>
        /// <param name="_length"></param>
        /// <returns></returns>
        public int[] GetRearrangeData(int _length)
        {
            int[] data = new int[_length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }
            System.Random r = new System.Random(System.DateTime.Now.Millisecond);
            return data.OrderBy(x => r.Next()).ToArray();
        }

        #region PunRPC

        #region Start
        /// <summary>
        ///Setting Lobby PhotonPlayer Array position,call by master client
        /// </summary>
        /// <param name="_id"></param>
        [PunRPC]
        public void RPC_SetPlayerId(int _id)
        {
            playerId = _id;
        }
        
        /// <summary>
        ///Setting Lobby Player Arena Randam Positon,call by master client
        /// </summary>
        /// <param name="_playerPosition"></param>
        [PunRPC]
        public void RPC_SetupPlayerPosition(int[] _playerPosition)
        {
            playerPosition = _playerPosition;
            for (int i = 0; i < _playerPosition.Length; i++)
            {
                PlayerHeroes[i] = new PlayerHero();
                if (_playerPosition[i] == playerId)
                {
                    SelfPlayerArena = PlayerArenas[i].GetComponent<PlayerArena>();
                    posId = i;
                    //PlayerHeroes[playerId] = PlayerHero;
                }
            }
        }

        #endregion

        [PunRPC]
        public void RPC_SyncGameStatus(GameStatus _gameStatus, float _peroidTime)
        {
            GameStatus = _gameStatus;
            PeriodTime = _peroidTime;
            remainTime = PeriodTime;
        }

        [PunRPC]
        public void RPC_SyncRemainTIme(float _remainTime)
        {
            remainTime = _remainTime;
        }

        /// <summary>
        /// Setting all player's PlayerHeroes[] and remote player's GameArena Hero position, when player put or take hero form gameboard and herolist
        /// </summary>
        /// <param name="_posId"></param>
        /// <param name="_playerId"></param>
        /// <param name="_name"></param>
        /// <param name="_heroPos"></param>
        /// <param name="_heroLevel"></param>
        /// <param name="_newPos"></param>
        [PunRPC]
        public void RPC_SyncPlayerHeroPlace(int _posId, int _playerId, string _name, int _heroPos, HeroLevel _heroLevel, int _newPos, SyncMoveHero _syncMoveHero)
        {
            Debug.Log("Sync Move Hero Method: " + _syncMoveHero.ToString());
            List<NetworkHero> ChangedHero = PlayerHeroes[_playerId].UsableHeroes;
            GamePlace enemyArena = PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena;
            Debug.Log("Player [" + _playerId + "] move the hero [" + _name + "] to place [" + _newPos + "]");
            //The loop is used for finding the network hero which satisfied the passing value.
            for (int i = 0; i < ChangedHero.Count; i++)
            {
                //Debug.Log("Hero: "+ _name + ", id: "+ i);
                //Debug.Log("name: " + ChangedHero[i].name.Equals(_name));
                //Debug.Log("position: " + (ChangedHero[i].position == _heroPos)+ " " + ChangedHero[i].position + ", " + _heroPos);
                //Debug.Log("HeroLevel: " + (ChangedHero[i].HeroLevel == _heroLevel));

                if (ChangedHero[i].name.Equals(_name) &&
                    ChangedHero[i].position == _heroPos &&
                    ChangedHero[i].HeroLevel == _heroLevel)
                {
                    int lastPos = ChangedHero[i].position;
                    switch (_syncMoveHero)
                    {
                        case SyncMoveHero.AddGameboard:
                            ChangedHero[i].position = _newPos;
                            PlayerHeroes[_playerId].GameboardAddHero(ChangedHero[i]);
                            //Remote Player Game Arena Update
                            if (_playerId != playerId)
                            {
                                //Check Selected Place Whether Null to Prevent Null Reference Exception
                                if (enemyArena.HeroList.GetChild(_heroPos).childCount != 0)
                                {
                                    Debug.Log("Move the hero to Gameboard");
                                    enemyArena.HeroList.GetChild(_heroPos).GetChild(0).parent = enemyArena.GameBoard.GetChild(_newPos);

                                    Hero _newHeroPlacement = enemyArena.GameBoard.GetChild(_newPos).GetChild(0).GetComponent<Hero>();
                                    _newHeroPlacement.transform.localPosition = Vector3.zero;
                                    _newHeroPlacement.LastHeroPlace = _newHeroPlacement.HeroPlace;
                                    _newHeroPlacement.HeroPlace = _newHeroPlacement.transform.parent.GetComponent<HeroPlace>();
   
                                }
                            }
                            break;
                        case SyncMoveHero.RemoveGameboard:
                            PlayerHeroes[_playerId].GameboardRemoveHero(ChangedHero[i]);
                            ChangedHero[i].position = _newPos;
                            //Remote Player Game Arena Update
                            if (_playerId != playerId)
                            {
                                //Check Selected Place Whether Null to Prevent Null Reference Exception
                                if (enemyArena.GameBoard.GetChild(_heroPos).childCount != 0)
                                {
                                    Debug.Log("Move away the hero from Gameboard");
                                    enemyArena.GameBoard.GetChild(_heroPos).GetChild(0).parent = enemyArena.HeroList.GetChild(_newPos);

                                    Hero _newHeroPlacement = enemyArena.HeroList.GetChild(_newPos).GetChild(0).GetComponent<Hero>();
                                    _newHeroPlacement.transform.localPosition = Vector3.zero;
                                    _newHeroPlacement.LastHeroPlace = _newHeroPlacement.HeroPlace;
                                    _newHeroPlacement.HeroPlace = _newHeroPlacement.transform.parent.GetComponent<HeroPlace>();
                                }
                            }
                            break;
                        case SyncMoveHero.MoveHero:
                            PlayerHeroes[_playerId].MoveHero(ChangedHero[i], _newPos);
                            //Remote Player Game Arena Update
                            if (_playerId != playerId)
                            {
                                Transform _heroPlacement;
                                //Set the Hero's Placement
                                if (PlayerHeroes[_playerId].GameBoardHeroes.Contains(ChangedHero[i]))
                                {
                                    //Hero is in gameboard
                                    _heroPlacement = enemyArena.GameBoard;
                                    Debug.Log("Moved the hero in Gameboard");
                                }
                                else
                                {
                                    //Hero is not in gameboard
                                    _heroPlacement = enemyArena.HeroList;
                                    Debug.Log("Moved the hero in HeroList");
                                }
                                //Check Selected Place Whether Null to Prevent Null Reference Exception
                                if (_heroPlacement.childCount != 0)
                                {
                                    _heroPlacement.GetChild(_heroPos).GetChild(0).parent = _heroPlacement.GetChild(_newPos);

                                    Hero _newHeroPlacement = _heroPlacement.GetChild(_newPos).GetChild(0).GetComponent<Hero>();
                                    _newHeroPlacement.transform.localPosition = Vector3.zero;
                                      _newHeroPlacement.LastHeroPlace = _newHeroPlacement.HeroPlace;
                                      _newHeroPlacement.HeroPlace = _newHeroPlacement.transform.parent.GetComponent<HeroPlace>();
                                    //_newHeroPlacement.moveToThePlace(_newHeroPlacement, _newHeroPlacement.transform.parent.GetComponent<HeroPlace>());
                                }
                            }
                            break;
                    }
                    break;
                }
            }
            PlayerHero = PlayerHeroes[playerId];
        }

        /// <summary>
        /// Setting all player's PlayerHeroes[] and remote player's GameArena Hero position, when player buy hero to herolist
        /// </summary>
        /// <param name="_posId"></param>
        /// <param name="_playerId"></param>
        /// <param name="_name"></param>
        /// <param name="_heroPos"></param>
        /// <param name="_heroLevel"></param>
        /// <param name="_syncMethod"></param>
        [PunRPC]
        public void RPC_SyncPlayerHeroes(int _posId, int _playerId, string _name, int _heroPos, HeroLevel _heroLevel, SyncHeroMethod _syncMethod)
        {
            switch (_syncMethod)
            {
                case SyncHeroMethod.AddHero:
                    Debug.Log("Player [" + _playerId + "] add hero.");
                    PlayerHeroes[_playerId].UsableHeroes.Add(new NetworkHero(_name, _heroPos, _heroLevel));
                    if (_playerId != playerId)
                    {
                        #region Instantiate remote hero object
                        //Loop hero type
                        for (int i = 0; i < MainGameManager.heroTypes.Count; i++)
                        {
                            if (MainGameManager.heroTypes[i].name.Equals(_name))
                            {
                                Debug.Log("Arena [" + _posId + "] added hero in HeroList [" + _heroPos + "].");
                                Transform transformParent = PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.HeroList.GetChild(_heroPos);
                                Hero remoteHero = (Instantiate(MainGameManager.heroTypes[i].gameObject) as GameObject).GetComponent<Hero>();
                                remoteHero.name = MainGameManager.heroTypes[i].name;
                                remoteHero.transform.parent = transformParent;
                                //GameObject remoteHero = Instantiate(MainGameManager.heroTypes[i].gameObject, transformParent);
                                remoteHero.gameObject.transform.localPosition = Vector3.zero;
                                break;
                            }
                        }
                        #endregion
                    }
                    break;
                case SyncHeroMethod.RemoveHero:
                    List<NetworkHero> RemoveHero = PlayerHeroes[_playerId].UsableHeroes;
                    for (int i = 0; i < RemoveHero.Count; i++)
                    {
                        if (RemoveHero[i].name.Equals(_name) &&
                            RemoveHero[i].position == _heroPos &&
                            RemoveHero[i].HeroLevel == _heroLevel)
                        {
                            RemoveHero.Remove(RemoveHero[i]);
                            break;
                        }
                    }
                    if (_playerId != playerId)
                    {
                        #region Destroy remote hero object
                        DestroyImmediate(PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.HeroList.
                            GetChild(_heroPos).GetChild(0).gameObject);
                        #endregion
                    }
                    break;
                case SyncHeroMethod.HeroUpgrade:
                    List<NetworkHero> upgradeUsableHero = PlayerHeroes[_playerId].UsableHeroes;
                    for (int i = 0; i < upgradeUsableHero.Count; i++)
                    {
                        if (upgradeUsableHero[i].name.Equals(_name) &&
                            upgradeUsableHero[i].position == _heroPos &&
                            upgradeUsableHero[i].HeroLevel == _heroLevel - 1)
                        {
                            upgradeUsableHero[i].HeroLevel = _heroLevel;
                            break;
                        }
                    }
                    if (_playerId != playerId)
                    {
                        #region Updrage remote hero object
                        PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.HeroList.
                            GetChild(_heroPos).GetChild(0).GetComponent<Hero>().HeroLevel = _heroLevel;
                        #endregion
                    }
                    break;
            }
            PlayerHero = PlayerHeroes[playerId];
        }

        #endregion
    }
}

