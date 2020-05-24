using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public Collection collectionPrefab;
    public Transform collectionBag;
    public Button unLockBtn,closeBtn,okBtn;
    public GameObject buyPanel,hintPanel;  
    public TextMeshProUGUI payText,hintText;
    public delegate void Purchase();
    Purchase buy;

    public void Start()
    {
        Instance = this;
        GoogleSheetManager.Instance.finishLoad += OnFinishLoad;
       
        closeBtn.onClick.AddListener(ClosePurchaseMenu);
        unLockBtn.onClick.AddListener(BuyCharacter);
        okBtn.onClick.AddListener(CloseHintMenu);
    }
    public void CreateCollection(TFTCharacter type,bool isLock) {
        Collection collection = Instantiate(collectionPrefab,collectionBag);
        collection.setCollection(type,isLock);

    }

    public void OpenPurchaseMenu(int price ) {
        
        payText.text = "$ " + price + "to unlock this item";
        buyPanel.SetActive(true);
    }
    public void ClosePurchaseMenu() {
        buyPanel.SetActive(false);
        LobbyManager.instance.playClickSound2();
    }
    public void CloseHintMenu()
    {
        hintPanel.SetActive(false);
        LobbyManager.instance.playClickSound2();
    }
    public void purchaseCollection(Purchase purchase) {
        buy += purchase;
    }
    public void ShowBuySuccess() {
        hintPanel.SetActive(true);
        hintText.text = "Buy Successfully";
    }
    public void ShowBuyFail()
    {
        hintPanel.SetActive(true);
        hintText.text = "You have no enough money";
    }
    public void showChangeSuccessfully()
    {
        hintPanel.SetActive(true);
        hintText.text = "change character successfully";
    }
    public void BuyCharacter() {
        buy();
        buy = null;
        //Debuct Money
        //Add Skin  
        //Pass enum to database
    }
    public void OnFinishLoad(object sender,EventArgs e) {
        TFTCharacter type = 0;
        List<string> playerCharacter = GoogleSheetManager.Instance.Skins.SkinList;
        foreach (string s in playerCharacter)
        {
            Debug.Log("i have " + s);
        }

        for (int i = 0; i < (int)TFTCharacter.TotalCharacter; i++)
        {
            if (playerCharacter.Contains(CollectionStore.Instance.GetName(type)))
                CreateCollection(type++, false);
            else
                CreateCollection(type++, true);
        }
    }
}
