using System.Collections;
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
                    if (ClassValue.ContainsKey(heroClassRace.HeroClasses[i])) ClassValue[heroClassRace.HeroClasses[i]]++;
                    else ClassValue.Add(heroClassRace.HeroClasses[i], 1);
                }
                for (int i = 0; i < heroClassRace.HeroRaces.Length; i++)
                {
                    if (RaceValue.ContainsKey(heroClassRace.HeroRaces[i])) RaceValue[heroClassRace.HeroRaces[i]]++;
                    else RaceValue.Add(heroClassRace.HeroRaces[i], 1);
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
                if (ClassValue[heroClassRace.HeroClasses[i]] > 1) ClassValue[heroClassRace.HeroClasses[i]]--;
                else ClassValue.Remove(heroClassRace.HeroClasses[i]);
            }
            for (int i = 0; i < heroClassRace.HeroRaces.Length; i++)
            {
                if (RaceValue[heroClassRace.HeroRaces[i]] > 1) RaceValue[heroClassRace.HeroRaces[i]]--;
                else RaceValue.Remove(heroClassRace.HeroRaces[i]);
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
        public HeroBuffList BuffList;

        /// <summary>
        /// Add hero to gameboard.
        /// </summary>
        /// <param name="addedHero"></param>
        public void GameboardAddHero(NetworkHero addedHero)
        {
            GameBoardHeroes.Add(addedHero);
            BuffList.AddHeroBuff(addedHero);
            //AddHeroArray(Hero, ref GameBoardHeros);
        }

        /// <summary>
        /// Remove hero from gameboard.
        /// </summary>
        /// <param name="removedHero"></param>
        public void GameboardRemoveHero(NetworkHero removedHero)
        {
            GameBoardHeroes.Remove(removedHero);
            BuffList.RemoveHeroBuff(removedHero);
            //RemoveHeroArray(removedHero, ref GameBoardHeros);
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

