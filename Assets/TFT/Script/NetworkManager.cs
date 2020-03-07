using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TFT
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;
        public static PhotonView PhotonView;

        public int playerId, posId;                 //Player Id and Position Id for Networking
        public Opponent opponent;                   //Player's Opponent

        public int[] PlayerPosition;                //Player's Position Represent to PlayerArenas
        public PlayerHero[] PlayerHeroes;           //Players' Heroes List
        public GameObject[] PlayerArenas;           //PlayerArenas Location (The Data are hard coded in scene)
        public OpponentManager[] OpponentManagers;  //Player's Opponent Data
        public Camera[] Cameras;                    //Cameras of Focusing Game Arena (The Data are hard coded in scene)
      
        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            PhotonNetworkSetup();
        }
        
        void Update()
        {

            #region PhotonNetwork Debugging
            if (Input.GetKeyDown(KeyCode.L))
            {
                PhotonView.RPC("RPC_testHeroes", PhotonTargets.All);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                PhotonView.RPC("RPC_Battle", PhotonTargets.All, 0, 1);
            }
            #endregion

        }
        
        #region Photon Network Setup
        /// <summary>
        /// Photon Connection Setup, It will be called by Start
        /// </summary>
        private void PhotonNetworkSetup()
        {
            PhotonView = GetComponent<PhotonView>();

            PlayerHeroes = new PlayerHero[PhotonNetwork.playerList.Length];

            if (PhotonNetwork.isMasterClient)
                SetupNetworkPlayer();
        }

        /// <summary>
        /// Rearrange player gameboard position, it is implemented by master client
        /// </summary>
        /// <returns></returns>
        private void SetupNetworkPlayer()
        {
            #region Setup All Player ID (Follow master client arrangement)
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                PhotonView.RPC("RPC_SetPlayerId", PhotonNetwork.playerList[i], i);
                Debug.Log("Player Name: " + PhotonNetwork.playerList[i].NickName + ", Id: " + i);
            }
            #endregion

            //Set Player Random Position
            PhotonView.RPC("RPC_SetupPlayerPosition", PhotonTargets.All, GetRearrangeData(PhotonNetwork.playerList.Length));
        }

        #endregion

        #region Sync Hero Method
        /// <summary>
        /// Sync Player Hero to Other Players
        /// </summary>
        /// <param name="_syncHero"></param>
        /// <param name="_syncHeroMethod"></param>
        public void SyncPlayerHero(NetworkHero _syncHero, SyncHeroMethod _syncHeroMethod)
        {
            PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All, posId, 
                playerId, _syncHero.name, _syncHero.position, _syncHero.HeroLevel, _syncHeroMethod);
        }

        /// <summary>
        /// Sync Player Hero Position to Other Players
        /// </summary>
        /// <param name="_hero"></param>
        /// <param name="_syncMoveHero"></param>
        public void SyncPlayerHeroPlace(Hero _hero, SyncMoveHero _syncMoveHero)
        {
            PhotonView.RPC("RPC_SyncPlayerHeroPlace", PhotonTargets.All, posId,
                playerId, _hero.name, _hero.LastHeroPlace.PlaceId, _hero.HeroLevel, _hero.HeroPlace.PlaceId, _syncMoveHero);
        }
        #endregion

        #region Get Hero Place
        /// <summary>
        /// Get Gameboard Hero place by Id
        /// </summary>
        /// <param name="_posId"></param>
        /// <param name="_placeId"></param>
        /// <returns></returns>
        public HeroPlace GetGameboardHeroPlace(int _posId, int _placeId)
        {
            if (posId == _posId)
            {
                Debug.Log("Hero is me");
                return GameManager.Instance.SelfPlayerArena.SelfArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
            }
            else
            {
                Debug.Log("Hero is not me");
                return PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
            }
        }

        /// <summary>
        /// Get HeroList Hero Place by Id
        /// </summary>
        /// <param name="posId"></param>
        /// <param name="placeId"></param>
        /// <returns></returns>
        public HeroPlace GetHeroListHeroPlace(int posId, int placeId)
        {
            if (this.posId == posId)
                return GameManager.Instance.SelfPlayerArena.SelfArena.HeroList.GetChild(placeId).GetComponent<HeroPlace>();
            else
                return PlayerArenas[posId].GetComponent<PlayerArena>().EnemyArena.HeroList.GetChild(placeId).GetComponent<HeroPlace>();
        }

        /// <summary>
        /// Get Player's Hero Place by Id
        /// </summary>
        /// <param name="_posId"></param>
        /// <param name="_placeId"></param>
        /// <param name="_isEnemy"></param>
        /// <returns></returns>
        public HeroPlace GetPlayerHeroPlace(int _posId, int _placeId)
        {
            if (this.posId== _posId)
                return PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
            else               
                return PlayerArenas[_posId].GetComponent<PlayerArena>().SelfArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
        }
        #endregion

        #region Set Player Opponent
        /// <summary>
        /// Match Player Opponent, Called by Master Client
        /// </summary>
        public void MatchPlayerOpponent()
        {
            //Get the surival players list
            int[] opponentResult = GetRearrangeData(PhotonNetwork.playerList.Length);
            PhotonView.RPC("RPC_SyncPlayersOpponent", PhotonTargets.All, opponentResult);
        }

        /// <summary>
        /// Setting Player Opponent, Called by PunRPC
        /// </summary>
        /// <param name="matchResult"></param>
        private void SetPlayerOpponent(int[] matchResult)
        {
            if (matchResult.Length % 2 != 0)
            {
                OpponentManagers = new OpponentManager[matchResult.Length / 2 + 1];
                OpponentManagers[OpponentManagers.Length - 1] = new OpponentManager(matchResult[matchResult.Length / 2], matchResult[0], true);
                if (matchResult[matchResult.Length / 2] == playerId)
                {
                    opponent = new Opponent(matchResult[0], true);
                }
            }
            else
            {
                OpponentManagers = new OpponentManager[matchResult.Length / 2];
            }

            for (int i = 0; i < matchResult.Length / 2; i++)
            {
                OpponentManagers[i] = new OpponentManager(matchResult[i], matchResult[matchResult.Length - 1 - i]);
                Debug.Log("Player " + matchResult[i] + " vs  Player" + matchResult[matchResult.Length - 1 - i]);
                if (matchResult[i] == playerId)
                {
                    opponent = new Opponent(matchResult[matchResult.Length - 1 - i]);
                }
                else if (matchResult[matchResult.Length - 1 - i] == playerId)
                {
                    opponent = new Opponent(matchResult[i]);
                }
            }
        }

        #endregion

        /// <summary>
        /// Rearrange integer array data
        /// </summary>
        /// <param name="_length"></param>
        /// <returns></returns>
        private int[] GetRearrangeData(int _length)
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

        #region Start Photon Method, Called by Master Client

        /// <summary>
        /// Setting Lobby PhotonPlayer Array Position
        /// </summary>
        /// <param name="_id"></param>
        [PunRPC]
        public void RPC_SetPlayerId(int _id)
        {
            playerId = _id;
        }

        /// <summary>
        /// Setting Lobby Player Arena Randam Positon
        /// </summary>
        /// <param name="_playerPosition"></param>
        [PunRPC]
        public void RPC_SetupPlayerPosition(int[] _playerPosition)
        {
            PlayerPosition = _playerPosition;
            for (int i = 0; i < _playerPosition.Length; i++)
            {
                PlayerHeroes[i] = new PlayerHero();
                if (_playerPosition[i] == playerId)
                {
                    GameManager.Instance.SelfPlayerArena = PlayerArenas[i].GetComponent<PlayerArena>();
                    
                    posId = i;
                    Debug.Log("plaer ID " + playerId + " I " + i + " "
                        + PlayerHeroes.Length + " Hero " + PlayerHeroes[playerId]);
                    PlayerHeroes[playerId] = new PlayerHero
                    {
                        posId = posId
                    };

                    //Cameras[posId].enabled = true;
                    GameManager.Instance.SelfPlayerArena.Camera.SetActive(true);
                    Debug.Log("Camera " + posId + " Open");
                    //PlayerHeroes[playerId] = PlayerHero;
                }
            }
        }

        #endregion

        /// <summary>
        /// Setting All Players Opponent
        /// </summary>
        /// <param name="_matchResult"></param>
        [PunRPC]
        public void RPC_SyncPlayersOpponent(int[] _matchResult)
        {
            SetPlayerOpponent(_matchResult);
        }

        #region Sync Player Hero
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
                                    #region For PhotoNetwork.instatiate
                                    //      Hero _newHeroPlacement = enemyArena.HeroList.GetChild(_heroPos).GetChild(0).GetComponent<Hero>();
                                    //     _newHeroPlacement.GetComponent<PhotonView>().RPC("RPC_AddToGameBoard", PhotonTargets.All, _posId, _newPos);
                                    #endregion

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
                                    #region For PhotonNetwork.instatiate
                                    //   Hero _newHeroPlacement = enemyArena.GameBoard.GetChild(_heroPos).GetChild(0).GetComponent<Hero>();
                                    //   _newHeroPlacement.GetComponent<PhotonView>().RPC("RPC_AddToGameBoard", PhotonTargets.All, _posId, _newPos);
                                    #endregion

                                }
                                else
                                {
                                    //Hero is not in gameboard
                                    _heroPlacement = enemyArena.HeroList;
                                    Debug.Log("Moved the hero in HeroList");
                                    #region For PhotonNetwork.instatiate
                                    //   Hero _newHeroPlacement = enemyArena.HeroList.GetChild(_heroPos).GetChild(0).GetComponent<Hero>();
                                    // _newHeroPlacement.GetComponent<PhotonView>().RPC("RPC_AddToHeroList", PhotonTargets.All, _posId, _newPos);
                                    #endregion

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
            GameManager.Instance.PlayerHero = PlayerHeroes[playerId];
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
                    //Debug.Log("Player [" + _playerId + "] add hero.");
                    PlayerHeroes[_playerId].UsableHeroes.Add(new NetworkHero(_name, _heroPos, _heroLevel));
                    /*   if (_playerId != playerId)
                       {
                           #region Instantiate remote hero object
                           //Loop hero type
                           for (int i = 0; i < MainGameManager.heroTypes.Count; i++)
                           {
                               if (MainGameManager.heroTypes[i].name.Equals(_name))
                               {

                                   Debug.Log("Arena [" + _posId + "] added hero in HeroList [" + _heroPos + "].");
                                   Transform transformParent = PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.HeroList.GetChild(_heroPos);
                                //   Hero remoteHero = (Instantiate(MainGameManager.heroTypes[i].gameObject) as GameObject).GetComponent<Hero>();
                                 //  remoteHero.name = MainGameManager.heroTypes[i].name;
                                 //  remoteHero.transform.parent = transformParent;
                                   //GameObject remoteHero = Instantiate(MainGameManager.heroTypes[i].gameObject, transformParent);
                                   remoteHero.gameObject.transform.localPosition = Vector3.zero;
                                   break;

                               }
                           }
                           #endregion
                       }*/
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
            GameManager.Instance.PlayerHero = PlayerHeroes[playerId];
        }

        #endregion

        [PunRPC]
        void RPC_Battle(int player1Id, int player2Id)
        {
            if (playerId == player2Id)
            {
                PlayerArenas[player1Id].GetComponent<PlayerArena>().Camera.SetActive(true);
                PlayerArenas[player2Id].GetComponent<PlayerArena>().Camera.SetActive(false);

                foreach (NetworkHero networkHero in PlayerHeroes[player2Id].GameBoardHeroes)
                {
                    Hero heroObject = GameManager.Instance.SelfPlayerArena.SelfArena.GameBoard.GetChild(networkHero.position).GetChild(0).GetComponent<Hero>();


                    heroObject.GetComponent<PhotonView>().RPC("RPC_MoveToThePlayerHeroPlace", PhotonTargets.All, PlayerHeroes[player1Id].posId, networkHero.position);
                }
            }

        }

        [PunRPC]
        public void RPC_testHeroes()
        {
            Hero hero;
            if (PhotonView.isMine)
            {
                Debug.Log("me SPAWN");
                hero = (PhotonNetwork.Instantiate(Path.Combine("Prefabs", "God of Wizard"), transform.position, Quaternion.identity, 0)).GetComponent<Hero>();
                // hero.GetComponent<PhotonView>().RPC("RPC_MoveToTheHeroPlace", PhotonTargets.All, posId, place[0].PlaceId);
                //  hero.transform.parent = SelfPlayerArena.SelfArena.GameBoard.GetChild(place[0].PlaceId).transform;               
                //  hero.transform.localPosition = Vector3.forward;
                //  hero.HeroPlace = place[0];
            }
            else
            {
                Debug.Log("other spawm");
                hero = (PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Lonnie"), transform.position, Quaternion.identity, 0)).GetComponent<Hero>();
                // hero.GetComponent<PhotonView>().RPC("RPC_MoveToTheHeroPlace", PhotonTargets.All, posId, place[1].PlaceId);
                //   hero.transform.parent = SelfPlayerArena.SelfArena.GameBoard.GetChild(place[1].PlaceId).transform;
                //   hero.transform.localPosition = Vector3.zero;
                //  hero.HeroPlace = place[1];
            }
            //  hero.transform.parent = SelfPlayerArena.SelfArena.GameBoard.GetChild(place[0].PlaceId).transform;

            PlayerHeroes[playerId].GameboardAddHero(new NetworkHero(hero));
        }

        #endregion
    }
}