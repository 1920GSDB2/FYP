using System;
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
    public Hero testHero;
  //  List<Node> path;

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
        if (Input.GetKeyDown(KeyCode.K))
        {
            targetEnemy = testHero;
            if (targetEnemy != null)
            {
                
                Debug.Log(HeroStatus);
                if (HeroStatus == HeroStatus.Standby)
                {
                    PathFindingManager.Instance.requestNextStep(HeroPlace, targetEnemy.HeroPlace, onStepFind);
                }
                targetEnemy = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            targetEnemy = testHero;
            if (targetEnemy != null)
            {
                PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, onPathFind);
               
               // targetEnemy = null;
            }
        }
    }
    //called by OathfindingManager when request a path
    public void onPathFind(List<Node> path,bool isFindPath) {
        if (isFindPath)
        {
            foreach (Node node in path)
            {
                node.heroPlace.settColor(Color.blue);

            }
            // StartCoroutine(followPath());
            if (path != null) 
            StartCoroutine(followStep(path[0]));
            //path = null;
        }
    }
    //called by OathfindingManager when request next step
    public void onStepFind(Node step, bool isFindStep)
    {
        if (isFindStep)
        {
            step.heroPlace.settColor(Color.blue);  
            StartCoroutine(followStep(step));
            
        }
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
    //a hero move to the heroplace;
    public void moveToThePlace(Hero hero, HeroPlace newHeroPlace) {
        HeroPlace.leavePlace();
        newHeroPlace.setHeroOnPlace(hero);

        LastHeroPlace = HeroPlace;
        HeroPlace = newHeroPlace;
        Debug.Log("change");
    }
    // Hero will follow the whole path and walk to the destination
    IEnumerator followPath(List<Node> path) {
        int index = 0;
        Node currentNode = path[index];

            while (true)
            {
            if (transform.position == currentNode.heroPlace.transform.position)
            {
                index++;
                currentNode = path[index];
               // moveToThePlace(this,path[index].heroPlace);
            }
            transform.position = Vector3.MoveTowards(transform.position, currentNode.heroPlace.transform.position, 5*Time.deltaTime);
            yield return null;
        }                    
    }
    //Hero will just move to the next step
    IEnumerator followStep(Node step)
    {
        
        while (true)
        {
            if (transform.position != step.heroPlace.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, step.heroPlace.transform.position, 5 * Time.deltaTime);
            }
            else
            {
               moveToThePlace(this, step.heroPlace);
               PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, onPathFind);
               break;
            }
            yield return null;
        }
        
    }
}
