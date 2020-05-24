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
    public Button unLockBtn,closeBtn;
    public GameObject buyPanel;  
    public TextMeshProUGUI payText;
    public delegate void Purchase();
    Purchase buy;

    public void Start()
    {
        Instance = this;
        TFTCharacter type =0;
        for (int i = 0; i < (int)TFTCharacter.TotalCharacter; i++) {
            CreateCollection(type++,true);
        }
        closeBtn.onClick.AddListener(ClosePurchaseMenu);
        unLockBtn.onClick.AddListener(BuyCharacter);
       
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
    }
    public void purchaseCollection(Purchase purchase) {
        buy += purchase;
    }
    public void BuyCharacter() {
        buy();
        buy = null;        
    }
   
}
