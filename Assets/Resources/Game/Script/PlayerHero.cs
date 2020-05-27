using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class HeroClassRace
    {
        public HeroClass[] HeroClasses;
        public HeroRace[] HeroRaces;
        public HeroClassRace(HeroClass[] _heroClasses, HeroRace[] _heroRaces)
        {
            HeroClasses = _heroClasses;
            HeroRaces = _heroRaces;
        }
    }

    [System.Serializable]
    public class HeroBuffList
    {
        List<string> HeroType = new List<string>();         //For verifing hero type
        public Dictionary<HeroClass, int> ClassValue { get; private set; } = new Dictionary<HeroClass, int>();   //Number of class in gameboard hero
        public Dictionary<HeroRace, int> RaceValue { get; private set; } = new Dictionary<HeroRace, int>();      //Number of race in gameboard hero
        public List<string> BuffsName
        {
            get
            {
                List<string> result = new List<string>();
                foreach (KeyValuePair<HeroClass, int> ClassValue in ClassValue)
                {
                    result.Add(ClassValue.Key.ToString());
                }
                foreach (KeyValuePair<HeroRace, int> RaceValue in RaceValue)
                {
                    result.Add(RaceValue.Key.ToString());
                }
                return result;
            }
        }

        public BuffListenHandler addBuff, upgradeBuff, removeBuff;
        public BuffListenEventArgs argsAdd, argsUpgrade, argsRemove;

        /// <summary>
        /// Call the method, while adding hero to the gameboard
        /// </summary>
        /// <param name="addedHero"></param>
        public void AddHeroBuff(NetworkHero addedHero)
        {
            
            HeroClassRace heroClassRace = GetClassRace(addedHero);
            bool isSameType = false;
            for (int j = 0; j < HeroType.Count; j++)
            {
                if (HeroType[j].Equals(addedHero.name))
                {
                    isSameType = true;
                    break;
                }
            }
            if (!isSameType)
            {
                for (int i = 0; i < heroClassRace.HeroClasses.Length; i++)
                {
                    if (ClassValue.ContainsKey(heroClassRace.HeroClasses[i]))
                    {
                        ClassValue[heroClassRace.HeroClasses[i]]++;

                        argsUpgrade= new BuffListenEventArgs();
                        argsUpgrade.BuffType = BuffType.Class;
                        argsUpgrade.HeroClass = heroClassRace.HeroClasses[i];
                        argsUpgrade.BuffCount = ClassValue[heroClassRace.HeroClasses[i]];
                        upgradeBuff?.Invoke(this, argsUpgrade);
                    }
                    else
                    {
                        ClassValue.Add(heroClassRace.HeroClasses[i], 1);

                        //argsAdd = new BuffListenEventArgs();
                        //argsAdd.BuffType = BuffType.Class;
                        //argsAdd.HeroClass = heroClassRace.HeroClasses[i];
                        //argsAdd.BuffCount = ClassValue[heroClassRace.HeroClasses[i]];
                        //addBuff?.Invoke(this, argsAdd);

                        //Debug.Log("AddHeroBuff: " + heroClassRace.HeroClasses[i]);
                    }
                }
                for (int i = 0; i < heroClassRace.HeroRaces.Length; i++)
                {
                    if (RaceValue.ContainsKey(heroClassRace.HeroRaces[i]))
                    {
                        RaceValue[heroClassRace.HeroRaces[i]]++;

                        argsUpgrade = new BuffListenEventArgs();
                        argsUpgrade.BuffType = BuffType.Race;
                        argsUpgrade.HeroRace = heroClassRace.HeroRaces[i];
                        argsUpgrade.BuffCount = RaceValue[heroClassRace.HeroRaces[i]];
                        upgradeBuff?.Invoke(this, argsUpgrade);
                    }
                    else
                    {
                        RaceValue.Add(heroClassRace.HeroRaces[i], 1);
                        //argsAdd = new BuffListenEventArgs();
                        //argsAdd.BuffType = BuffType.Race;
                        //argsAdd.HeroRace = heroClassRace.HeroRaces[i];
                        //argsAdd.BuffCount = RaceValue[heroClassRace.HeroRaces[i]];
                        //addBuff?.Invoke(this, argsAdd);

                        //Debug.Log("AddHeroBuff: " + heroClassRace.HeroRaces[i]);
                    }
                }
            }
            
        }

        /// <summary>
        /// Call the method, while removing hero from the gameboard
        /// </summary>
        /// <param name="removedHero"></param>
        public void RemoveHeroBuff(NetworkHero removedHero)
        {
            HeroClassRace heroClassRace = GetClassRace(removedHero);
            for (int i = 0; i < heroClassRace.HeroClasses.Length; i++)
            {
                argsRemove = new BuffListenEventArgs();
                argsRemove.BuffType = BuffType.Class;
                argsRemove.HeroClass = heroClassRace.HeroClasses[i];

                if (ClassValue[heroClassRace.HeroClasses[i]] > 1)
                {
                    ClassValue[heroClassRace.HeroClasses[i]]--;
                    argsRemove.BuffCount = ClassValue[heroClassRace.HeroClasses[i]];
                }
                else
                {
                    argsRemove.BuffCount = 0;
                    ClassValue.Remove(heroClassRace.HeroClasses[i]);
                }
                removeBuff?.Invoke(this, argsRemove);
            }
            for (int i = 0; i < heroClassRace.HeroRaces.Length; i++)
            {
                argsRemove = new BuffListenEventArgs();
                argsRemove.BuffType = BuffType.Race;
                argsRemove.HeroRace = heroClassRace.HeroRaces[i];

                if (RaceValue[heroClassRace.HeroRaces[i]] > 1)
                {
                    RaceValue[heroClassRace.HeroRaces[i]]--;
                    argsRemove.BuffCount = RaceValue[heroClassRace.HeroRaces[i]];
                }
                else
                {
                    argsRemove.BuffCount = 0;
                    RaceValue.Remove(heroClassRace.HeroRaces[i]);
                }
                removeBuff?.Invoke(this, argsRemove);
            }

        }

        /// <summary>
        /// Get classes and races data of network hero
        /// </summary>
        /// <param name="_networkHero"></param>
        /// <returns></returns>
        public static HeroClassRace GetClassRace(NetworkHero _networkHero)
        {   
            Hero[] heroList = TFT.GameManager.Instance.MainGameManager.heroTypes.ToArray();
            for (int i = 0; i < heroList.Length; i++)
            {
                if (_networkHero.name.Equals(heroList[i].name))
                {
                    return new HeroClassRace(heroList[i].HeroClasses, heroList[i].HeroRaces);
                }
            }
            return null;
        }

    }
    [System.Serializable]
    public class NetworkHero
    {
        public int position;
        public string name;
        public HeroLevel HeroLevel;

        public int[] HeroEquipments = new int[3];

        public NetworkHero(Hero _hero)
        {
            name = _hero.name;
            position = _hero.HeroPlace.PlaceId;
            HeroLevel = _hero.HeroLevel;
        }
        public NetworkHero(string _name, int _position, HeroLevel _heroLevel)
        {
            name = _name;
            position = _position;
            HeroLevel = _heroLevel;
        }
    }
    //This class is used for synchronizing netowrk data.
    [System.Serializable]
    public class PlayerHero
    {
        //public Hero[] UsableHeros = new Hero[0];      //List of Player Bought Heros
        //public List<Hero> UsableHeros = new List<Hero>();      //List of Player Bought Heros
        //public Hero[] GameBoardHeros = new Hero[0];      //List of Player Bought Heros
        //public List<Hero> GameBoardHeros = new List<Hero>();   //List of Heros Are into GameBoard
        public List<NetworkHero> UsableHeroes = new List<NetworkHero>();
        public List<NetworkHero> GameBoardHeroes = new List<NetworkHero>();
        public List<Character> battleGameBoardHeroes = new List<Character>();
        public HeroBuffList BuffList = new HeroBuffList();
        public int posId;
        public int TFTCharacterId;
        public PhotonPlayer player;
        public bool isResponse;
        public bool isDead;

        /// <summary>
        /// Add hero to gameboard.
        /// </summary>
        /// <param name="_addedHero"></param>
        public void GameboardAddHero(NetworkHero _addedHero)
        {
            GameBoardHeroes.Add(_addedHero);
            BuffList.AddHeroBuff(_addedHero);
            //AddHeroArray(Hero, ref GameBoardHeros);
        }
       
        public PlayerHero() { }
        public void setPlayer(PhotonPlayer player) {
            this.player = player;
        }
        public void setPersonalInformation(int posId,PhotonPlayer player,int tftPlayerId) {
            this.posId = posId;
            this.player = player;
            TFTCharacterId = tftPlayerId;
        }

        /// <summary>
        /// Remove hero from gameboard.
        /// </summary>
        /// <param name="_removedHero"></param>
        public void GameboardRemoveHero(NetworkHero _removedHero)
        {
            GameBoardHeroes.Remove(_removedHero);
            BuffList.RemoveHeroBuff(_removedHero);
            //RemoveHeroArray(removedHero, ref GameBoardHeros);
        }

        /// <summary>
        /// Move hero in same HeroPlace
        /// </summary>
        /// <param name="_movedHero"></param>
        /// <param name="_newPos"></param>
        public void MoveHero(NetworkHero _movedHero, int _newPos)
        {
            _movedHero.position = _newPos;
            if (GameBoardHeroes.Contains(_movedHero))
            {
                //GameBoardHeroes
            }
        }

        //public void AddHeroArray(Hero hero, ref Hero[] heroArray)
        //{
        //    Hero[] newHeroGameArray = new Hero[heroArray.Length + 1];
        //    for(int i = 0; i< heroArray.Length; i++)
        //    {
        //        newHeroGameArray[i] = heroArray[i];
        //    }
        //    newHeroGameArray[heroArray.Length] = hero;
        //    heroArray = newHeroGameArray;
        //}

        //public void RemoveHeroArray(Hero hero, ref Hero[] heroArray)
        //{
        //    if (GameBoardHeros.Length - 1 > 0)
        //    {
        //        Hero[] newHeroGameArray = new Hero[heroArray.Length - 1];
        //        for (int i = 0; i < newHeroGameArray.Length; i++)
        //        {
        //            newHeroGameArray[i] = heroArray[i];
        //        }
        //        heroArray = newHeroGameArray;

        //    }
        //    else
        //    {
        //        heroArray = new Hero[0];
        //    }
        //}
    }
}

