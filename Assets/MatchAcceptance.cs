using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchAcceptance : MonoBehaviour
{
    public enum AcceptanceStatus
    {
        WAIT,
        ACCPET,
        DECLINE
    }

    public static MatchAcceptance instance;
    private bool enable;
    public bool Enable
    {
        get { return enable; }
        set
        {
            enable = value;
            gameObject.SetActive(value);
        }
    }

    public int WaitTime = 10;
    private float CountUpTime;

    private AcceptanceStatus currStatus;
    public AcceptanceStatus CurrStatus
    {
        get { return currStatus; }
        private set
        {
            currStatus = value;
            switch (CurrStatus)
            {
                case AcceptanceStatus.WAIT:
                    AcceptanceStatusText.text = "Enter The Game";
                    break;
                case AcceptanceStatus.ACCPET:
                    AcceptanceStatusText.text = "Accept The Game";
                    break;
                case AcceptanceStatus.DECLINE:
                    AcceptanceStatusText.text = "Decline The Game";
                    break;
            }
        }
    }

    [SerializeField]
    private TextMeshProUGUI AcceptanceStatusText;
    [SerializeField]
    private Button AcceptButton, DeclineButton;
    private Button[] Buttons = new Button[2];

    [SerializeField]
    private Image TimerUi;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;
        Enable = false;
        Buttons[0] = AcceptButton;
        Buttons[1] = DeclineButton;

        AcceptButton.onClick.AddListener(delegate { Accept(); });
        DeclineButton.onClick.AddListener(delegate { Decline(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Accept()
    {
        CurrStatus = AcceptanceStatus.ACCPET;
        foreach(Button button in Buttons)
        {
            button.enabled = false;
        }
    }

    public void Decline()
    {
        CurrStatus = AcceptanceStatus.DECLINE;
        foreach (Button button in Buttons)
        {
            button.enabled = false;
        }
    }

    public void InitialPanel()
    {
        Enable = true;
        CurrStatus = AcceptanceStatus.WAIT;
        CountUpTime = WaitTime;
        foreach (Button button in Buttons)
        {
            button.enabled = true;
        }
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (CountUpTime > WaitTime)
        {
            CountUpTime -= 0.01f;
            TimerUi.fillAmount = CountUpTime / WaitTime;
            if (CurrStatus != AcceptanceStatus.WAIT) break;
            yield return new WaitForSeconds(0.01f);
        }
        if (CurrStatus == AcceptanceStatus.WAIT) CurrStatus = AcceptanceStatus.DECLINE;
    }
}
