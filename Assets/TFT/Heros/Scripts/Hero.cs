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
    public Sprite Icon;
    public GameObject SelectingBox;
    private Collider BoxCollider;
    //public MouseSelect MouseSelect;

    private GameManager GameManager;
    private SelectManager SelectManager;

    private HeroStatus heroStatus = HeroStatus.Standby;
    public HeroStatus HeroStatus
    {
        get { return heroStatus; }
        set
        {
            heroStatus = value;
            EquipmentManager.ItemList.gameObject.SetActive(value == HeroStatus.Standby);
        }
    }
    public Rarity Rarity;
    public HeroClass[] HeroClasses;
    public HeroRace[] HeroRaces;
    public HeroLevel HeroLevel;

    public EquipmentManager EquipmentManager;

    

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
    [Range(0, 10)]
    public int BasicCritcalChance;
    [Range(1, 4)]
    public int BasicAttackRange;


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
        //MouseSelect = GameManager.Instance.GetComponent<MouseSelect>();

        GameManager = GameManager.Instance;
        SelectManager = SelectManager.Instance;
        EquipmentManager = GetComponent<EquipmentManager>();

        HeroPlace = transform.parent.GetComponent<HeroPlace>();

        resetAttribute();
         BoxCollider = GetComponent<Collider>();
        //HeroState = HeroState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(HeroBar.transform.position.x, cameraPos.y, HeroBar.transform.position.x);
        HeroBar.transform.LookAt(targetPostition);
        
        if (HeroState == HeroState.Idle) {
            if (targetEnemy == null)
            {
                targetEnemy = NetworkManager.Instance.getCloestEnemyTarget(isEnemy, transform);
                if(targetEnemy!=null)
                photonView.RPC("RPC_SyncTargetEnemy", PhotonTargets.Others, targetEnemy.networkPlaceId,isEnemy);
            }
            else
            {
                HeroState = HeroState.Walking;
                followEnemy();
            }

        }
       /* if (HeroState == HeroState.Walking && !animator.GetBool("Walk"))
        {           
            checkWithInAttackRange();
        }*/

        if (HeroState == HeroState.Fight) {
            if (!isAttackCooldown)
                if (targetEnemy.Health <= 0)
                {
                    targetEnemy = null;
                    HeroState = HeroState.Idle;
                }
                else
                {
                    // isAttackCooldown = true;
                    StartCoroutine(basicAttackCoolDown());
                    photonView.RPC("RPC_AttackAnimation", PhotonTargets.Others);
                    animator.SetTrigger("Attack");
                    transform.LookAt(targetEnemy.transform);
                }

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //  targetEnemy = testHero;

            //animator.SetBool("Attack",true);
            //  gameObject.SetActive(false);
            //    Debug.Log(name + " Health " + Health + " / " + MaxHealth);

        
            //Debug.Log(name+" Position "+transform.position+" Target POs"+targetEnemy.transform.position);
            //Debug.Log(name + " Hero Pace " + HeroPlace.transform.position + " Target Hero pLace" + targetEnemy.HeroPlace.transform.position);
            //   photonView.RPC("test", PhotonTargets.All);
            //  photonView.RPC("RPC_Animation", PhotonTargets.All);
        }

        SelectingBox.SetActive(!BoxCollider.enabled);

        if (SelectManager.DragObject != null &&
            SelectManager.DragObject == gameObject &&
            //transform.parent != LastHeroPlace.transform)
            SelectManager.IsFinishDrag)
        {
            SelectManager.DragObject = null;
            HeroPlace = transform.parent.GetComponent<HeroPlace>();
            GameManager.ChangeHeroPos(this);
            Debug.Log("GameManager.ChangeHeroPos");
            LastHeroPlace = HeroPlace;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {           
        }
    }
    public void resetAttribute() {
        MaxHealth = 100 * BasicHealth;
        MaxMp = 100;
        Health = MaxHealth;
        AttackDamage = 10 * BasicAttackDamage;
        AttackSpeed = 0.1f * BasicAttackSpeed;
        attackRange = attackRange * BasicAttackRange;
        BoxCollider = GetComponent<Collider>();
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
        HeroBar.SetActive(false);
        photonView.RPC("RPC_Heal", PhotonTargets.All, MaxHealth);
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        HeroState = HeroState.Nothing;
        isEnemy = false;
        isAttackCooldown = false;
        isMirror = true;
    }
    [PunRPC]
    public void syncNetworkPlaceId(int id)
    {
        networkPlaceId = id;
    }
    [PunRPC]
    public void RPC_SyncTargetEnemy(int posId,bool isEnemy)
    {
        syncTargetEnemy(posId, isEnemy);
    }
    void syncTargetEnemy(int posId,bool isEnemy) {
        int index;
        if (isEnemy)
        {
            index = NetworkManager.Instance.opponent.hero.FindIndex(x => x.networkPlaceId == posId);
            if (index != 1)
                targetEnemy = NetworkManager.Instance.opponent.hero[index];
        }
        else
        {
            index = NetworkManager.Instance.selfGameBoardHero.FindIndex(x => x.networkPlaceId == posId);
            if(index!=1)
            targetEnemy = NetworkManager.Instance.selfGameBoardHero[index];
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

        //if (HeroStatus == HeroStatus.Standby && 
        //    !(HeroPlace.PlaceType == PlaceType.OnBoard && 
        //    GameManager.Instance.GameStatus == GameStatus.Transiting)
        //    )
        //    MouseSelect.SelectedHero = this;
        if (HeroStatus == HeroStatus.Fight || HeroStatus == HeroStatus.Dead) 
        {
            return;
        }
        else if (SelectManager.DragObject != null && SelectManager.DragObject.GetComponent<Hero>() == null)
        {
            int index = EquipmentManager.Equipments.Count;
            if (index < 3)
            { 
                SelectManager.ParentObject = EquipmentManager.ItemList.GetChild(index).gameObject;
            }
        }
        else
        {
            SelectManager.SelectedObject = gameObject;
            LastHeroPlace = HeroPlace;
        }
    }

    private void OnMouseExit()
    {
        //MouseSelect.SelectedHero = null;
        if (SelectManager.SelectedObject == gameObject)
        {
            SelectManager.SelectedObject = null;
        }
        if(SelectManager.ParentObject == gameObject)
        {
            SelectManager.ParentObject = null;
        }
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