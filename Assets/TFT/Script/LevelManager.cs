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
            Shop.Instance.RefreshShop();
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
            if (Level < MaxLevel)
            {
                if (Experience >= ExperienceCurve[Level--])
                {
                    Level++;
                    return true;
                }
            }
            return false;
        }
    }
}
