using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class Shop : MonoBehaviour
    {
        private Main.GameManager MainGameManager;
        private SelectManager SelectManager;
        private GameManager TFTGameManager;
        public static Shop Instance;

        public Asset asset;
        public HeroUI[] heroUis = new HeroUI[5];
        private int ExpPrice, RefreshPrice;

        public float delayTime = 0.1f;
        public Button SwitchButton, RemoveButton;
        public bool isShowShop = true;
        private Vector2 currrShopPos, nextShopPos;
        private Vector3 currShopBtnRot, nextShopBtnRot;

        RectTransform shopRect;
        public RectTransform textRect;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            
            MainGameManager = TFT.GameManager.Instance.MainGameManager;
            TFTGameManager = TFT.GameManager.Instance;
            SelectManager = SelectManager.Instance;

            ExpPrice = MainGameManager.ExpPrice;
            RefreshPrice = MainGameManager.RefreshPrice;
            RefreshShop();
            shopRect = GetComponent<RectTransform>();

            currrShopPos = shopRect.anchoredPosition;
            nextShopPos = currrShopPos;
            currShopBtnRot = textRect.eulerAngles;

            RemoveButton.onClick.AddListener(delegate { RemoveHero(); });
        }

        void Update()
        {
            if (SelectManager.DragObject != null && isShowShop)
            {
                SwitchShop();
            }
            if (currrShopPos != nextShopPos)
            {
                shopRect.anchoredPosition = Vector2.Lerp(currrShopPos, nextShopPos, delayTime);
                textRect.eulerAngles = Vector3.Slerp(currShopBtnRot, nextShopBtnRot, delayTime);
                currrShopPos = shopRect.anchoredPosition;
                currShopBtnRot = textRect.eulerAngles;
            }
        }
        public void RemoveHero()
        {
            Hero selectHero = SelectManager.DragObject as Hero;
            
            if (selectHero != null)
            {
                PlayerHero playerHero = GameManager.Instance.PlayerHero;
                for (int i = 0; i < playerHero.UsableHeroes.Count; i++)
                {
                    if (playerHero.UsableHeroes[i].position == selectHero.transform.parent.GetSiblingIndex() &&
                        playerHero.UsableHeroes[i].HeroLevel == selectHero.HeroLevel)
                    {
                        NetworkManager.Instance.SyncPlayerHero(playerHero.UsableHeroes[i], SyncHeroMethod.RemoveHero);
                        break;
                    }
                }
                asset.AssetValue += selectHero.Price * (int)selectHero.HeroLevel + 1;
                PhotonNetwork.Destroy(selectHero.gameObject);
                SelectManager.DragObject = null;
            }
        }

        public void RefreshShop()
        {
            List<Hero> heroTypes = MainGameManager.heroTypes;
            for (int i = 0; i < heroUis.Length; i++)
            {
                int heroId = Random.Range(0, heroTypes.Count);
                //int heroId = Random.Range(0, 3);
                heroUis[i].BasicInfo.name = heroTypes[heroId].name;
                heroUis[i].BasicInfo.rarity = heroTypes[heroId].Rarity;
                heroUis[i].HeroClasses = heroTypes[heroId].HeroClasses;
                heroUis[i].HeroRares = heroTypes[heroId].HeroRaces;
                heroUis[i].Hero = heroTypes[heroId];
                heroUis[i].gameObject.SetActive(true);
            }
        }

        public void BuyRefresh()
        {
            if (asset.AssetValue >= RefreshPrice)
            {
                asset.AssetValue -= RefreshPrice;
                RefreshShop();
            }
        }

        public void BuyExp()
        {
            if (asset.AssetValue >= ExpPrice && !TFTGameManager.LevelManager.IsMaxLevel)
            {
                asset.AssetValue -= ExpPrice;
                TFTGameManager.LevelManager.BuyExp(ExpPrice);
            }
        }

        public void BuyHero(HeroUI heroUi)
        {
            if (asset.AssetValue >= heroUi.BasicInfo.price)
            {

             //  Hero newHero = (Instantiate(heroUi.Hero.gameObject) as GameObject).GetComponent<Hero>();
              //  newHero.name = heroUi.Hero.name;
                Hero newHero = (PhotonNetwork.Instantiate(Path.Combine("Prefabs", heroUi.Hero.name), Vector3.zero, Quaternion.identity, 0)).GetComponent<Hero>();
                newHero.name = heroUi.Hero.name;
                //gameManager.AddHeroBuff(newHero);
                if (TFT.GameManager.Instance.BuyHero(newHero))
                {
                    asset.AssetValue -= heroUi.BasicInfo.price;
                    newHero.Price = heroUi.BasicInfo.price;
                    heroUi.gameObject.SetActive(false);
                }
                else
                {
                    PhotonNetwork.Destroy(newHero.gameObject);
                }
            }
        }

        public void SwitchShop()
        {
            isShowShop = !isShowShop;
            if (isShowShop)
            {
                nextShopPos.y = 0;
                nextShopBtnRot.z = 180;
            }
            else
            {
                nextShopPos.y = -70;
                nextShopBtnRot.z = 0;
            }

        }
    }

}
