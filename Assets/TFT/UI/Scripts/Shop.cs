using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Asset asset;
    public HeroUI[] heroUis = new HeroUI[5];
    TFTGameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshShop()
    {
        List<Hero> heroTypes = gameManager.gameManager.heroTypes;
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
        if (asset.AssetValue >= 2)
        {
            asset.AssetValue -= 2;
            RefreshShop();
        }
    }

    public void BuyExp()
    {
        if (asset.AssetValue >= 4)
        {
            asset.AssetValue -= 4;
            //BuyExp
        }
    }

    public void BuyHero(HeroUI heroUi)
    {
        if (asset.AssetValue >= heroUi.BasicInfo.price)
        {
            
            Hero newHero = (Instantiate(heroUi.Hero.gameObject) as GameObject).GetComponent<Hero>();
            newHero.name = heroUi.Hero.name;
            //gameManager.AddHeroBuff(newHero);
            if (gameManager.BuyHero(newHero))
            {
                asset.AssetValue -= heroUi.BasicInfo.price;
                heroUi.gameObject.SetActive(false);
                gameManager.HeroUpgrade();
            }
        }
    }
}
