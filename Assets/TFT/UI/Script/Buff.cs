using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Buff : MonoBehaviour
{
    public Image IconImg;
    public Image LevelImg;
    public Text NameText;
    public Text CurrentText;
    public Text TotalText;

    public Sprite NewbieLevel;
    public Sprite BronzeLevel;
    public Sprite SliverLevel;
    public Sprite GoldenLevel;

    public Sprite[] Levels;
    public Sprite[] ClassIcons;
    public Sprite[] RaceIcons;

    public HeroClass HeroClass;
    public HeroRace HeroRare;
    public BuffLevel BuffLevel;
    [Range(1, 6)]
    public int TotalValue;
    [Range(1, 6)]
    public int CurrentValue;

    // Start is called before the first frame update
    void Start()
    {
        LoadBuffer();
    }

    // Update is called once per frame
    void Update()
    {
        LoadBuffer();
        if (TotalText != null) TotalText.text = TotalValue.ToString();
        if (CurrentText != null) CurrentText.text = CurrentValue.ToString();
    }

    void LoadBuffer()
    {
        if (HeroClass != HeroClass.None)
        {
            switch (HeroClass)
            {
                case HeroClass.Assassin:
                    IconImg.sprite = ClassIcons[0];
                    NameText.text = "Assassin";
                    break;
                case HeroClass.Brawler:
                    IconImg.sprite = ClassIcons[1];
                    NameText.text = "Brawler";
                    break;
                case HeroClass.Ranger:
                    IconImg.sprite = ClassIcons[2];
                    NameText.text = "Ranger";
                    break;
                case HeroClass.Supporter:
                    IconImg.sprite = ClassIcons[3];
                    NameText.text = "Supporter";
                    break;
                case HeroClass.Warrior:
                    IconImg.sprite = ClassIcons[4];
                    NameText.text = "Warrior";
                    break;
                case HeroClass.Wizard:
                    IconImg.sprite = ClassIcons[5];
                    NameText.text = "Wizard";
                    break;
            }
        }
        else if (HeroRare != HeroRace.None)
        {
            switch (HeroRare)
            {
                case HeroRace.Demon:
                    IconImg.sprite = RaceIcons[0];
                    NameText.text = "Demon";
                    break;
                case HeroRace.Divinity:
                    IconImg.sprite = RaceIcons[1];
                    NameText.text = "Divinity";
                    break;
                case HeroRace.Human:
                    IconImg.sprite = RaceIcons[2];
                    NameText.text = "Human";
                    break;
                case HeroRace.Rebel:
                    IconImg.sprite = RaceIcons[3];
                    NameText.text = "Rebel";
                    break;
                case HeroRace.Spirit:
                    IconImg.sprite = RaceIcons[4];
                    NameText.text = "Spirit";
                    break;
                case HeroRace.Wild:
                    IconImg.sprite = RaceIcons[5];
                    NameText.text = "Wild";
                    break;
                case HeroRace.Dwarf:
                    IconImg.sprite = RaceIcons[6];
                    NameText.text = "Dwarf";
                    break;
            }
        }
        if (LevelImg != null)
        {
            switch (BuffLevel)
            {
                case BuffLevel.Newbie:
                    LevelImg.sprite = Levels[0];
                    IconImg.color = Color.white;
                    break;
                case BuffLevel.Bronze:
                    LevelImg.sprite = Levels[1];
                    IconImg.color = Color.black;
                    break;
                case BuffLevel.Sliver:
                    LevelImg.sprite = Levels[2];
                    IconImg.color = Color.black;
                    break;
                case BuffLevel.Golden:
                    LevelImg.sprite = Levels[3];
                    IconImg.color = Color.black;
                    break;
            }
        }
        
    }
}
