using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI moneyText;
    public Image icon;
    public Main.GameManager gameManager;
    private void Update()
    {
        moneyText.text = "Money: "+GoogleSheetManager.Instance.money;
        playerNameText.text = "Player name: "+gameManager.userData.name;
        icon.sprite = CollectionStore.Instance.GetSprite(GoogleSheetManager.Instance.Skins.currSkin);
    }
}
