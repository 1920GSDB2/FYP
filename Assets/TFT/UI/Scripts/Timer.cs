using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class Timer : MonoBehaviour
    {
        public static Timer Instance;
        public TextMeshProUGUI timer;
        private GameManager GameManager;
        public Image timerUI;
        
        private void Awake()
        {
            Instance = this;
            GameManager = GameManager.Instance;
        }

        // Start is called before the first frame update
        void Start()
        {

            StartCoroutine(TimerCount());
        }
        
        IEnumerator TimerCount()
        {
            while (true)
            {
                timer.text = ((int)GameManager.RemainTime).ToString();
                //GameManager.RemainTime -= Time.deltaTime;
                timerUI.fillAmount = GameManager.RemainTime / GameManager.PeriodTime;
                GameManager.RemainTime -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }

}