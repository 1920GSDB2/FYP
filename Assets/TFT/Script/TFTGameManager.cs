using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFTGameManager : MonoBehaviour
{
    public List<Hero> heroTypes = new List<Hero>();                             //List of total heroes
    public List<Hero> gbHero = new List<Hero>();                                //List of gameboard's heroes
    List<Hero> heroes = new List<Hero>();                                       //List of player's Heroes
    Dictionary<HeroClass, int> classValue = new Dictionary<HeroClass, int>();   //Number of class in gameboard hero
    Dictionary<HeroRace, int> raceValue = new Dictionary<HeroRace, int>();      //Number of race in gameboard hero

    [SerializeField]
    BuffList BuffList;
    Transform HeroList, GameBoard;
    BuffersManager buffersManager;

    void Start()
    {
        buffersManager = GetComponent<BuffersManager>();
        HeroList = GameObject.FindGameObjectWithTag("HeroList").transform;
        GameBoard = GameObject.FindGameObjectWithTag("GameBoard").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Check whether player can buy a hero.
    /// </summary>
    /// <param name="_hero"></param>
    /// <returns></returns>
    public bool BuyHero(Hero _hero)
    {
        foreach(Transform child in HeroList)
        {
            if (child.childCount == 0)
            {
                heroes.Add(_hero);
                _hero.gameObject.transform.parent = child;
                _hero.gameObject.transform.localPosition = Vector3.zero;
                return true;
            }
        }
        DestroyImmediate(_hero.gameObject);
        return false;
    }

    /// <summary>
    /// Check the number same level hero, if it reachs specified number, the hero will level up.
    /// </summary>
    public void HeroUpgrade()
    {
        foreach(Hero heroType in heroTypes)
        {
            List<Hero> lv1 = new List<Hero>();
            List<Hero> lv2 = new List<Hero>();

            foreach (Hero hero in heroes)
            {
                if (hero != null && hero.name.Equals(heroType.name))
                {
                    if (hero.HeroState == HeroState.Idle)
                    {
                        switch (hero.HeroLevel)
                        {
                            case HeroLevel.Level1:
                                lv1.Add(hero);
                                break;
                            case HeroLevel.Level2:
                                lv2.Add(hero);
                                break;
                        }
                    }
                }
            }
            if (lv1.Count >= 3)
            {
                Debug.Log("Upgrade to Lv2");
                lv2.Add(HeroLevelUp(lv1, HeroLevel.Level2));
            }
            if (lv2.Count >= 3)
            {
                Debug.Log("Upgrade to Lv3");
                HeroLevelUp(lv2, HeroLevel.Level3);
            }
        }
    }

    /// <summary>
    /// Process of hero level up.
    /// </summary>
    /// <param name="heroLevel"></param>
    /// <param name="nextLevel"></param>
    /// <returns></returns>
    private Hero HeroLevelUp(List<Hero> heroLevel, HeroLevel nextLevel)
    {
        Hero temp = null;
        for (int i = 0; i < 2; i++)
        {
            heroes.Remove(heroLevel[i]);
            Destroy(heroLevel[i].gameObject);
        }
        temp = heroLevel[2];
        temp.HeroLevel = nextLevel;
        return temp;
    }

    /// <summary>
    /// When hero is added into gameboard, the buff of hero will be added into the buff list.
    /// </summary>
    /// <param name="hero"></param>
    public void AddHeroBuff(Hero hero)
    {
        bool sameType = false;
        foreach (Hero heroType in gbHero)
        {
            if (heroType != null && heroType.name.Equals(hero.name))
            {
                sameType = true;
                break;
            }
            
        }
        if (!sameType)
        {
            foreach (HeroClass heroClass in hero.HeroClasses)
            {
                if (classValue.ContainsKey(heroClass))
                {
                    classValue[heroClass]++;
                    BuffList.UpgradeBuff(heroClass.ToString());
                }
                else
                {
                    classValue.Add(heroClass, 1);
                    foreach (Buffers buffers in buffersManager.buffersClass)
                    {
                        if (buffers.heroClass == heroClass)
                        {
                            BuffList.AddBuff(buffers, heroClass.ToString());
                        }
                    }
                }
            }
            foreach (HeroRace heroRare in hero.HeroRaces)
            {
                if (raceValue.ContainsKey(heroRare))
                {
                    raceValue[heroRare]++;
                    BuffList.UpgradeBuff(heroRare.ToString());
                }
                else
                {
                    raceValue.Add(heroRare, 1);
                    foreach (Buffers buffers in buffersManager.buffersClass)
                    {
                        if (buffers.heroRare == heroRare)
                        {
                            BuffList.AddBuff(buffers, heroRare.ToString());
                        }
                    }
                }

            }
        }
    }

    public void ResetBuffList()
    {
        classValue.Clear();
        raceValue.Clear();
        BuffList.ClearBuff();
        StartCoroutine(AddBuffBack());
    }

    //Destroy in bufflist not fast enough, it causes some bugs.
    IEnumerator AddBuffBack()
    {
        yield return 0.0001;
        foreach (Transform child in GameBoard)
        {
            if (child.childCount != 0)
            {
                AddHeroBuff(child.GetChild(0).GetComponent<Hero>());
            }
        }
    }
}
