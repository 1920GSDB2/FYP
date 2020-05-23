using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TFT
{

    public class ExpCurve : MonoBehaviour
    {
        public TextMeshProUGUI currentExp, targetExp;
        public Image expFilling;
        GameManager GameManager;

        public static ExpCurve Instace;

        private void Awake()
        {
            Instace = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            GameManager = GameManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            LevelManager levelManager = GameManager.LevelManager;
            int currentLevel = levelManager.Level;
            if(!levelManager.IsMaxLevel) {

                float currentExpValue = currentLevel <= 1 ? levelManager.Experience :
                    levelManager.Experience - levelManager.ExperienceCurve[currentLevel - 2];

                float totalExpValue = currentLevel <= 1 ? levelManager.ExperienceCurve[currentLevel - 1] :
                    levelManager.ExperienceCurve[currentLevel - 1] - levelManager.ExperienceCurve[currentLevel - 2];

                currentExp.text = currentExpValue.ToString();
                targetExp.text = totalExpValue.ToString();
                expFilling.fillAmount = currentExpValue / totalExpValue;
            }
            else
            {
                currentExp.text = "Max";
                targetExp.text = "Max";
                expFilling.fillAmount = 1;
            }

        }
    }

}