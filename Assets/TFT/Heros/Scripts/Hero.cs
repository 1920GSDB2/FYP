using System;
using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine.UI;
using UnityEngine;


public class Hero : Character
{
   // public bool isEnemy;
   // public HeroPlace HeroPlace, LastHeroPlace;         //Current HeroPlace Position of Hero
    public MouseSelect MouseSelect;
    public HeroStatus HeroStatus = HeroStatus.Standby;
    public Rarity Rarity;
    public HeroClass[] HeroClasses;
    public HeroRace[] HeroRaces;
    public HeroLevel HeroLevel;

    public int networkPlaceId;
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


   // bool isAttackCooldown;
    string lastTransform;

    //  List<Node> path;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
       // PhotonNetwork.sendRate = 30;
      //  PhotonNetwork.sendRateOnSerialize = 30;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        hpBar = HeroBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        mpBar = HeroBar.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        animator = GetComponent<Animator>();
        //  HeroBar.transform.LookAt(NetworkManager.Instance.getCamera().transform);

        lastTransform = transform.parent.name;
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
        MouseSelect = GameManager.Instance.GetComponent<MouseSelect>();

        HeroPlace = transform.parent.GetComponent<HeroPlace>();

        MaxHealth = 100 * BasicHealth;
        Health = MaxHealth;
        AttackDamage = 5 * BasicAttackDamage;
        
        //HeroState = HeroState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 targetPostition = new Vector3(HeroBar.transform.position.x, cameraPos.y, HeroBar.transform.position.x);
        HeroBar.transform.LookAt(targetPostition);
        if (HeroState == HeroState.Idle) {
            if (targetEnemy == null)
                targetEnemy = NetworkManager.Instance.getCloestEnemyTarget(isEnemy, transform);
            else {
                HeroState = HeroState.Walking;
                followEnemy();
            }

        }
        if (HeroState == HeroState.Fight) {
            if (!isAttackCooldown)
                 photonView.RPC("RPC_AttackAnimation", PhotonTargets.All);
                animator.SetTrigger("Attack");

        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            //  targetEnemy = testHero;

            //animator.SetBool("Attack",true);
            //  gameObject.SetActive(false);
              Debug.Log(name + " Health " + Health + " / " + MaxHealth);
         //   photonView.RPC("test", PhotonTargets.All);

            //  photonView.RPC("RPC_Animation", PhotonTargets.All);
        }
     
           if (Input.GetKeyDown(KeyCode.O))
           {           
           }
    }
  
    public override void die() {
        this.gameObject.SetActive(false);
        HeroPlace.leavePlace();
        NetworkManager.Instance.battleHeroDie(isEnemy, this);
    }
    [PunRPC]
    public void RPC_ResetStatus() {
        Debug.Log("Reset Status");
        this.gameObject.SetActive(true);
        photonView.RPC("RPC_Heal", PhotonTargets.All, MaxHealth);
        HeroState = HeroState.Nothing;
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
    public void readyForBattle(bool isEnemy, int posId) {
        HeroState = HeroState.Idle;
        this.isEnemy = isEnemy;
        photonView.RPC("RPC_ShowHpBar", PhotonTargets.All, posId);
        Debug.Log("State " + HeroState + " Enemy " + isEnemy);
    }
 
    private void OnMouseEnter()
    {
        if (HeroStatus == HeroStatus.Standby && 
            !(HeroPlace.PlaceType == PlaceType.OnBoard && 
            GameManager.Instance.GameStatus == GameStatus.Transiting)
            )
            MouseSelect.SelectedHero = this;
    }

    private void OnMouseExit()
    {
        MouseSelect.SelectedHero = null;
    }

    #region RPC move hero
    [PunRPC]
    public void RPC_AddToGameBoard(int posId, int placeId)
    {  
        HeroPlace heroPlace = NetworkManager.Instance.GetGameboardHeroPlace(posId, placeId);
        SetHeroPlace(heroPlace);
    }
   
    [PunRPC]
    public void RPC_AddToHeroList(int posId, int placeId)
    {
        HeroPlace heroPlace = NetworkManager.Instance.GetHeroListHeroPlace(posId, placeId);
        if (!photonView.isMine)
            transform.Rotate(new Vector3(0, 180, 0));
        SetHeroPlace(heroPlace);
    }
    

    #endregion
}