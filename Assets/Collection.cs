
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using ExitGames.Client.Photon;

public class Collection : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    TFTCharacter type;
    public Image icon;
    public Button button;
    public bool isLock;
    
    private void Start()
    {
        button.onClick.AddListener(checkUse);
    }
    public void setCollection(TFTCharacter type,bool isLock) {
        this.type = type;
        nameText.text = CollectionStore.Instance.GetName(type);
        this.icon.sprite = CollectionStore.Instance.GetSprite(type);
        ColorBlock color = button.colors;
        this.isLock = isLock;
        if (isLock)
        {
            color.normalColor = Color.red;
            button.colors = color;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Lock";
        }
        else {
            color.normalColor = Color.green;
            button.colors = color;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
        }

    }

    public void checkUse() {
        if (isLock)
        {
            ShopManager.Instance.OpenPurchaseMenu(CollectionStore.Instance.GetPrice(type));
            ShopManager.Instance.purchaseCollection(BuyThisCollection);
        }
        else
            changeCharacter();

    }

    public void BuyThisCollection()
    {
        Debug.Log("BuyThisCollection");
        isLock = !GoogleSheetManager.Instance.BuyCharacter(type);
        if (!isLock)
        {
            Unlock();
        }
        else{
            ShopManager.Instance.ShowBuyFail();
        }
    }
   
    public void changeCharacter() {
        GoogleSheetManager.Instance.ChangeCharacter(type);
        ShopManager.Instance.showChangeSuccessfully();
        Hashtable playerProp = PhotonNetwork.player.CustomProperties;
        PhotonNetwork.player.SetCustomProperties(new Hashtable
        {
            {"NAME", playerProp["NAME"] },
            {"READY_FOR_START", playerProp["READY_FOR_START"] },
            {"Character_Name",GoogleSheetManager.Instance.Skins.currSkin}
        });
    }
    public void Unlock() {
        ShopManager.Instance.ClosePurchaseMenu();
        ColorBlock color = button.colors;
        color.normalColor = Color.green;
        button.colors = color;
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
        isLock = false;
        ShopManager.Instance.ShowBuySuccess();
    }
}
