using System.Collections;
using System.Collections.Generic;
using TFT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    public TextMeshProUGUI timerText;
    public GameManager GameManager;
    public Image timerUI;

    //private float PeriodTime;
    //private float remainTime;
    //public float RemainTime
    //{
    //    get { return remainTime; }
    //    set
    //    {
    //        if (value <= 0)
    //        {
    //            ChangeStatus();
                
    //        }
    //        else
    //        {
    //            remainTime = value;
    //        }
    //    }
    //}

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
    
    //private void ChangeStatus()
    //{
    //    if (GameManager.GameStatus == GameStatus.Transiting)
    //    {
    //        switch (GameManager.LastGameStatus)
    //        {
    //            case GameStatus.Readying:
    //                PeriodTime = GameManager.MainGameManager.playingTime;
    //                GameManager.GameStatus = GameStatus.Playing;
    //                break;
    //            case GameStatus.Playing:
    //                PeriodTime = GameManager.MainGameManager.compingTime;
    //                GameManager.GameStatus = GameStatus.Comping;
    //                break;
    //            case GameStatus.Comping:
    //                PeriodTime = GameManager.MainGameManager.readyingTime;
    //                GameManager.GameStatus = GameStatus.Readying;
    //                //change timer count from countdown to countup
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        GameManager.LastGameStatus = GameManager.GameStatus;
    //        PeriodTime = GameManager.MainGameManager.transitionTime;
    //        GameManager.GameStatus = GameStatus.Transiting;

    //    }
    //    remainTime = PeriodTime;
    //}

    IEnumerator TimerCount()
    {
        while (true)
        {
            timerText.text = ((int)GameManager.RemainTime).ToString();
            GameManager.RemainTime -= Time.deltaTime;
            timerUI.fillAmount = GameManager.RemainTime / GameManager.PeriodTime;
            yield return new WaitForSeconds(0.001f);
            GameManager.RemainTime -= 0.001f;
        }
    }
}
