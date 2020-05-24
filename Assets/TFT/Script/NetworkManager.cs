using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

namespace TFT
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;
        public static PhotonView PhotonView;
        
        public string[] PlayersId;
        public string[] PlayerName;
        //public Dictionary<string, string> PlayersName = new Dictionary<string, string>;
        public int playerId;
        [SerializeField]
        private int posId;
        public int PosId
        {
            get { return posId; }
            set
            {
                posId = value;
                string playerName = PhotonNetwork.player.CustomProperties["NAME"].ToString();
                PhotonNetwork.player.SetCustomProperties(new Hashtable
                {
                    {"NAME", playerName},
                    {"READY_FOR_START", true},
                    {"FOCUSING", PosId },
                    {"ISHOST", true },
                    {"POSITION", PosId },
                });
            }
        }
        [SerializeField]
        private int battlePosId;                 //Id for Networking
        public int BattlePosId
        {
            get { return battlePosId; }
            set
            {
                battlePosId = value;
                if(BattlePosId == PosId)
                {
                    string playerName = PhotonNetwork.player.CustomProperties["NAME"].ToString();
                    PhotonNetwork.player.SetCustomProperties(new Hashtable
                    {
                        {"NAME", playerName},
                        {"READY_FOR_START", true},
                        {"FOCUSING", BattlePosId },
                        {"ISHOST", true },
                        {"POSITION", PosId },
                    });
                }
                else
                {
                    string playerName = PhotonNetwork.player.CustomProperties["NAME"].ToString();
                    PhotonNetwork.player.SetCustomProperties(new Hashtable
                    {
                        {"NAME", playerName},
                        {"READY_FOR_START", true},
                        {"FOCUSING", BattlePosId },
                        {"ISHOST", false },
                        {"POSITION", PosId },
                    });
                }
            }
        }
        private int focusPlayerId;
        public int FocusPlayerId
        {
            get { return focusPlayerId; }
            set
            {
                focusPlayerId = value;
                currentLookPosId = PlayerHeroes[focusPlayerId].posId;
                BuffList.Instance.ClearBuff();
                BuffList.Instance.HeroBuffList = PlayerHeroes[FocusPlayerId].BuffList;
            }
        }
       
        private int remainPlayer;
        public int RemainPlayer
        {
            get { return remainPlayer; }
            set
            {
                remainPlayer = value;
                if (remainPlayer <= 1) {
                    if (!PlayerHeroes[playerId].isDead) {
                        PhotonView.RPC("PlayerTFTWin",PhotonTargets.All);
                    }
                }
            }
        }
        [SerializeField]
        public Opponent opponent;                   //Player's Opponent

        public int[] PlayerPosition;                //Player's Position Represent to PlayerArenas
        public RankManager RankManager;
        public PlayerHero[] PlayerHeroes;           //Players' Heroes List
        public GameObject[] PlayerArenas;           //PlayerArenas Location (The Data are hard coded in scene)
        public OpponentManager[] OpponentManagers;  //Player's Opponent Data
        //public Camera[] Cameras;                    //Cameras of Focusing Game Arena (The Data are hard coded in scene)
        public GridMap map;
        public List<Character> selfGameBoardHero = new List<Character>();
        public List<Character> battleGameBoardHero;
        List<Character> shadowHero = new List<Character>();
        TFTPlayerCharacter playerCharacter;
        private static DamageText textPrefab;
        private static GameObject canvas;

        public GameObject testButton;
        public Camera CurrentCamera;
        //public Camera CurrentCamera { get; private set; }
        public bool isHomeTeam { get; private set; }
        int waveFinishResponse;
        int currentLookPosId;


        public GameObject debugCamera;
        WaveType waveType;
        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            RankManager = RankManager.Instance;
            PhotonNetworkSetup();
            textPrefab = Resources.Load<DamageText>("otherPrefabs/damageText");
            canvas = GameObject.Find("Canvas");
        }
      
        void Update()
        {

            #region PhotonNetwork Debugging
            if (Input.GetKeyDown(KeyCode.L))
            {
                //  PhotonView.RPC("RPC_resetResponse", PhotonTargets.All);
                // PhotonView.RPC("MonsterBattle", PhotonTargets.All);
                MonsterBattle();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {

                PhotonView.RPC("RPC_resetResponse", PhotonTargets.All);
                PhotonView.RPC("RPC_Battle", PhotonTargets.All, 0, 1);
                //   PhotonView.RPC("RPC_Test", PhotonTargets.All);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {

                PhotonView.RPC("RPC_resetResponse", PhotonTargets.All);
                PhotonView.RPC("RPC_Battle", PhotonTargets.All, 0, 1);
                PhotonView.RPC("RPC_Battle", PhotonTargets.All, 2, 3);
                //   PhotonView.RPC("RPC_Test", PhotonTargets.All);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonView.RPC("RankChange", PhotonTargets.All, PlayerHeroes[playerId].player.NickName, 15);
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
            }
            #endregion

            //Set Player Random Position
            PlayersId = new string[PhotonNetwork.playerList.Length];
            PlayerName = new string[PhotonNetwork.playerList.Length];
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                PlayersId[i] = PhotonNetwork.playerList[i].NickName;
                PlayerName[i] = PhotonNetwork.playerList[i].CustomProperties["NAME"].ToString();
            }
            PhotonView.RPC("RPC_SetupPlayerPosition", PhotonTargets.All, GetRearrangeData(PhotonNetwork.playerList.Length));
            PhotonView.RPC("RPC_SetupPlayerName", PhotonTargets.Others, PlayersId, PlayerName);
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
            PhotonView.RPC("RPC_SyncPlayerHeroes", PhotonTargets.All, PosId,
                playerId, _syncHero.name, _syncHero.position, _syncHero.HeroLevel, _syncHeroMethod);
        }

        /// <summary>
        /// Sync Player Hero Position to Other Players
        /// </summary>
        /// <param name="_hero"></param>
        /// <param name="_syncMoveHero"></param>
        public void SyncPlayerHeroPlace(Hero _hero, SyncMoveHero _syncMoveHero)
        {
            switch (_syncMoveHero)
            {
                case SyncMoveHero.AddGameboard:
                    selfGameBoardHero.Add(_hero);
                    break;
                case SyncMoveHero.RemoveGameboard:
                    selfGameBoardHero.Remove(_hero);
                    break;
            }
            _hero.networkPlaceId = _hero.HeroPlace.PlaceId;
            _hero.GetComponent<PhotonView>().RPC("syncNetworkPlaceId", PhotonTargets.All, _hero.networkPlaceId);
            PhotonView.RPC("RPC_SyncPlayerHeroPlace", PhotonTargets.All, PosId,
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
            if (PosId == _posId)
            {
                return GameManager.Instance.SelfPlayerArena.SelfArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
            }
            else
            {
                return PlayerArenas[_posId].GetComponent<PlayerArena>().SelfArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
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
            if (this.PosId == posId)
                return GameManager.Instance.SelfPlayerArena.SelfArena.HeroList.GetChild(placeId).GetComponent<HeroPlace>();
            else
               return PlayerArenas[posId].GetComponent<PlayerArena>().SelfArena.HeroList.GetChild(placeId).GetComponent<HeroPlace>();
            //   return PlayerArenas[posId].GetComponent<PlayerArena>().EnemyArena.HeroList.GetChild(placeId).GetComponent<HeroPlace>();
        }

        /// <summary>
        /// Get Hero by NetworkHero
        /// </summary>
        /// <param name="_networkHero"></param>
        /// <param name="_playerId"></param>
        /// <returns></returns>
        public Hero GetHeroByNetworkHero(NetworkHero _networkHero, int? _placeType = null, int? _playerId = null)
        {
            int placeType = _placeType ?? 0;
            int targetPlayerId = _playerId ?? playerId;
            //This is in Hero List
            if (placeType == 0)
            {
                return PlayerArenas[PlayerHeroes[targetPlayerId].posId].
                GetComponent<PlayerArena>().SelfArena.HeroList.
                GetChild(_networkHero.position).GetChild(0).GetComponent<Hero>();
            }
            //This is in Game Board
            else
            {
                return PlayerArenas[PlayerHeroes[targetPlayerId].posId].
                GetComponent<PlayerArena>().SelfArena.GameBoard.
                GetChild(_networkHero.position).GetChild(0).GetComponent<Hero>();
            }
            
        }

        /// <summary>
        /// Get Player's Hero Place by Id
        /// </summary>
        /// <param name="_posId"></param>
        /// <param name="_placeId"></param>
        /// <param name="_isEnemy"></param>
        /// <returns></returns>
        public HeroPlace GetMyGameBoardEnemyHeroPlace(int _posId, int _placeId)
        {
          //  if (this.posId == _posId)
                return PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
           // else
               // return PlayerArenas[_posId].GetComponent<PlayerArena>().SelfArena.GameBoard.GetChild(_placeId).GetComponent<HeroPlace>();
        }
      /*  public HeroPlace GetOpponentHeroPlace(int placeId, bool isEnemyPlace) {
            if (isEnemyPlace)
                return PlayerArenas[PlayerHeroes[opponent.opponentId].posId].GetComponent<PlayerArena>().EnemyArena.GameBoard.GetChild(placeId).GetComponent<HeroPlace>();
            else
                return PlayerArenas[PlayerHeroes[opponent.opponentId].posId].GetComponent<PlayerArena>().SelfArena.GameBoard.GetChild(placeId).GetComponent<HeroPlace>();
        }*/
        public HeroPlace GetBattleHeroPlace(int posId,int placeId, bool isEnemyPlace)
        {
            if (isEnemyPlace)
                return PlayerArenas[posId].GetComponent<PlayerArena>().EnemyArena.GameBoard.GetChild(placeId).GetComponent<HeroPlace>();
            else
                return PlayerArenas[posId].GetComponent<PlayerArena>().SelfArena.GameBoard.GetChild(placeId).GetComponent<HeroPlace>();
        }
        public Character GetBattleBoardHero(int posId, int placeId, bool isEnemyPlace)
        {
            if (isEnemyPlace)
                return PlayerArenas[posId].GetComponent<PlayerArena>().EnemyArena.GameBoard.GetChild(placeId).GetChild(0).GetComponent<Hero>();
            else
                return PlayerArenas[posId].GetComponent<PlayerArena>().SelfArena.GameBoard.GetChild(placeId).GetChild(0).GetComponent<Hero>();
        }
        public Character getRandomCharacter(bool isEnemy) {

            if (isEnemy)
            {
                int random = Random.Range(0, opponent.heroes.Count);
                return opponent.heroes[random];
            }
            else {
                int random = Random.Range(0, battleGameBoardHero.Count);
                return battleGameBoardHero[random];
            }

        }
        #endregion

        public Character getCloestEnemyTarget(bool isEnemy, Transform heroPos) {

            if (isEnemy)
            {
             //   Debug.Log("Get battleGame " + isEnemy);
                return calculateClosestDistance(opponent.heroes, heroPos);
            }
            else
            {
                //Debug.Log("Get opponent " + isEnemy);            
                return calculateClosestDistance(battleGameBoardHero, heroPos);
            }
        }
        public Character getFurthestEnemyTarget(bool isEnemy, Transform heroPos)
        {

            if (isEnemy)
            {
                Debug.Log("Get battleGame " + isEnemy);
                return calculateFurthestDistance(opponent.heroes, heroPos);
            }
            else
            {
                Debug.Log("Get opponent " + isEnemy);
                return calculateFurthestDistance(battleGameBoardHero, heroPos);
            }
        }
        public float getNodeDistance(HeroPlace self,HeroPlace target) {
           float dis= map.GetDistance(map.getHeroPlaceGrid(self), map.getHeroPlaceGrid(target));
            return dis;
        }
      /*  public Character getEnemyIndexById(int placeId,bool isEnemy) {
            if (isEnemy){
                int index = selfGameBoardHero.FindIndex(x => x.networkPlaceId == placeId);
                if(index!=-1)
                return selfGameBoardHero[index];
            }
            else{
               int index = NetworkManager.Instance.opponent.hero.FindIndex(x => x.networkPlaceId == placeId);
                if (index != -1)
                  return opponent.hero[index];
            }
            return null;
        }*/
        Character calculateClosestDistance(List<Character> targetHeros, Transform heroPos) {
            Character[] hero = targetHeros.ToArray<Character>();

            if (hero.Length != 0) {
                float closestDis = Vector3.Distance(hero[0].transform.position, heroPos.position);
                Character closestHero = hero[0];
                for (int i = 1; i < hero.Length; i++) {
                    float dis = Vector3.Distance(hero[i].transform.position, heroPos.position);
                  //  Debug.Log("MY hero "+heroPos.name+"taget Hero "+hero[i].name+"Dis " + dis);
                    if (dis < closestDis)
                    {
                        closestDis = dis;
                        closestHero = hero[i];
                   //     Debug.Log("CLOESTEST !!!! MY hero " + heroPos.name + "taget Hero " + hero[i].name + "Dis " + dis);
                    }
                }
                return closestHero;
            }
            return null;
        }
        Character calculateFurthestDistance(List<Character> targetHeros, Transform heroPos)
        {
            Character[] hero = targetHeros.ToArray<Character>();

            if (hero.Length != 0)
            {
                float furthestDis = Vector3.Distance(hero[0].transform.position, heroPos.position);
                Character furthestHero = hero[0];
                for (int i = 1; i < hero.Length; i++)
                {
                    float dis = Vector3.Distance(hero[i].transform.position, heroPos.position);
                    if (dis > furthestDis)
                    {
                        furthestHero = hero[i];
                        furthestDis = dis;
                    }
                }
                return furthestHero;
            }
            return null;
        }
        public GameObject getCameraObject(int id,int player)
        {
            if (playerId==player)
            {
                // currentCamera = PlayerArenas[id].GetComponent<PlayerArena>().Camera.GetComponent<Camera>();
                return PlayerArenas[id].GetComponent<PlayerArena>().Camera;
            }
            else {
                //   currentCamera = PlayerArenas[id].GetComponent<PlayerArena>().enemyCamera.GetComponent<Camera>();             
                return PlayerArenas[id].GetComponent<PlayerArena>().enemyCamera;
            }
        }
        private void SetCurrentCamera(bool isEnemy,int id) {
            if (PlayerPosition[id] == playerId)
            {
                if (CurrentCamera != null) CurrentCamera.gameObject.SetActive(false);
                CurrentCamera = PlayerArenas[id].GetComponent<PlayerArena>().Camera.GetComponent<Camera>();
                CurrentCamera.gameObject.SetActive(true);
            }
            else
            {
                if (CurrentCamera != null) CurrentCamera.gameObject.SetActive(false);
                CurrentCamera = PlayerArenas[id].GetComponent<PlayerArena>().enemyCamera.GetComponent<Camera>();
                CurrentCamera.gameObject.SetActive(true);
            }
        }
        public void WatchOtherPlayer(string name) {
            for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                PhotonPlayer target = PhotonNetwork.playerList[i];
                string targetNickName = target.CustomProperties["NAME"].ToString();
                if (name.Equals(targetNickName))
                {
                    bool isSelf = target == PhotonNetwork.player;
                    int targetCurrPosId = (int)target.CustomProperties["FOCUSING"];
                    int targetPosId = (int)target.CustomProperties["POSITION"];
                    bool isHost = (bool)target.CustomProperties["ISHOST"];
                    CurrentCamera.gameObject.SetActive(false);

                    if (isSelf)
                    {
                        SetCurrentCamera(!isHost, targetCurrPosId);
                    }
                    else
                    {
                        SetCurrentCamera(isHost, targetCurrPosId);
                    }
                    CurrentCamera.gameObject.SetActive(true);

                    BuffList.Instance.HeroBuffList = PlayerHeroes[targetPosId].BuffList;
                    break;
                }
            }
            //for (int i = 0; i < PlayerName.Length; i++)
            //{
            //    if (i == PosId)
            //        PlayerArenas[PosId].GetComponent<PlayerArena>().Camera.SetActive(false);
            //    else
            //        PlayerArenas[i].GetComponent<PlayerArena>().enemyCamera.SetActive(false);          
             
            //    if (PlayerName[i].Equals(name))
            //    {
            //        if (i == PosId)
            //        {
            //            PlayerArenas[i].GetComponent<PlayerArena>().Camera.SetActive(true);
            //            setCurrentCamera(false, PosId);
            //            currentLookPosId = PosId;
            //        }
            //        else
            //        {
            //            PlayerArenas[i].GetComponent<PlayerArena>().enemyCamera.SetActive(true);
            //            setCurrentCamera(true, i);
            //            currentLookPosId = i;
            //        }
            //        BuffList.Instance.HeroBuffList = PlayerHeroes[i].BuffList;
            //    }
            //}
        }
        public void showDamageText(string damage,DamageType type,Vector3 position,int posId) {
            if (currentLookPosId == posId)
            {
                DamageText text = Instantiate(textPrefab);
                text.setText(damage, type);
                Vector2 screenPosition = CurrentCamera.WorldToScreenPoint(position);
                text.transform.SetParent(canvas.transform, false);
                text.transform.position = screenPosition;
               // Debug.Log("Damage "+damage+ " posid "+ posId+" current "+currentLookPosId);
            }
        }
        #region Set Player Opponent
        /// <summary>
        /// Match Player Opponent, Called by Master Client
        /// </summary>
        public void MatchPlayerOpponent()
        {
            //Get the surival players list
            //int[] opponentResult = GetRearrangeData(PhotonNetwork.playerList.Length);
            int[] opponentResult = GetRemainPlayer();
            // PhotonView.RPC("RPC_SyncPlayersOpponent", PhotonTargets.All, opponentResult);
            SetPlayerOpponent(opponentResult);
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
             /*   if (matchResult[matchResult.Length / 2] == playerId)
                {
                    opponent = new Opponent(matchResult[0], true);
                }*/
            }
            else
            {
                OpponentManagers = new OpponentManager[matchResult.Length / 2];
            }

            PhotonView.RPC("RPC_resetResponse", PhotonTargets.All);
            for (int i = 0; i < matchResult.Length / 2; i++)
            {
                OpponentManagers[i] = new OpponentManager(matchResult[i], matchResult[matchResult.Length - 1 - i]);
                Debug.Log("Player " + matchResult[i] + " vs  Player" + matchResult[matchResult.Length - 1 - i]);
             //   Debug.Log("OpponentManager "+ i);
                //  PhotonView.RPC("RPC_Battle", PhotonTargets.All, matchResult[i], matchResult[matchResult.Length - 1 - i]);
                //    Debug.Log("My playerid "+playerId);
                //     if (matchResult[i] == playerId)
                //     {
                //opponent = new Opponent(matchResult[matchResult.Length - 1 - i]);

                //    PhotonView.RPC("RPC_Battle", PhotonTargets.All, matchResult[i], matchResult[matchResult.Length - 1 - i]);
                //   }
                /* else if (matchResult[matchResult.Length - 1 - i] == playerId)
                 {
                     opponent = new Opponent(matchResult[i]);
                 }*/
            }
            Debug.Log("battle count "+OpponentManagers.Length);
            for (int i = 0; i < OpponentManagers.Length; i++) {
                battleAdapter(OpponentManagers[i].host, OpponentManagers[i].guest,OpponentManagers[i].isShadow);
            }
        }
        public void battleAdapter(int host,int guest,bool isShadow) {
            if (isShadow)
            {
                 PhotonView.RPC("RPC_ShadowBattle", PlayerHeroes[host].player,guest);
            }
            else {
                PhotonView.RPC("RPC_Battle", PhotonTargets.All, host, guest);
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
        public int[] GetRemainPlayer() {
            int []remainPlayers = new int[RemainPlayer];
            int count = 0;
            for (int i = 0; i < PlayerHeroes.Length; i++) {
                if (!PlayerHeroes[i].isDead) {
                    remainPlayers[count] = i;
                    count++;
                }                    
            }
            System.Random r = new System.Random(System.DateTime.Now.Millisecond);
            return remainPlayers.OrderBy(x => r.Next()).ToArray();
        }
        public HeroPlace getNeighboursHeroPlace(HeroPlace currentHeroplace) {
            List<Node> neighbour = map.getNeighbours(map.getHeroPlaceGrid(currentHeroplace));
            bool isFind=false;
            Node currentNode = null ;
            foreach (Node n in neighbour) {
                if (n.isWalkable)
                {
                    isFind = true;
                    currentNode = n;
                    break;
                }
            }
            if (isFind)
                return currentNode.heroPlace;
            else
               return getNeighboursHeroPlace(neighbour[neighbour.Count - 1].heroPlace);
        }


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

        [PunRPC]
        public void RPC_SetupPlayerName(string[] _playerId, string[] _playerName)
        {
            PlayersId = _playerId;
            PlayerName = _playerName;
            RankManager.PlayerCollectionSetup();
        }
        /// <summary>
        /// Setting Lobby Player Arena Randam Positon
        /// </summary>
        /// <param name="_playerPosition"></param>
        [PunRPC]
        public void RPC_SetupPlayerPosition(int[] _playerPosition)
        {
            for (int i = 0; i < _playerPosition.Length; i++)
            {
                PlayerHeroes[i] = new PlayerHero();
            }
                PlayerPosition = _playerPosition;
            RemainPlayer = PhotonNetwork.playerList.Length;
            for (int i = 0; i < _playerPosition.Length; i++)
            {
                if (_playerPosition[i] == playerId)
                {
                    GameManager.Instance.SelfPlayerArena = PlayerArenas[i].GetComponent<PlayerArena>();
                    map.setPlayerArena(GameManager.Instance.SelfPlayerArena);
                    PosId = i;
                    /* PlayerHeroes[playerId] = new PlayerHero
                     {
                         posId = posId,
                         player = PhotonNetwork.player
                     };*/
                    // Debug.Log("my id " + playerId + " posid " + posId);
                    string characterName=PhotonNetwork.player.CustomProperties["Character_Name"].ToString();

                    if (characterName == "")
                        characterName = "LifeStone";
                    playerCharacter = PhotonNetwork.Instantiate(Path.Combine("otherPrefabs", characterName), Vector3.zero, Quaternion.identity, 0).GetComponent<TFTPlayerCharacter>();
                    playerCharacter.GetComponent<PhotonView>().RPC("RPC_SyncPlayerCharacterPosition", PhotonTargets.All, PosId);
                    int CharacterViewId = playerCharacter.GetComponent<PhotonView>().viewID;
                  //  Debug.Log("set my position playerid " + playerId + " posid " + posId);
                   // PlayerHeroes[playerId].setPersonalInformation(posId, PhotonNetwork.player, CharacterViewId);
                    GameManager.Instance.AddPlayerHeroListener(PlayerHeroes[playerId]);
                    //Cameras[posId].enabled = true;

                    GameManager.Instance.SelfPlayerArena.Camera.SetActive(true);
                    GameManager.Instance.MainCamera = GameManager.Instance.SelfPlayerArena.Camera.GetComponent<Camera>();
                    CurrentCamera = GameManager.Instance.MainCamera;



                    PhotonView.RPC("RPC_SyncPlayerInformation", PhotonTargets.All, playerId, PosId, PhotonNetwork.player, CharacterViewId);
                    createEquipmentBoard();
;                    //PlayerHeroes[playerId] = PlayerHero;
                }
            }
        }
        void createEquipmentBoard() {
            UnityEngine.Object pPrefab = Resources.Load("otherPrefabs/Equipment List");
            GameObject gameObject = Instantiate(pPrefab) as GameObject;
            gameObject.transform.parent = PlayerArenas[PosId].GetComponent<PlayerArena>().equipmentBoard;
            gameObject.transform.localPosition = Vector3.zero;
            MonsterWaveManager.Instance.setEquipmentBoard(gameObject.GetComponent<EquipmentSlotManager>());
        }
        [PunRPC]
        public void RPC_SyncPlayerInformation(int SyncPlayerId, int SyncPosId, PhotonPlayer player,int tftId) {
            Debug.Log("SYNC inofor player id " + SyncPlayerId + "  POsid " + SyncPosId+" tft ID "+tftId);
            PlayerHeroes[SyncPlayerId].setPersonalInformation(SyncPosId, player,tftId);
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

            // GamePlace enemyArena = PlayerArenas[_posId].GetComponent<PlayerArena>().EnemyArena;
            GamePlace enemyArena = PlayerArenas[_posId].GetComponent<PlayerArena>().SelfArena;
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
                            NetworkHero networkHero = ChangedHero[i];
                            PlayerHeroes[_playerId].GameboardAddHero(ChangedHero[i]);
                            PlayerHeroes[_playerId].UsableHeroes.RemoveAt(i);
                            PlayerHeroes[_playerId].UsableHeroes.Add(networkHero);
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
                                   // setOtherPlayerBattleHero();
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
            TeamFlag.Instance.GameboardCard.text = GameManager.Instance.PlayerHero.GameBoardHeroes.Count.ToString();

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
            FocusPlayerId = playerId;

            switch (_syncMethod)
            {
                case SyncHeroMethod.AddHero:
                    PlayerHeroes[_playerId].UsableHeroes.Add(new NetworkHero(_name, _heroPos, _heroLevel));
                    break;
                case SyncHeroMethod.RemoveHero:
                    List<NetworkHero> RemoveHero = PlayerHeroes[_playerId].UsableHeroes;
                    List<NetworkHero> RemoveGameHero = PlayerHeroes[_playerId].GameBoardHeroes;
                    for (int i = 0; i < RemoveHero.Count; i++)
                    {
                        if (RemoveHero[i].name.Equals(_name) &&
                            RemoveHero[i].position == _heroPos &&
                            RemoveHero[i].HeroLevel == _heroLevel)
                        {
                            if (RemoveGameHero.Contains(RemoveHero[i]))
                            {
                                Debug.Log("RemoveGameHero index: " + RemoveGameHero.IndexOf(RemoveHero[i]));
                                RemoveGameHero.Remove(RemoveHero[i]);
                            }
                            RemoveHero.Remove(RemoveHero[i]);
                            break;
                        }
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

                    break;
            }
            GameManager.Instance.PlayerHero = PlayerHeroes[playerId];
        }

        #endregion

        #region Battle

        [PunRPC]
        void RPC_Battle(int hostID, int guestID)
        {
            if (playerId == hostID || playerId == guestID)
            {
                waveType = WaveType.Player;
                BattlePosId = PlayerHeroes[hostID].posId;
                currentLookPosId = BattlePosId;
            }
            if (playerId == guestID)
            {
                PlayerArenas[PlayerHeroes[guestID].posId].GetComponent<PlayerArena>().Camera.SetActive(false);
                PlayerArenas[PlayerHeroes[hostID].posId].GetComponent<PlayerArena>().enemyCamera.SetActive(true);
                opponent.opponentId = hostID;
                setOppoentHero(hostID, hostID);
                isHomeTeam = false;
                setBattleGameBoardHero();
                SetCurrentCamera(true,BattlePosId);
                playerCharacter.GetComponent<PhotonView>().RPC("RPC_PlayerCharacterMoveToGameBoard", PhotonTargets.All, PlayerHeroes[hostID].posId);
                Debug.Log("player battle Posid"+BattlePosId);
            }
            if (playerId == hostID) {
                isHomeTeam = true;
                opponent.opponentId = guestID;
                setOppoentHero(hostID, guestID);
                SetCurrentCamera(false,BattlePosId);
                // setBattleGameBoardHero();
                CurrentCamera = PlayerArenas[BattlePosId].GetComponent<PlayerArena>().Camera.GetComponent<Camera>();
                StartCoroutine(startBattle(PlayerHeroes[hostID].posId));
                Debug.Log("player battle posid "+ BattlePosId);
            }
        }
        [PunRPC]
        public void RPC_ShadowBattle(int guest) {
            shadowBattle(guest);
        }
        public void shadowBattle(int guest)
        {
            Debug.Log("Shadow Battle");
            opponent.opponentId = -1;
            waveType = WaveType.Shadow;
            BattlePosId = PosId;
            currentLookPosId = BattlePosId;
            isHomeTeam = true;
            foreach (NetworkHero networkHero in PlayerHeroes[guest].GameBoardHeroes)
            {
                Hero hero = PhotonNetwork.Instantiate(Path.Combine("Prefabs", networkHero.name), Vector3.zero, Quaternion.identity, 0).GetComponent<Hero>();
                hero.GetComponent<PhotonView>().RPC("RPC_MoveToThePlayerHeroPlace",PhotonTargets.All ,PosId,networkHero.position,true);
                opponent.heroes.Add(hero);
                shadowHero.Add(hero);

            }
            StartCoroutine(startBattle(PosId));
        }
        void setOppoentHero(int homePlayerId, int opponentPlayerId) {
            foreach (NetworkHero networkHero in PlayerHeroes[opponentPlayerId].GameBoardHeroes)
            {
                //Hero heroObject = PlayerArenas[PlayerHeroes[opponentPlayerId].posId].
                //                  GetComponent<PlayerArena>().SelfArena.GameBoard.
                //                  GetChild(networkHero.position).GetChild(0).GetComponent<Hero>();

                Hero heroObject = GetHeroByNetworkHero(networkHero, 1, opponentPlayerId);

                heroObject.isEnemy = true;
                //Hero heroObject = GameManager.Instance.SelfPlayerArena.SelfArena.GameBoard.GetChild(networkHero.position).GetChild(0).GetComponent<Hero>(); 
                opponent.heroes.Add(heroObject);
                if (playerId == homePlayerId)
                    heroObject.GetComponent<PhotonView>().RPC("RPC_MoveToThePlayerHeroPlace",
                        PhotonTargets.All,
                        PlayerHeroes[homePlayerId].posId, networkHero.position,true);
            }

        }
        void setBattleGameBoardHero(){
                battleGameBoardHero = new List<Character>(selfGameBoardHero);
        }
        public void battleHeroDie(bool isEnemy,Character hero) {
            if (isHomeTeam)
            {
                if (isEnemy)
                {
                    //Debug.Log("character die " + hero.name);
;                    opponent.heroes.Remove(hero);
                    if(opponent.opponentId!=-1)
                    PhotonView.RPC("RPC_SyncBattleHero", PlayerHeroes[opponent.opponentId].player,hero.photonView.viewID,true);
                    if (opponent.heroes.Count == 0)
                    {
                   //     Debug.Log("i win ");
                        playerWinBattle(playerId,opponent.opponentId,battleGameBoardHero);                    
                    }
                }
                else
                {                  
                   
                        battleGameBoardHero.Remove(hero);
                    if (opponent.opponentId != -1)
                    {
                        PhotonView.RPC("RPC_SyncBattleHero", PlayerHeroes[opponent.opponentId].player, hero.photonView.viewID, false);
                    }
                        if (battleGameBoardHero.Count == 0)
                        {
                       //     Debug.Log("opponent win ");
                            playerWinBattle(opponent.opponentId, playerId,opponent.heroes);                    
                        }
                    
                }
            }
        }
        public void playerWinBattle(int winnerId,int loserId,List<Character> remainHero) {
            if (winnerId == -1)
            {
                Debug.Log("monster win Battle " + winnerId);
                MonsterHitPlayer();
            }
            else if (loserId != -1)
            {
                Debug.Log("player win Battle " + winnerId);
                PhotonView.RPC("RPC_HitOpponent", PlayerHeroes[winnerId].player,PlayerHeroes[loserId].TFTCharacterId);
            }

                   
           // if (winnerId == -1 || loserId == -1)
                PhotonView.RPC("RPC_Response", PhotonTargets.All, winnerId,loserId);
          //  else
           //     PhotonView.RPC("RPC_Response", PhotonTargets.All, 2);

           if (opponent.opponentId != -1)
            {
                int totalDamage = 0;
                foreach (Hero hero in remainHero) {
                    totalDamage += (int)hero.Rarity+1 + (int)hero.heroLevel;
                }
                PhotonView.RPC("RankChange",PhotonTargets.All, PlayerHeroes[loserId].player.NickName, totalDamage);
            }

            map.resetMap();

        }
        public void hitTFTPLayer(int damage,string name) {
            PhotonView.RPC("RankChange", PhotonTargets.All,name, damage);
        }
        [PunRPC]
        public void overTimeFinish() {
            if (isHomeTeam && !PlayerHeroes[playerId].isResponse)
                noPlayerWin();
        }
        public void noPlayerWin() {

            Debug.Log("no player win");
            if (opponent.opponentId != -1)
            {
               
                if(opponent.heroes.Count!=0)
                PhotonView.RPC("RPC_HitOpponent", PlayerHeroes[opponent.opponentId].player,PlayerHeroes[playerId].TFTCharacterId);
            }
            else
            {              
                    MonsterHitPlayer();
            }
            PhotonView.RPC("RPC_Response", PhotonTargets.All,playerId,opponent.opponentId);
            if (battleGameBoardHero.Count != 0&&opponent.opponentId!=-1)
                PhotonView.RPC("RPC_HitOpponent", PlayerHeroes[playerId].player,PlayerHeroes[opponent.opponentId].TFTCharacterId);

          
            map.resetMap();
        }
        [PunRPC]
        public void RPC_HitOpponent(int id) {
            hitOpponent(id);
        }
        void hitOpponent(int id) {
            Debug.Log("Newwork site hit oppoent");
            for (int i = 0; i < battleGameBoardHero.Count; i++)
            {
                battleGameBoardHero[i].GetComponent<PhotonView>().RPC("RPC_HitPlayerCharacter", PhotonTargets.All, id);
            }
        }
        
        void MonsterHitPlayer()
        {
            Debug.Log("monster hit oppoent");
            int totalDamage = 0;
            
            foreach (Character monster in opponent.heroes)
            {
                monster.GetComponent<PhotonView>().RPC("RPC_HitPlayerCharacter", PhotonTargets.All, PlayerHeroes[playerId].TFTCharacterId);
                totalDamage += monster.hitDamage;
            }
            PhotonView.RPC("RankChange", PhotonTargets.All, PlayerHeroes[playerId].player.NickName, totalDamage);
            
        }
        [PunRPC]
        public void RPC_SyncBattleHero(int id,bool isSelf) {
            syncBattleHero(id, isSelf);
        }
        public void syncBattleHero(int id, bool isSelf) {
            if (isSelf)
            {
                Character c = PhotonView.Find(id).GetComponent<Character>();
                battleGameBoardHero.Remove(c);
            }
            else
            {
                Character c = PhotonView.Find(id).GetComponent<Character>();
                opponent.heroes.Remove(c);
            }
        }
        public void addBattleHeroAdapter(Character c, bool isEnmy,int id) {
          
            if (isEnmy) 
                opponent.heroes.Add(c);
            else
                battleGameBoardHero.Add(c);
            if (opponent.opponentId != -1)
            {
                Debug.Log("summon " + c.name+" player "+ PlayerHeroes[opponent.opponentId].player+" photon "+id);
                PhotonView.RPC("RPC_addBattleHero",PlayerHeroes[opponent.opponentId].player, id, isEnmy);
            }

        }
        [PunRPC]
        public void RPC_addBattleHero(int id,bool isSelf) {
            addBattleHero(id, isSelf);
        }
        public void addBattleHero(int id, bool isSelf) {
            Character c = PhotonView.Find(id).GetComponent<Character>();
            if (isSelf)
            {
                battleGameBoardHero.Add(c);
            }
            else
            {
                opponent.heroes.Add(c);
            }
        }

        IEnumerator finishBattle() {
            foreach (Character character in opponent.heroes) {
                character.stopCharacter();
            }
            foreach (Character character in battleGameBoardHero) {
                character.stopCharacter();
            } 
            yield return new WaitForSeconds(2f);
           if(PhotonNetwork.isMasterClient)
                GameManager.Instance.finishWave();
           
            if (waveType == WaveType.Monster) {
                foreach (Monster monster in opponent.heroes) {
                    monster.destory();
                }
            }
            if (waveType == WaveType.Shadow) {
                Debug.Log("Shadow List Length" + shadowHero.Count);
                foreach (Character character in shadowHero) {

                    PhotonNetwork.Destroy(character.gameObject);
                    
                }
                shadowHero.Clear();
            }
            if (!isHomeTeam)
            {
                playerCharacter.GetComponent<PhotonView>().RPC("RPC_PlayerCharacterBackToGameBoard", PhotonTargets.All, PosId);
                if(opponent.opponentId!=-1)
                PlayerArenas[PlayerHeroes[opponent.opponentId].posId].GetComponent<PlayerArena>().enemyCamera.SetActive(false);
                GameManager.Instance.SelfPlayerArena.Camera.SetActive(true);
            }
            else {
                foreach (Monster monster in opponent.heroes.OfType<Monster>()) {
                    monster.destory();
                }
                foreach (Monster monster in battleGameBoardHero.OfType<Monster>())
                {
                    monster.destory();
                }
            }
            isHomeTeam = false;
            ResetHeroAfterBattle();
        }
        void ResetHeroAfterBattle() {
           Debug.Log("reset Position id: "+playerId+" Hero "+selfGameBoardHero.Count);          
            foreach (Character hero in selfGameBoardHero)
            {       
                hero.photonView.RPC("RPC_AddToGameBoard", PhotonTargets.All, PosId, hero.networkPlaceId);
                hero.photonView.RPC("RPC_ResetStatus", PhotonTargets.All);
            }
            opponent.heroes.Clear();
        }
       
        [PunRPC]
        public void RPC_Response(int player1,int player2) {
            if (player1 != -1)
            {
                PlayerHeroes[player1].isResponse = true;
                waveFinishResponse ++;
            }
            if (player2 != -1)
            {
                PlayerHeroes[player2].isResponse = true;
                waveFinishResponse++;
            }
           
            Debug.Log("Response +" + " total "+waveFinishResponse);
            if (PhotonNetwork.isMasterClient)
            {
                if (waveFinishResponse == RemainPlayer) {
                    PhotonView.RPC("RPC_endWave", PhotonTargets.All);
                   
                }
            }
               
        }
        [PunRPC]
        public void RPC_resetResponse()
        {
            for (int i = 0; i < PlayerHeroes.Length; i++) {
                PlayerHeroes[i].isResponse = false;
            }
            waveFinishResponse = 0;
        }
        public bool BattleFinish() {
            return waveFinishResponse == PlayerHeroes.Length;
        }
        [PunRPC]
        public void RPC_endWave()
        {
            StartCoroutine(finishBattle());
        }
        
        IEnumerator startBattle(int playerPosId) {
        //    Debug.Log("start Battle");
            yield return new WaitForSeconds(2);
            battleGameBoardHero = new List<Character>(selfGameBoardHero);
            if (battleGameBoardHero.Count == 0 && opponent.heroes.Count == 0)
            {
                noPlayerWin();
            }
            else if (battleGameBoardHero.Count == 0)
            {
                playerWinBattle(opponent.opponentId,playerId,opponent.heroes);
            }
            else if (opponent.heroes.Count == 0)
            {
                playerWinBattle(playerId,opponent.opponentId,battleGameBoardHero);
            }
            else
            {
          //      Debug.Log("start Battle mid ");
                foreach (Hero hero in battleGameBoardHero)
                {
                    //hero
                    hero.ReadyForBattle(false, BattlePosId);
                }


                foreach (Character hero in opponent.heroes)
                {
                    hero.ReadyForBattle(true, BattlePosId);
                }
            }
         //   Debug.Log("start Battle finish");
        }
        #endregion
        #region monster
        public void MonsterBattle() {
            PhotonView.RPC("RPC_resetResponse", PhotonTargets.All);
            PhotonView.RPC("RPC_MonsterBattle", PhotonTargets.All);
        }
        [PunRPC]
        public void RPC_MonsterBattle() {
            isHomeTeam = true;
            waveType = WaveType.Monster;
            opponent.opponentId = -1;
            BattlePosId = PosId;
            currentLookPosId = BattlePosId;
            SetCurrentCamera(false,BattlePosId);
            MonsterWaveManager.Instance.spawnCurrentWaveAllMonster();
        }
        public void spawnMonster(string name,int placeId) {
            Monster monster = PhotonNetwork.Instantiate(Path.Combine("Prefabs", name), Vector3.zero, Quaternion.identity, 0).GetComponent<Monster>();
            monster.GetComponent<PhotonView>().RPC("RPC_MoveToThePlayerHeroPlace", PhotonTargets.All, PosId, placeId,true);
            opponent.heroes.Add(monster);
        }

        public void BattleWithMonsters() {
            StartCoroutine(startBattle(BattlePosId));
        }
        #endregion
        [PunRPC]
        void RPC_Test() {
            Hero newhero = (PhotonNetwork.Instantiate(Path.Combine("Prefabs", "GOBLIN"), Vector3.zero, Quaternion.identity, 0)).GetComponentInChildren<Hero>();
            //  Monster monster = (PhotonNetwork.Instantiate(Path.Combine("Prefabs", "GOBLIN"), Vector3.zero, Quaternion.identity, 0)).GetComponentInChildren<Monster>();
           // newhero.photonView.RPC("RPC_AddToHeroList", PhotonTargets.All,0,1);
            //newhero.name = "Armor Crocodile";
            // if (TFT.GameManager.Instance.BuyHero(newHero));
           // Debug.Log("Hero GameBoard"+ selfGameBoardHero.Count);
        }

        [PunRPC]
        public void RankChange(string _playerName, int _value)
        {
            Debug.Log("RankChange " + _playerName);
            RankManager.DeductHP(_playerName, _value);
        }
        [PunRPC]
        public void RPC_Time_Sync(float _time)
        {
            GameManager.Instance.RemainTime = _time;
        }
        [PunRPC]
        public void RPC_RoundUp()
        {
            GameManager.Instance.RoundManager.RoundUp();
        }
        [PunRPC]
        public void RPC_ChangeStatus(GameStatus _gameStatus, GameStatus _lastGameStatus)
        {
            GameManager.Instance.GameStatus = _gameStatus;
            GameManager.Instance.LastGameStatus = _lastGameStatus;
        }
        public void playerDie() {
            Debug.Log("PLayer Die");
            
            
            PhotonView.RPC("RPC_PlayerDie",PhotonTargets.All,playerId);
            PhotonNetwork.DestroyPlayerObjects(PlayerHeroes[playerId].player);
        }
        [PunRPC]
        public void RPC_PlayerDie(int playerId) {
            PlayerHeroes[playerId].isDead = true;
            RemainPlayer--;
           
        }
        [PunRPC]
        public void PlayerTFTWin() {
            testButton.SetActive(true);
        }
        public void outGame()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (PhotonNetwork.playerList.Length > 1)
                    PhotonNetwork.SetMasterClient(PhotonNetwork.masterClient.GetNext());

            }
            StartCoroutine(DisconnectLoading());
        }
        private IEnumerator DisconnectLoading()
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.inRoom)
                yield return null;
            SceneManager.LoadScene("Lobby");
        }
    }
}