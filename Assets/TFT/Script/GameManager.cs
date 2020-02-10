using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TFT
{
    public enum SyncHeroMethod
    {
        AddHero,
        RemoveHero,
        HeroUpgrade
    }

    public class GameManager : MonoBehaviour
    {
        public int[] playerPosition;
        public PlayerHero[] PlayerHeroes;
        public GameObject[] PlayerArenas;
        public static PhotonView PhotonView;
        [Header("Player Personal Data")]
        public PlayerHero PlayerHero;
        public PlayerArena SelfPlayerArena;
        public int playerId;
        private GameStatus gameStatus;
        public GameStatus GameStatus
        {
            get { return gameStatus; }
            set
            {
                //gameStatus = value;
                //switch (value)
                //{
                //    case GameStatus.Setup:
                //        foreach (Hero gbHero in PlayerHero.GameBoardHeros)
                //        {
                //            gbHero.transform.parent = gbHero.HeroPlace.transform;
                //            gbHero.transform.localPosition = Vector3.zero;
                //            gbHero.transform.eulerAngles = Vector3.zero;
                //            gbHero.HeroStatus = HeroStatus.Standby;
                //        }
                //        break;
                //    case GameStatus.Playing:
                //        foreach (Hero gbHero in PlayerHero.GameBoardHeros)
                //        {
                //            gbHero.transform.parent = null;
                //            gbHero.HeroStatus = HeroStatus.Fight;
                //        }
                //        break;
                //}
            }
        }
        public Main.GameManager MainGameManager;
        public static GameManager Instance;
        

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            //PlayerArenas = GameObject.FindGameObjectsWithTag("PlayerArena");
            PlayerHero = new PlayerHero();

            PhotonNetworkSetup();
        }

        // Update is called once per frame
        void Update()
        {

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
            int[] data = new int[PhotonNetwork.playerList.Length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }
            System.Random r = new System.Random(System.DateTime.Now.Millisecond);
            data = data.OrderBy(x => r.Next()).ToArray();
            PhotonView.RPC("RPC_SetupPlayerPosition", PhotonTargets.All, data);
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
                        PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All,
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
                        PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All,
                            playerId, heroes[i].name, heroes[i].position, heroes[i].HeroLevel, SyncHeroMethod.RemoveHero);
                        #endregion

                        #region Upgrade Hero
                        sameLvHero.HeroLevel++;
                        SelfPlayerArena.SelfArena.HeroList.GetChild(sameLvHero.position).GetChild(0).GetComponent<Hero>().HeroLevel++;
                        PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All,
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
        public void ChangeHeroPos(ref Hero hero)
        {
            //if (PlayerHero.GameBoardHeroes.Contains(hero))
            //{
            //    PlayerHero.GameboardRemoveHero(ref hero);
            //}
            //else
            //{
            //    PlayerHero.GameboardAddHero(ref hero);
            //}
            //PhotonNetwork.RPC(PhotonView, "RPC_SyncPlayerHeroes", PhotonTargets.All, true, PlayerHero, playerId);
        }

        [PunRPC]
        public void RPC_SetPlayerId(int _id)
        {
            playerId = _id;
        }

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
                    //PlayerHeroes[playerId] = PlayerHero;
                }
            }
        }

        [PunRPC]
        public void RPC_SyncPlayerHeroes(int _playerId, string _name, int _heroPos, HeroLevel _heroLevel, SyncHeroMethod _syncMethod)
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
                                //Loop player arena
                                for (int j =0; j < playerPosition.Length; j++)
                                {
                                    if (playerPosition[j] == _playerId)
                                    {
                                        Debug.Log("Arena [" + j + "] added hero in HeroList [" + _heroPos + "].");
                                        Transform transformParent = PlayerArenas[j].GetComponent<PlayerArena>().EnemyArena.HeroList.GetChild(_heroPos);
                                        Hero remoteHero = (Instantiate(MainGameManager.heroTypes[i].gameObject) as GameObject).GetComponent<Hero>();
                                        remoteHero.name = MainGameManager.heroTypes[i].name;
                                        remoteHero.transform.parent = transformParent;
                                        //GameObject remoteHero = Instantiate(MainGameManager.heroTypes[i].gameObject, transformParent);
                                        remoteHero.gameObject.transform.localPosition = Vector3.zero;
                                    }
                                }
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
                        for (int j = 0; j < playerPosition.Length; j++)
                        {
                            if (playerPosition[j] == _playerId)
                            {
                                DestroyImmediate(PlayerArenas[j].GetComponent<PlayerArena>().EnemyArena.HeroList.
                                    GetChild(_heroPos).GetChild(0).gameObject);
                            }
                        }
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
                        for (int j = 0; j < playerPosition.Length; j++)
                        {
                            if (playerPosition[j] == _playerId)
                            {
                                PlayerArenas[j].GetComponent<PlayerArena>().EnemyArena.HeroList.
                                    GetChild(_heroPos).GetChild(0).GetComponent<Hero>().HeroLevel = _heroLevel;
                            }
                        }
                        #endregion
                    }
                    break;
            }
            PlayerHero = PlayerHeroes[playerId];
        }
        
    }
}

