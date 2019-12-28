using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public MouseSelect MouseSelect;
    public Rarity Rarity;
    public HeroClass[] HeroClasses;
    public HeroRace[] HeroRaces;
    public HeroLevel HeroLevel;
    public HeroState HeroState;

    [Range(0, 10)]
    public int BasicHealth;
    [Range(0, 10)]
    public int BasicAttackDamage;
    [Range(0, 10)]
    public int BasicAttackSpeed;
    [Range(0, 10)]
    public int BasicSkillPower;
    [Range(0, 10)]
    public int BasicPhysicalDefense;
    [Range(0, 10)]
    public int BasicMagicDefense;
    
    public float Health { get; set; }
    public float AttackDamage { get; set; }
    public float AttackSpeed { get; set; }
    public float SkillPower { get; set; }
    public float PhysicalDefense { get; set; }
    public float MagicDefense { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        MouseSelect = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MouseSelect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        MouseSelect.SelectedHero = this;
    }

    private void OnMouseExit()
    {
        MouseSelect.SelectedHero = null;
    }
}
