using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeroUI : MonoBehaviour
{
    public HeroClass[] HeroClasses;
    public HeroRace[] HeroRares;
    public BasicInfo BasicInfo;
    public Hero Hero;
    public Image HeroIcon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Hero != null)
        {
            HeroIcon.sprite = Hero.Icon;
        }
    }
}
