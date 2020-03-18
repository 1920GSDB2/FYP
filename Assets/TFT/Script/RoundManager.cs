using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TFT
{
    public class RoundManager : MonoBehaviour
    {
        private Main.GameManager MainGameManager;
        private LevelManager LevelManager;

        [SerializeField]
        private  TextMeshProUGUI Opponent, Round;

        private int[] MonsterRound;
        private int CurrentRound;
        private OpponentType CurrentOpponentType
        {
            get
            {
                foreach (int MonsterRound in this.MonsterRound)
                {
                    if (MonsterRound == CurrentRound) return OpponentType.Monster;
                }
                return OpponentType.Player;
            }
        }
        
        void Start()
        {
            MainGameManager = GetComponent<GameManager>().MainGameManager;
            LevelManager = GetComponent<GameManager>().LevelManager;
            MonsterRound = MainGameManager.TFTMonsterRound;

            CurrentRound = 1;
            UIUpdate();
        }
        
        public void RoundUp()
        {
            CurrentRound++;
            LevelManager.RoundEnd();
            Shop.Instance.RefreshShop();
            UIUpdate();
        }

        private void UIUpdate()
        {
            Round.text = CurrentRound.ToString();

            Opponent.text = CurrentOpponentType.ToString();
        }

        
    }
}

