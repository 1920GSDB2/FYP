using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public bool isEnemy;
    public HeroPlace HeroPlace, LastHeroPlace;         //Current HeroPlace Position of Hero
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
    //TFTGameManager gameManager;
    public Hero targetEnemy;

    // Start is called before the first frame update
    void Start()
    {
        lastTransform = transform.parent.name;
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
        MouseSelect = GameManager.Instance.GetComponent<MouseSelect>();
        HeroPlace = transform.parent.GetComponent<HeroPlace>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (HeroStatus == HeroStatus.Fight && targetEnemy == null)
        {
            //EnemyDetecter();
        }
    }

    private void EnemyDetecter()
    {
        LayerMask mask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5, mask);
        targetEnemy = hitColliders[0].GetComponent<Hero>();
        //Debug.Log(hitColliders.Length);
    }
    /// <summary>
    /// Add buff or not, Called by MouseSelect
    /// </summary>
    public void ChangeStatus()
    {
        string currTransform = transform.parent.name;   //Hero Place Name
        if (!currTransform.Equals(lastTransform))
        {
            if (lastTransform.Equals("Hexagon"))
            {
                //GameManager.Instance.ResetBuffList();
                //GameManager.Instance.PlayerHero.GameboardAddHero(ref gameObject.GetComponent<Hero>());
            }
            else
            {
                //GameManager.Instance.AddHeroBuff(this);
            }
        }
        lastTransform = currTransform;
    }

    private void OnMouseEnter()
    {
        if (HeroStatus == HeroStatus.Standby)
            MouseSelect.SelectedHero = this;
    }

    private void OnMouseExit()
    {
        MouseSelect.SelectedHero = null;
    }
}
