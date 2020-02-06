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

    //This class is used for synchronizing netowrk data.
    [System.Serializable]
    public class PlayerHero
    {
        public List<Hero> UsableHeros = new List<Hero>();      //List of Player Bought Heros
        public List<Hero> GameBoardHeros = new List<Hero>();   //List of Heros Are into GameBoard
        public HeroBuffList BuffList;

        /// <summary>
        /// Add hero to gameboard.
        /// </summary>
        /// <param name="addedHero"></param>
        public void GameboardAddHero(ref Hero addedHero)
        {
            GameBoardHeros.Add(addedHero);
            BuffList.AddHeroBuff(ref addedHero);
        }

        /// <summary>
        /// Remove hero from gameboard.
        /// </summary>
        /// <param name="removedHero"></param>
        public void GameboardRemoveHero(ref Hero removedHero)
        {
            GameBoardHeros.Remove(removedHero);
            BuffList.RemoveHeroBuff(ref removedHero);
        }
    }
}

