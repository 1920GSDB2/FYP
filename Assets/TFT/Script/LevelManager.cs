using UnityEngine;

namespace TFT
{
    [System.Serializable]
    public class LevelManager
    {
        [SerializeField]
        private int level = 1;                          //Player Currrent Level
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        [SerializeField]
        private int experience = 0;                     //Player Current Total Experience
        public int Experience
        {
            get { return experience; }
            set { experience = value; }
        }

        private readonly int[] ExperienceCurve;         //Level Curve
        private readonly int MaxLevel;                  //Maximum Level

        public bool IsMaxLevel { get { return Level > MaxLevel ? true : false; } }

        /// <summary>
        /// Constructor of initializing
        /// </summary>
        /// <param name="_experienceCurve"></param>
        public LevelManager(int[] _experienceCurve)
        {
            ExperienceCurve = _experienceCurve;
            MaxLevel = _experienceCurve.Length;
        }

        /// <summary>
        /// Called, while round is ended
        /// </summary>
        public void RoundEnd()
        {
            Experience += 2;
            CheckLevelUp();
        }

        /// <summary>
        /// Add exp while buy experience in shop
        /// </summary>
        /// <param name="_exp"></param>
        public void BuyExp(int _exp)
        {
            Experience += _exp;
            CheckLevelUp();
        }

        /// <summary>
        /// Check the level can be up
        /// </summary>
        /// <returns></returns>
        private bool CheckLevelUp()
        {
            if (!IsMaxLevel)
            {
                if (Experience >= ExperienceCurve[Level - 1])
                {
                    Level++;
                    return true;
                }
            }
            return false;
        }

    }
}
