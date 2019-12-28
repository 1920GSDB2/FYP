using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    private const int TOTAL_HP = 100;
    public Image imageSlider;
    public Text hpText;
    [Range(0, TOTAL_HP)]
    public int hpValue = TOTAL_HP;
    public PlayerType playerType;

    private void Start()
    {
        if (playerType == PlayerType.LocalPlayer)
        {
            imageSlider.color = Color.yellow;
        }
        else
        {
            imageSlider.color = Color.red;
        }
    }

    void Update()
    {
        if (hpValue >= 0)
        {
            imageSlider.fillAmount = (float)hpValue / TOTAL_HP;
            hpText.text = hpValue.ToString();
        }
    }
}
