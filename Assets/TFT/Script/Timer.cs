using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TFTGameManager TFTGameManager;
    public Image timerUI;
    public float CountTime
    {
        get { return countTime; }
        set
        {
            if (value <= 0)
            {
                switch (TFTGameManager.GameStatus)
                {
                    case GameStatus.Setup:
                        TFTGameManager.GameStatus = GameStatus.Playing;
                        currentTime = TFTGameManager.gameManager.playtingTime;
                        break;
                    case GameStatus.Playing:
                         TFTGameManager.GameStatus = GameStatus.Extra;
                        currentTime = TFTGameManager.gameManager.extraTime;
                        break;
                    case GameStatus.Extra:
                        TFTGameManager.GameStatus = GameStatus.Setup;
                        currentTime = TFTGameManager.gameManager.setupTime;
                        //change timer count from countdown to countup
                        break;
                }
                countTime = currentTime;
            }
            else
            {
                countTime = value;
            }
        }
    }
    float countTime, currentTime;

    // Start is called before the first frame update
    void Start()
    {
        TFTGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
        currentTime = TFTGameManager.gameManager.setupTime;
        CountTime = currentTime;
        StartCoroutine(TimerCount());
    }

    // Update is called once per frame
    void Update()
    {
        //timerText.text = ((int)CountTime).ToString();
        //CountTime -= Time.deltaTime;
        //timerUI.fillAmount = CountTime / currentTime;
    }

    IEnumerator TimerCount()
    {
        while (true)
        {
            timerText.text = ((int)CountTime).ToString();
            CountTime -= Time.deltaTime;
            timerUI.fillAmount = CountTime / currentTime;
            yield return new WaitForSeconds(0.001f);
            CountTime -= 0.001f;
        }
    }
}
