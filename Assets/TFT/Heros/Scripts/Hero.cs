using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public bool isEnemy;
    public MouseSelect MouseSelect;
    public HeroStatus HeroStatus;
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

    string lastTransform;
    TFTGameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        lastTransform = transform.parent.name;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
        MouseSelect = gameManager.GetComponent<MouseSelect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeStatus()
    {
        string currTransform = transform.parent.name;
        if (!currTransform.Equals(lastTransform))
        {
            if (lastTransform.Equals("Hexagon"))
            {
                gameManager.ResetBuffList();
            }
            else
            {
                gameManager.AddHeroBuff(this);
            }
        }
        lastTransform = currTransform;
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
