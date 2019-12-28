using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFTGameManager : MonoBehaviour
{
    public List<Hero> heroTypes = new List<Hero>();
    public List<Hero> heroes = new List<Hero>();
    public Dictionary<HeroClass, int> classValue = new Dictionary<HeroClass, int>();
    public Dictionary<HeroRace, int> rareValue = new Dictionary<HeroRace, int>();
    public BuffList buffList;
    private BuffersManager buffersManager;

    void Start()
    {
        buffersManager = GetComponent<BuffersManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void AddHeroBuff(Hero hero)
    {
        bool sameType = false;
        foreach(Hero heroType in heroes)
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
                    buffList.UpgradeBuff(heroClass.ToString());
                }
                else
                {
                    classValue.Add(heroClass, 1);
                    foreach (Buffers buffers in buffersManager.buffersClass)
                    {
                        if (buffers.heroClass == heroClass)
                        {
                            buffList.AddBuff(buffers, heroClass.ToString());
                        }
                    }
                }
            }
            foreach (HeroRace heroRare in hero.HeroRaces)
            {
                if (rareValue.ContainsKey(heroRare))
                {
                    rareValue[heroRare]++;
                    buffList.UpgradeBuff(heroRare.ToString());
                }
                else
                {
                    rareValue.Add(heroRare, 1);
                    foreach (Buffers buffers in buffersManager.buffersClass)
                    {
                        if (buffers.heroRare == heroRare)
                        {
                            buffList.AddBuff(buffers, heroRare.ToString());
                        }
                    }
                }

            }
        }
    }
}
