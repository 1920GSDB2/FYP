using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TFT
{
    public class Shop : MonoBehaviour
    {
        private Main.GameManager GameManager;
        public static Shop Instance;

        public Asset asset;
        public HeroUI[] heroUis = new HeroUI[5];
        private int ExpPrice, RefreshPrice;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameManager = TFT.GameManager.Instance.MainGameManager;

            ExpPrice = GameManager.ExpPrice;
            RefreshPrice = GameManager.RefreshPrice;
        }

        public void RefreshShop()
        {
            List<Hero> heroTypes = GameManager.heroTypes;
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
            if (asset.AssetValue >= ExpPrice)
            {
                asset.AssetValue -= ExpPrice;
                TFT.GameManager.Instance.PlayerLevel.BuyExp(ExpPrice);
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
                    heroUi.gameObject.SetActive(false);
                }
            }
        }
    }

}
