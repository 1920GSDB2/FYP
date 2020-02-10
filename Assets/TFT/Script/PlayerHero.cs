using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
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
        public void AddHeroBuff(ref Hero addedHero)
        {
            bool isSameType = false;
            for(int i =0; i< HeroType.Count; i++)
            {
                if (HeroType[i].Equals(addedHero.name))
                {
                    isSameType = true;
                    break;
                }
            }
            if (!isSameType)
            {
                for(int i = 0; i< addedHero.HeroClasses.Length; i++)
                {
                    if (ClassValue.ContainsKey(addedHero.HeroClasses[i])) ClassValue[addedHero.HeroClasses[i]]++;
                    else ClassValue.Add(addedHero.HeroClasses[i], 1);
                }
                for (int i = 0; i < addedHero.HeroRaces.Length; i++)
                {
                    if (RaceValue.ContainsKey(addedHero.HeroRaces[i])) RaceValue[addedHero.HeroRaces[i]]++;
                    else RaceValue.Add(addedHero.HeroRaces[i], 1);
                }
            }
        }

        /// <summary>
        /// Call the method, while removing hero from the gameboard
        /// </summary>
        /// <param name="removedHero"></param>
        public void RemoveHeroBuff(ref Hero removedHero)
        {
            for (int i = 0; i < removedHero.HeroClasses.Length; i++)
            {
                if (ClassValue[removedHero.HeroClasses[i]] > 1) ClassValue[removedHero.HeroClasses[i]]--;
                else ClassValue.Remove(removedHero.HeroClasses[i]);
            }
            for (int i = 0; i < removedHero.HeroRaces.Length; i++)
            {
                if (RaceValue[removedHero.HeroRaces[i]] > 1) RaceValue[removedHero.HeroRaces[i]]--;
                else RaceValue.Remove(removedHero.HeroRaces[i]);
            }
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
        //public HeroBuffList BuffList;

        /// <summary>
        /// Add hero to gameboard.
        /// </summary>
        /// <param name="addedHero"></param>
        public void GameboardAddHero(ref Hero addedHero)
        {
            //GameBoardHeros.Add(addedHero);
            //AddHeroArray(Hero, ref GameBoardHeros);
            //Dictionary<add>
            //BuffList.AddHeroBuff(ref addedHero);
        }

        /// <summary>
        /// Remove hero from gameboard.
        /// </summary>
        /// <param name="removedHero"></param>
        public void GameboardRemoveHero(ref Hero removedHero)
        {
            //GameBoardHeros.Remove(removedHero);
            //RemoveHeroArray(removedHero, ref GameBoardHeros);
            //BuffList.RemoveHeroBuff(ref removedHero);
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

