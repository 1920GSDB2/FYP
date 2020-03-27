using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    LocalPlayer,
    RemotePlayer
}

public class PlayerInfo : MonoBehaviour
{
    private int totalHp;
    public int TotalHP
    {
        get { return totalHp; }
        set
        {
            totalHp = value;
            CurrHp = totalHp;
        }
    }
    public int CurrHp;

    public Image playerIcon;
    public TextMeshProUGUI PlayerName;

    [SerializeField]
    private Image imageSlider;
    [SerializeField]
    private TextMeshProUGUI hpText;

    public PlayerType PlayerType
    {
        set
        {
            if (value == PlayerType.LocalPlayer)
            {
                imageSlider.color = Color.yellow;
            }
            else
            {
                imageSlider.color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrHp >= 0)
        {
            imageSlider.fillAmount = (float)CurrHp / TotalHP;
            hpText.text = CurrHp.ToString();
        }
    }
}
