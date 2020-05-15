using System;
using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class Hero : Character, ISelectable
{
    // public bool isEnemy;
    // public HeroPlace HeroPlace, LastHeroPlace;         //Current HeroPlace Position of Hero
    public Sprite Icon;
    public GameObject SelectingBox;
    private Collider BoxCollider;
    //public MouseSelect MouseSelect;

    private HeroStatusUI HeroStatusUI;
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
    bool test;
    public Skill skill;
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
    public static GameObject testCanvas;
    public static TextMeshProUGUI text;
    public GameObject testSkill;
    // bool isAttackCooldown;
    string lastTransform;

    //  List<Node> path;
    public virtual void UseSkill() {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
    }
    public virtual void setAttribute() {}

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        text = Resources.Load<TextMeshProUGUI>("otherPrefabs/damageText");
        // PhotonNetwork.sendRate = 30;
        //  PhotonNetwork.sendRateOnSerialize = 30;

    }
    // Start is called before the first frame update
    void Start()
    {
        testCanvas = GameObject.Find("Canvas");
        hpBar = HeroBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        mpBar = HeroBar.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        animator = GetComponent<Animator>();
        //  HeroBar.transform.LookAt(NetworkManager.Instance.getCamera().transform);

        lastTransform = transform.parent.name;

        HeroStatusUI = HeroStatusUI.Instance;
        GameManager = GameManager.Instance;
        SelectManager = SelectManager.Instance;
        EquipmentManager = GetComponent<EquipmentManager>();

        HeroPlace = transform.parent.GetComponent<HeroPlace>();

        resetAttribute();
        setAttribute();
        BoxCollider = GetComponent<Collider>();
        SelectingBox.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPostition = new Vector3(HeroBar.transform.position.x, cameraPos.y, HeroBar.transform.position.x);
        HeroBar.transform.LookAt(targetPostition);
      //  Debug.Log()
       

        switch (HeroState) {
            case HeroState.Idle: idle();
                break;
            case HeroState.Fight:
                if (targetEnemy.Health <= 0)
                {
                    targetDie();
                    HeroState = HeroState.Idle;
                }
                else
                {
                    fight();
                }
                break;
            case HeroState.Control: handleControl();
                break;
        }
       
      
        if (Input.GetKeyDown(KeyCode.I))
        {
            //  targetEnemy = testHero;
            //  HeroBar.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
           
            TextMeshProUGUI a= Instantiate(text);
        
            Camera camera = NetworkManager.Instance.getCamera(NetworkManager.Instance.posId).GetComponent<Camera>();
            Vector2 screenPosition = camera.WorldToScreenPoint(transform.position);
            a.transform.SetParent(testCanvas.transform,false);
            a.transform.position = screenPosition;

            //   photonView.RPC("test", PhotonTargets.All);
            //  photonView.RPC("RPC_Animation", PhotonTargets.All);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            photonView.RPC("RPC_test", PhotonTargets.Others,photonView.viewID);
        }
    }
   
    public void idle() {
        if (targetEnemy == null)
        {
            targetEnemy = NetworkManager.Instance.getCloestEnemyTarget(!isEnemy, transform);
            if (targetEnemy != null)
                photonView.RPC("RPC_SyncTargetEnemy", PhotonTargets.Others, targetEnemy.networkPlaceId);
        }
        else
        {
            HeroState = HeroState.Walking;
            FollowEnemy();
        }
    }
    public void fight() {
        if (Mp >= MaxMp)
        {
            //  photonView.RPC("RPC_userSkill", PhotonTargets.All);
            UseSkill();
        }
        else if (!isAttackCooldown)
        {
            if (targetEnemy.Health <= 0)
            {
                targetDie();
                HeroState = HeroState.Idle;
            }
            else
            {
                // isAttackCooldown = true;
                StartCoroutine(basicAttackCoolDown());
                photonView.RPC("RPC_AttackAnimation", PhotonTargets.All);
              //  animator.SetTrigger("Attack");
                transform.LookAt(targetEnemy.transform);
            }
        }
    }
    public void handleControl() {
        
    }
    public virtual void targetDie() {
        targetEnemy = null;
    }
    [PunRPC]
    public void RPC_userSkill() {
        UseSkill();
    }
    public void resetAttribute() {
       
        MaxHealth = 100 * BasicHealth;
        MaxMp = 100;
        Health = MaxHealth;
        AttackDamage = 10 * BasicAttackDamage;
        AttackSpeed = 0.1f * BasicAttackSpeed;
        attackRange = attackRange * BasicAttackRange;
        SkillPower = BasicSkillPower * 1.25f;
        BoxCollider = GetComponent<Collider>();
    }

    public override void die() {
        HeroState = HeroState.Die;
        this.gameObject.SetActive(false);
        HeroPlace.leavePlace();
        NetworkManager.Instance.battleHeroDie(isEnemy, this);
       // Debug.Log("Die " + name + " state " + HeroState + "player id" + NetworkManager.Instance.playerId);
    }
    [PunRPC]
    public void RPC_ResetStatus() {
        targetEnemy = null;
        tag = "Character";
        gameObject.SetActive(true);
        HeroBar.SetActive(false);
        HeroBar.transform.rotation = Quaternion.identity;
        StartCoroutine(resetStatusCount());
    }
    IEnumerator resetStatusCount() {
        yield return new WaitForSeconds(2f);
        resetStatus();
    }
    public virtual void resetStatus() {
        Debug.Log("Reset Status");
        HeroState = HeroState.Nothing;
        // GetComponent<PhotonTransformView>().enabled = false;
        float recoverHealth = MaxHealth;
        if (Health < 0)
            recoverHealth += Health * -1;
        photonView.RPC("RPC_Heal", PhotonTargets.All, recoverHealth);
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        isEnemy = false;
        isAttackCooldown = false;
        isMirror = false;
        negativeEffects.Clear();
    }
    [PunRPC]
    public void syncNetworkPlaceId(int id)
    {
        networkPlaceId = id;
    }
    [PunRPC]
    public void RPC_SyncTargetEnemy(int posId)
    {
        syncTargetEnemy(posId);
    }

    void syncTargetEnemy(int placeId) {
        targetEnemy = NetworkManager.Instance.getEnemyIndexById(placeId, isEnemy);

      /*  if (isEnemy)
        {
            index = NetworkManager.Instance.opponent.hero.FindIndex(x => x.networkPlaceId == posId);
            if (index != -1)
                targetEnemy = NetworkManager.Instance.opponent.hero[index];
        }
        else
        {
            index = NetworkManager.Instance.selfGameBoardHero.FindIndex(x => x.networkPlaceId == posId);
            if(index!=-1)
            targetEnemy = NetworkManager.Instance.selfGameBoardHero[index];
        }*/ 
    }
    [PunRPC]
    public void RPC_castUnitTargetSkill(int id)
    {
        processShootUnitSkillObject(id);
    }

    void processShootUnitSkillObject(int id)
    { 
        Character target = PhotonView.Find(id).GetComponent<Character>();
       // Debug.Log("skill power " + (SkillPower * 0.5f * AttackDamage));
        skill.shootSkill(target, SkillPower * 0.5f * AttackDamage, isMirror);

    }
    [PunRPC]
    public void RPC_castAoeSkill(int id)
    {
        processAoeSkillObject(id);
    }

    void processAoeSkillObject(int id)
    {
        Character target = PhotonView.Find(id).GetComponent<Character>();
    
        skill.castSkill(target, SkillPower * 0.35f * AttackDamage, isMirror,isEnemy);

    }
    [PunRPC]
    public void RPC_MeleeSkill(int id)
    {
          processMeleeSkill(id);
        
    }

    public virtual void processMeleeSkill(int id)
    {
        Character target = PhotonView.Find(id).GetComponent<Character>();     
        skill.meleeHit(target, SkillPower, isMirror);
        Debug.Log("process Melee");
    }
    [PunRPC]
    public void RPC_MeleeSkillAnimation() {
        animator.SetTrigger("Skill");
    }
    [PunRPC]
    public void RPC_SyncHeroAttribute(byte attributeType,float value) {
        HeroAttribute type = (HeroAttribute)attributeType;
        syncHeroAttribute(type, value);
    }
    public void syncHeroAttribute(HeroAttribute type,float value) {
        switch (type)
        {
            case HeroAttribute.Attack:
                AttackDamage = value;
                break;
            case HeroAttribute.Attack_Speed:
                AttackSpeed = value;
                if (AttackSpeed < 1)
                    value = 1;
                animator.SetFloat("AttackSpeed", value);
                break;
            case HeroAttribute.Critical_Cahnce:
                BasicCritcalChance = (int)value;
                break;
            case HeroAttribute.Magic_Defense:
                MagicDefense = value;
                break;
            case HeroAttribute.Mana:
                MaxMp = value;
                break;
            case HeroAttribute.Health:
                Health = value;
                break;
            case HeroAttribute.maxHp:
                MaxHealth = value;
                if (Health > MaxHealth)
                    Health = MaxHealth;
                break;
            case HeroAttribute.Physic_Defense:
                PhysicalDefense = value;
                break;
            case HeroAttribute.Skill_Damage:
                SkillPower = value;
                break;
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
    public Transform GetEquipmentSlot()
    {
        int index = EquipmentManager.Equipments.Count;
        return EquipmentManager.ItemList.GetChild(index).gameObject.transform;
    }
    private void OnMouseEnter()
    {
        if (HeroStatus == HeroStatus.Fight || HeroStatus == HeroStatus.Dead) 
        {
            return;
        }
        else if (SelectManager.DragObject != null && SelectManager.DragObject as Hero == null)
        {
            int index = EquipmentManager.Equipments.Count;
            if (index < 3)
            { 
                SelectManager.ParentObject = gameObject;
            }
        }
        else
        {
            SelectManager.SelectedObject = this;
            LastHeroPlace = HeroPlace;
        }
    }

    private void OnMouseExit()
    {
        SelectManager.ParentObject = null;
        SelectManager.SelectedObject = null;
    }
    public void PutDown()
    {
        //SelectManager.DragObject = null;
        transform.parent = SelectManager.ParentObject.transform;
        transform.localPosition = Vector3.zero;

        HeroPlace = transform.parent.GetComponent<HeroPlace>();
        GameManager.ChangeHeroPos(this);
        LastHeroPlace = HeroPlace;

        BoxCollider.enabled = true;
        SelectingBox.SetActive(false);
        HeroStatusUI.OffPanelUI();
    }

    public void DragUp()
    {
        BoxCollider.enabled = false;
        SelectingBox.SetActive(true);
        HeroStatusUI.ShowPanelUI(this);
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
       // if (!photonView.isMine)
        //    transform.Rotate(new Vector3(0, 180, 0));
        SetHeroPlace(heroPlace);
    }
   

    #endregion
}