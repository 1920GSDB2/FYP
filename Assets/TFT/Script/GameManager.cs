using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TFT
{
    public class GameManager : MonoBehaviour
    {
        public int[] playerPointer;
        public PlayerHero[] PlayerHeroes;
        public GameObject[] PlayerArenas;
        public PhotonView PhotonView;
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
                gameStatus = value;
                switch (value)
                {
                    case GameStatus.Setup:
                        foreach (Hero gbHero in PlayerHero.GameBoardHeros)
                        {
                            gbHero.transform.parent = gbHero.HeroPlace.transform;
                            gbHero.transform.localPosition = Vector3.zero;
                            gbHero.transform.eulerAngles = Vector3.zero;
                            gbHero.HeroStatus = HeroStatus.Standby;
                        }
                        break;
                    case GameStatus.Playing:
                        foreach (Hero gbHero in PlayerHero.GameBoardHeros)
                        {
                            gbHero.transform.parent = null;
                            gbHero.HeroStatus = HeroStatus.Fight;
                        }
                        break;
                }
            }
        }
        public static GameManager Instance;


        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            PlayerArenas = GameObject.FindGameObjectsWithTag("PlayerArena");
            PlayerHero = new PlayerHero();

            PhotonNetworkSetup();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void PhotonNetworkSetup()
        {
            //PhotonView = LobbyManager.PhotonView;
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.player == PhotonNetwork.playerList[i])
                {
                    playerId = i;
                    break;
                }
            }
            PlayerHeroes = new PlayerHero[PhotonNetwork.playerList.Length];
            if (PhotonNetwork.isMasterClient)
                SetupPlayerPointer();
            
        }

        /// <summary>
        /// Rearrange player gameboard position, it is implemented by master client
        /// </summary>
        /// <returns></returns>
        public void SetupPlayerPointer()
        {
            int[] data = new int[PlayerArenas.Length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }
            System.Random r = new System.Random(System.DateTime.Now.Millisecond);
            data = data.OrderBy(x => r.Next()).ToArray();
            
            PhotonNetwork.RPC(PhotonView, "RPC_SetupPlayerPointer", PhotonTargets.All, false, data);
        }

        /// <summary>
        /// Check whether player can buy a hero.
        /// </summary>
        /// <param name="_hero"></param>
        /// <returns></returns>
        public bool BuyHero(Hero _hero)
        {
            foreach (Transform child in SelfPlayerArena.SelfArena.HeroList)
            {
                //Check whether hero list empty
                if (child.childCount == 0)
                {
                    PlayerHero.UsableHeros.Add(CheckHeroLevelUp(_hero));
                    PhotonNetwork.RPC(PhotonView, "RPC_SyncPlayerHeroes", PhotonTargets.Others, true, PlayerHero, playerId);
                    _hero.gameObject.transform.parent = child;
                    _hero.gameObject.transform.localPosition = Vector3.zero;
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
        public Hero CheckHeroLevelUp(Hero _hero)
        {
            if (_hero.HeroLevel == HeroLevel.Level3) return _hero;
            Hero[] heroes = PlayerHero.UsableHeros.ToArray();
            int sameLvCount = 0;
            Hero sameLvHero = new Hero();
            for(int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i].name.Equals(_hero.name) && heroes[i].HeroLevel == _hero.HeroLevel)
                {
                    sameLvCount++;
                    if (sameLvCount >= 2)
                    {
                        DestroyImmediate(heroes[i].gameObject);
                        sameLvHero.HeroLevel++;
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
            if (PlayerHero.GameBoardHeros.Contains(hero))
            {
                PlayerHero.GameboardRemoveHero(ref hero);
            }
            else
            {
                PlayerHero.GameboardAddHero(ref hero);
            }
            PhotonNetwork.RPC(PhotonView, "RPC_SyncPlayerHeroes", PhotonTargets.All, true, PlayerHero, playerId);
        }

        [PunRPC]
        public void RPC_SetupPlayerPointer(int[] _playerPointer)
        {
            playerPointer = _playerPointer;
            for (int i = 0; i < _playerPointer.Length; i++)
            {
                if (i == playerId)
                {
                    SelfPlayerArena = PlayerArenas[i].GetComponent<PlayerArena>();
                    PlayerHeroes[playerId] = PlayerHero;
                    PhotonNetwork.RPC(PhotonView, "RPC_SyncPlayerHeroes", PhotonTargets.Others, true, PlayerHero, playerId);
                }
            }
        }

        [PunRPC]
        public void RPC_SyncPlayerHeroes(PlayerHero _playerHero, int _playerId)
        {
            PlayerHeroes[_playerId] = _playerHero;
        }
        
    }
}

