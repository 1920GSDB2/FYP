using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    [System.Serializable]
    public class HeroBuffList
    {
        List<string> HeroType = new List<string>();
        public Dictionary<HeroClass, int> classValue = new Dictionary<HeroClass, int>();   //Number of class in gameboard hero
        public Dictionary<HeroRace, int> raceValue = new Dictionary<HeroRace, int>();      //Number of race in gameboard hero

        /// <summary>
        /// Call the method, while adding hero to the gameboard
        /// </summary>
        /// <param name="addedHero"></param>
        public void AddHero(ref Hero addedHero)
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
                    if (classValue.ContainsKey(addedHero.HeroClasses[i])) classValue[addedHero.HeroClasses[i]]++;
                    else classValue.Add(addedHero.HeroClasses[i], 1);
                }
                for (int i = 0; i < addedHero.HeroRaces.Length; i++)
                {
                    if (raceValue.ContainsKey(addedHero.HeroRaces[i])) raceValue[addedHero.HeroRaces[i]]++;
                    else raceValue.Add(addedHero.HeroRaces[i], 1);
                }
            }
        }

        /// <summary>
        /// Call the method, while removing hero from the gameboard
        /// </summary>
        /// <param name="removedHero"></param>
        public void RemoveHero(ref Hero removedHero)
        {
            for (int i = 0; i < removedHero.HeroClasses.Length; i++)
            {
                if (classValue[removedHero.HeroClasses[i]] > 1) classValue[removedHero.HeroClasses[i]]--;
                else classValue.Remove(removedHero.HeroClasses[i]);
            }
            for (int i = 0; i < removedHero.HeroRaces.Length; i++)
            {
                if (raceValue[removedHero.HeroRaces[i]] > 1) raceValue[removedHero.HeroRaces[i]]--;
                else raceValue.Remove(removedHero.HeroRaces[i]);
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
    }
}

