using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;
using UnityEngine.UI;
using System.IO;
using System;

public class Character : MonoBehaviour
{
    #region Attribute
    #region Changable Attribute in Battle
    [SerializeField]
    private float health;
    public float Health                                 //CurrentHp
    {
        get { return health; }
        set
        {
            if (value != Health)
            {
                health = value;
                hpChange?.Invoke(this, EventArgs.Empty);
            }
            if(Health <= 0)
            {           
                if (HeroState!= HeroState.Die)
                {
                   // Debug.Log("Call Die method " +name   );
                     die();
                    
                }
            }
        }
    }                              
    public float Sheild;                                //Shield
    private float mp;
    public float Mp
    {
        get { return mp; }
        set
        {
            if (value != Mp)
            {
                mp = value;
                if (Mp >= MaxMp && !isSlience)
                {
                    if(!isMirror)
                     UseSkill();
                    useSkill?.Invoke(this, EventArgs.Empty);
                }
                
            }
            
        }
    }
    public float AttackSpeed { get; set; }
    #endregion

    public float MaxMp;
    public float MaxHealth { get; set; }             //MaxHealth
    public float AttackDamage { get; set; }
    public float SkillPower { get; set; }
    public float PhysicalDefense { get; set; }
    public float MagicDefense { get; set; }
    public int CriticalChance { get; set; }
    public float CriticalDamage { get; set; }
    public float ManaRecoveryRate = 1;
    public float HealthRecoveryRate = 0;
    public float ShieldInit;

    #endregion

    #region Basic Attribute
    [Range(0, 500)]
    public int BasicHealth;
    [Range(0, 200)]
    public int MaxMana = 100;
    [Range(0, 200)]
    public int Mana;
    [Range(0, 300)]
    public int BasicAttackDamage;
    [Range(0, 10)]
    public float BasicAttackSpeed;
    [Range(0, 100)]
    public int BasicSkillPower;
    [Range(0, 100)]
    public int BasicPhysicalDefense;
    [Range(0, 100)]
    public int BasicMagicDefense;
    [Range(0, 100)]
    public int BasicCritcalChance;
    [Range(1, 4)]
    public int BasicAttackRange;
    #endregion

    public bool isStun, isSlience, isBlind;

    public bool isEnemy;
    public HeroState HeroState;
    public HeroPlace HeroPlace, LastHeroPlace;
    public PhotonView photonView;
    public Animator animator;
    public CharacterSoundPlayer mediaPlayer;
    protected bool isAttackCooldown;
    public Vector3 cameraPos;
    
    [SerializeField]
    private Character targetEnemy;
    public Character TargetEnemy
    {
        get { return targetEnemy; }
        set
        {
            if (value != null)
            {
                if (TargetEnemy != null)
                {
                    TargetEnemy.hpChange -= OnEnemyHpChangeListener;
                }
                value.hpChange += OnEnemyHpChangeListener;
            }
            else if (value == null && TargetEnemy != null)
            {
                TargetEnemy.hpChange -= OnEnemyHpChangeListener;
            }
            targetEnemy = value;
            targetChange?.Invoke(this, EventArgs.Empty);
        }
    }

    public Character testHero;
    public GameObject HeroBarObject;
    protected HpBar heroBar;
    public GameObject bullet;
    public Node targetNode;
    public int networkPlaceId;
    public int battlePosId;
    public bool isMirror = false;
    //public List<NegativeEffect> negativeEffects = new List<NegativeEffect>();
    public NegativeEffectManager NegativeEffectManager;
    public float attackRange = 11f;
    public bool isTargetable;
    public HeroPlace DebugHeroPlace;
    
    public event EventHandler hpChange, attack, beAttacked, beControlled, useSkill, roundStart, targetChange, combatStart, combatEnd;

    public delegate void NegativeEffectHandler(float _time);
    //public delegate void CharacterHpHandler();
    //public event CharacterHpHandler CharaterHpChange;

    protected virtual void Start()
    {
        NegativeEffectManager = GetComponent<NegativeEffectManager>();
        mediaPlayer = GetComponent<CharacterSoundPlayer>();

    }
    public void DebugTest()
    {
        Debug.Log("DebugTest" + name);
    }

    /// <summary>
    /// Add negative effect
    /// </summary>
    /// <param name="_time"></param>
    /// <param name="_negativeEffect"></param>
    public void AddNegativeEffect(float _time, NegativeEffectHandler _negativeEffect)
    {
     //   Debug.Log("AddNegativeEffect" + name);
        //TargetEnemy.AddNegativeEffect(0.5f, TargetEnemy.NegativeEffectManager.Slient);
        beControlled?.Invoke(this, EventArgs.Empty);
        _negativeEffect(_time);
    }

    void Update()
    {
        Vector3 targetPostition = new Vector3(HeroBarObject.transform.position.x, cameraPos.y, HeroBarObject.transform.position.x);
        HeroBarObject.transform.LookAt(targetPostition);
        if (HeroState == HeroState.Idle && !isStun)
        {
            IdleState();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            syncAddShield(50f);
            AddNegativeEffect(3f, NegativeEffectManager.Knock);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            mediaPlayer.playAttackSound();
        }
    }

    /// <summary>
    /// New method to handle idle state
    /// </summary>
    public void IdleState()
    {
        if (TargetEnemy == null)
        {
            //Set the target enemy
            TargetEnemy = NetworkManager.Instance.getCloestEnemyTarget(!isEnemy, transform);

            if (TargetEnemy != null)
            {
                //Add the event listener when the enemy hp is changed.      
                //TargetEnemy.hpChange += OnEnemyHpChangeListener;
               // CharacterFight();
                photonView.RPC("RPC_SyncTargetEnemy", PhotonTargets.Others, TargetEnemy.photonView.viewID);
            }
        }
        else
        {
            FollowEnemy();
        }
    }
    
    /// <summary>
    /// Enemy's HP changed listener
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnEnemyHpChangeListener(object sender, EventArgs e)
    {
        float targetHP = ((Character)sender).Health;

        //Target Die
        if (targetHP <= 0)
        {
          //  Debug.Log("Enemy " + TargetEnemy.name + " Dead");
            // TargetEnemy = null;
            targetDie();
            //TargetEnemy = null;
            if (!isMirror)
                HeroState = HeroState.Idle;

        }
    }

    /// <summary>
    /// New method to handle the start of game round
    /// </summary>
    public void CharacterFight()
    {
        try
        {
            StartCoroutine(Attack());
        }
        catch (Exception e) { }
    }
    
    public virtual void targetDie()
    {
        TargetEnemy = null;
    }
    public void TakePercentageDamage(float _percentage)
    {
        float value = MaxHealth / 100f * _percentage;
        photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, value, (byte)DamageType.TrueDamage);
    }
    public virtual void attackTarget()
    {
        if (!isMirror)
        {
            if (TargetEnemy != null && TargetEnemy.Health > 0)
            {
                int chance = UnityEngine.Random.Range(1,101);
                if (CriticalChance >= chance)
                {
                    float damage = AttackDamage * (CriticalDamage+1);
                    TargetEnemy.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage, (byte)DamageType.CriticalDamage);
                }
                else
                    TargetEnemy.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, AttackDamage, (byte)DamageType.Physical);
                photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, 10f*ManaRecoveryRate);
                
            }
        }
        mediaPlayer.playAttackSound();
    }
    public void shootTarget()
    {
        if (!isMirror)
        {
            if (TargetEnemy != null)
            {
                photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, 10f* ManaRecoveryRate);
                photonView.RPC("RPC_Shoot", PhotonTargets.All,TargetEnemy.photonView.viewID);
            }
        }

    }
    [PunRPC]
    public void RPC_SyncTargetEnemy(int id)
    {
        syncTargetEnemy(id);
    }

    void syncTargetEnemy(int id)
    {
        TargetEnemy = PhotonView.Find(id).GetComponent<Character>();
        CharacterFight();
    }

    [PunRPC] 
    public void RPC_Shoot(int id)
    {
        processShoot(id);
    }

    void processShoot(int id) {
        Bullet b = Instantiate(bullet, transform.position + Vector3.up, transform.rotation).GetComponent<Bullet>();
        Character target = PhotonView.Find(id).GetComponent<Character>();
        int chance = UnityEngine.Random.Range(1, 101);
        mediaPlayer.playAttackSound();
        if (CriticalChance >= chance)
        {
            float damage = AttackDamage * 1.5f;
            b.setBullet(target, damage, !isMirror, DamageType.CriticalDamage);
        }
        else
        {
            b.setBullet(target, AttackDamage, !isMirror);
        }
        //Debug.Log("target "+target.name +" photon prcess shot");
    }

    [PunRPC]
    public void RPC_TargetTakeDamage(float damage,byte damageType)
    {
            DamageType type = (DamageType)damageType;
            syncAdjustHp(-damage,type);    
    }
    
    [PunRPC]
    public void RPC_AddMaxHP(float value)
    {
        AddMaxHp(value);
    }
    [PunRPC]
    public void RPC_Heal(float index,byte damageType)
    {
        DamageType type = (DamageType)damageType;
        syncAdjustHp(index, type);
    }

    public void AddMaxHp(float _value)
    {
        MaxHealth += _value;
        Health += _value;
    }
    public virtual void syncAdjustHp(float damage, DamageType type)
    {
        if (type == DamageType.Physical) {
            if (PhysicalDefense > 0)
            {
                damage *= PhysicalDefense / (100 + PhysicalDefense);
            }
            else {
                damage *= 2 - 100 / (100 - PhysicalDefense);
            }
        }
           
        if (type == DamageType.Magic) {
            if (MagicDefense > 0)
            {
                damage *= MagicDefense / (100 + MagicDefense);
            }
            else {
                damage *= 2 - 100 / (100 - MagicDefense);
            }
        }
        damage = (float)Math.Floor(damage);
        if (damage < 0)
        {
            if (Sheild > 0)
                damage = sheildDefense(damage);
        }
      
        if (Health + damage > 0)
            Health += damage;
        else
            Health = 0;
        if (type != DamageType.No)
            NetworkManager.Instance.showDamageText(damage.ToString(), type, transform.position, battlePosId);
        if (Health > MaxHealth)
            Health = MaxHealth;      
        if (damage < 0)
        {
            heroBar.setHpBarWithDamage(Health / MaxHealth);
        }
        else
        {
            heroBar.setHpBar(Health / MaxHealth);
        }
    }
    
    [PunRPC]
    public void RPC_IncreaseMp(float index)
    {
        syncAdjustMp(index);
    }
    [PunRPC]
    public void RPC_ReduceMp(float index)
    {
        syncAdjustMp(-index);
    }
    public virtual void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
    }
    public void syncAdjustMp(float mp)
    {
        Mp += mp;
        if (Mp > MaxMp)
            Mp = MaxMp;
        if (Mp <= 0)
        {
            Mp = 0;
        }
        heroBar.setMpBar(Mp / MaxMp);
    }

    [PunRPC]
    public void RPC_addShield(float index)
    {
        syncAddShield(index);
    }
    public void syncAddShield(float value)
    {
        Sheild += value;
        heroBar.addShieldBar(value / MaxHealth);
      //  Debug.Log(value / MaxHealth);
    }
    public float sheildDefense(float damage)
    {

        float basicSheild = Sheild;
        Sheild += damage;

        if (Sheild > 0)
        {
            float precentage = damage * -1 / basicSheild * heroBar.getShieldbarFillAmount();      
            heroBar.reduceShieldBar(precentage);
            return 0;
        }
        else
        {
            float remainDamage = Sheild;
            Sheild = 0;
            heroBar.setShieldBar(0);      
            return remainDamage;
        }
    }
    public virtual void die() { }

    protected void FollowEnemy()
    {
        HeroState = HeroState.Walking;

        checkWithInAttackRange();
        if (HeroState != HeroState.Fight)
            PathFindingManager.Instance.requestPath(HeroPlace, TargetEnemy.HeroPlace, OnPathFind);
    }
    public void checkWithInAttackRange() {

        // float dis = Vector3.Distance(HeroPlace.transform.position, TargetEnemy.HeroPlace.transform.position);
        float dis = NetworkManager.Instance.getNodeDistance(HeroPlace,targetEnemy.HeroPlace);
    //    Debug.Log("dis "+dis +" target"+targetEnemy.name +" Range "+attackRange);
        if (dis <= attackRange) {
         //   Debug.Log("Within attack Range attack"+targetEnemy.HeroPlace);
            HeroState = HeroState.Fight;
            CharacterFight();
           // CharacterFight();
            photonView.RPC("RPC_StopWalk", PhotonTargets.All);
        }
    }

    public void OnPathFind(List<Node> path, bool isFindPath)
    {
        if (targetEnemy == null)
        {
            HeroState = HeroState.Idle;
        }
        else
        {
            if (isFindPath)
            {
                if (path != null)
                {
                    StartCoroutine(FollowStep(path[0]));
                }
            }
            else
            {
                StartCoroutine(cannotPathFind());
            }
        }

    }
    IEnumerator FollowStep(Node step)
    {
        GetComponent<PhotonView>().RPC("RPC_FollowStep", PhotonTargets.Others, NetworkManager.Instance.battlePosId, step.heroPlace.PlaceId, step.heroPlace.gridY);
        MoveToThePlace(step.heroPlace);
        transform.LookAt(step.heroPlace.transform);
        animator.SetBool("Walk", true);
        targetNode = step;
        while (HeroState == HeroState.Walking && !isStun)
        {

            if (transform.position != step.heroPlace.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, step.heroPlace.transform.position, 3 * Time.deltaTime);
            }
            else
            {            
                photonView.RPC("RPC_StopWalk", PhotonTargets.All);

                StartCoroutine(FindPathAgain());                         
                break;
            }
            yield return null;
        }
        yield return null;
    }
    public IEnumerator FindPathAgain()
    {
        TargetEnemy = NetworkManager.Instance.getCloestEnemyTarget(!isEnemy, transform);
       // Debug.Log(name+" find Target "+TargetEnemy);
        if (TargetEnemy != null)
        {            
            photonView.RPC("RPC_SyncTargetEnemy", PhotonTargets.Others, TargetEnemy.photonView.viewID);
        }
        yield return new WaitForSeconds(0.3f);
        FollowEnemy();
    }
    [PunRPC]
    public void RPC_FollowStep(int posId, int placeId, int YPos)
    {
        SyncFollowStep(posId, placeId, YPos);
    }
    public void SyncFollowStep(int posId, int placeId, int YPos)
    {
        bool isEnemyPlace;
        if (YPos <= 3)
            isEnemyPlace = false;  // isEnemyPlace = true;
        else
            isEnemyPlace = true;// isEnemyPlace = false;
        HeroPlace heroPlace = NetworkManager.Instance.GetBattleHeroPlace(posId, placeId, isEnemyPlace);
        try
        {
            StartCoroutine(RPC_FollowHeroPlace(heroPlace));
        }
        catch (Exception e) { }
    }
    public IEnumerator RPC_FollowHeroPlace(HeroPlace step)
    {
        animator.SetBool("Walk", true);
        DebugHeroPlace = step;
        transform.LookAt(step.transform);
        while (transform.position != step.transform.position && isMirror)
        {          
            transform.position = Vector3.MoveTowards(transform.position, step.transform.position, 3 * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator cannotPathFind() {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FindPathAgain());
    }
    public void MoveToThePlace(HeroPlace newHeroPlace)
    {
        HeroPlace.leavePlace();
        newHeroPlace.setHeroOnPlace(this);
        LastHeroPlace = HeroPlace;
        HeroPlace = newHeroPlace;
    }
    [PunRPC]
    public void RPC_StopWalk()
    {
        animator.SetBool("Walk", false);
    }
    public void ReadyForBattle(bool isEnemy, int posId)
    {
        isMirror = false;
        this.isEnemy = isEnemy;
        photonView.RPC("RPC_ShowHpBar", PhotonTargets.All, posId);
        photonView.RPC("RPC_Mirror", PhotonTargets.Others);
        photonView.RPC("RPC_SyncInfo", PhotonTargets.All,NetworkManager.Instance.battlePosId);

        combatStart?.Invoke(this, EventArgs.Empty);
        StartCoroutine(RecoveryHP());
        HeroState = HeroState.Idle;
        //Debug.Log("raedy ");
        //    Debug.Log("name " + name + " ready ");
    }
    [PunRPC]
    public void RPC_SyncInfo(int id)
    {
        battlePosId = id;
        tag = "BattleCharacter";
    }
    [PunRPC]
    public void setTransformView() {
        GetComponent<PhotonTransformView>().enabled = true;
    }

    [PunRPC]
    public void RPC_Mirror() {
        isMirror = true;
       
    }
    [PunRPC]
    public void RPC_ShowHpBar(int posid)
    {
        if (NetworkManager.Instance.opponent.heroes.Contains(this))
            heroBar.setHpBarColor(Color.red);
        HeroBarObject.SetActive(true);
        if (NetworkManager.Instance.isHomeTeam)
        {
            cameraPos = NetworkManager.Instance.getCameraObject(posid).transform.position;
        }
        else {
            cameraPos = NetworkManager.Instance.getCameraObject(posid).transform.position*-1;
        }
       
    }
    [PunRPC]
    public void RPC_AttackAnimation()
    {
        if (TargetEnemy != null)
            transform.LookAt(TargetEnemy.transform);
        animator.SetTrigger("Attack");
    }
    [PunRPC]
    public void RPC_MoveToThePlayerHeroPlace(int posId, int placeId,bool isEnemy)
    {
        HeroPlace heroPlace = NetworkManager.Instance.GetBattleHeroPlace(posId, placeId,isEnemy);
        SetHeroPlace(heroPlace);
        // Debug.Log( this.name + " become enemy? " + isEnemy+" Pos "+heroPlace.gridX+" "+heroPlace.gridY);
    }
    public void SetHeroPlace(HeroPlace heroPlace)
    {
        heroPlace.setHeroOnPlace(this);
        transform.localPosition = Vector3.zero;
        LastHeroPlace = heroPlace;
        HeroPlace = heroPlace;

    }
    public void stopCharacter()
    {
        HeroState = HeroState.Nothing;
        TargetEnemy = null;
        animator.SetBool("Walk", false);
    }
    [PunRPC]
    public void RPC_HitPlayerCharacter(int id) {
        hitopponentCharacter(id);
    }
    void hitopponentCharacter(int id) {
        UnityEngine.Object pPrefab = Resources.Load("Effect/heroHitPlayerEffect");
        GameObject b = Instantiate(pPrefab, transform.position, transform.rotation) as GameObject;
      //  GameObject opponentPlayer = null;
        GameObject character = PhotonView.Find(id).gameObject;
        //if (NetworkManager.Instance.playerId == playerId)
     /*   if (isHomeTeam)
        {
            
            opponentPlayer = NetworkManager.Instance.PlayerArenas[battlePos]. GetComponent<PlayerArena>()
                .opponentCharacterSlot.GetChild(0).gameObject;

        }
        else
        {
            opponentPlayer = NetworkManager.Instance.PlayerArenas[battlePos].GetComponent<PlayerArena>()
                .playerCharacterSlot.GetChild(0).gameObject;

        }*/
        b.GetComponent<Bullet>().setBullet(character, 8f);
    }
    public IEnumerator Attack()
    {
        while (true)
        {
            if (HeroState == HeroState.Die)
            {
                yield break;
            }
            else if (HeroState == HeroState.Fight && !isBlind)
            {
                photonView.RPC("RPC_AttackAnimation", PhotonTargets.All);
                if (TargetEnemy != null)
                {
                    transform.LookAt(TargetEnemy.transform);
                    TargetEnemy.beAttacked?.Invoke(this, EventArgs.Empty);
                    if (TargetEnemy.HeroState == HeroState.Die) TargetEnemy = null;
                }
                attack?.Invoke(this, EventArgs.Empty);
               // if(isEnemy)
              //  Debug.Log(name+" attack ");
                yield return new WaitForSeconds(1 / (AttackSpeed * 2.5f));

            }
            else
            {
                yield return new WaitForSeconds(0);
            }
        }
        
    }

    public IEnumerator RecoveryHP()
    {
       
            if (Health < MaxHealth)
            {
                float recoverValue = MaxHealth * HealthRecoveryRate;
                photonView.RPC("RPC_Heal", PhotonTargets.All, recoverValue,(byte)DamageType.No);

            }
            yield return new WaitForSeconds(1);
        StartCoroutine(RecoveryHP());
    }
    //public IEnumerator BasicAttackCoolDown()
    //{
    //    isAttackCooldown = true;
    //    yield return new WaitForSeconds(1 / (AttackSpeed * 2.5f));
    //    isAttackCooldown = false;
    //}
    //public IEnumerator controlableSkillDuration(float time,NegativeEffect type) {
    //    yield return new WaitForSeconds(time);
    //    negativeEffects.Remove(type);
    //    CheckNegativeEffect();
    //}

    //public void CheckNegativeEffect() {
    //    bool canMove=true;
    //    if (HeroState != HeroState.Die)
    //    {
    //        foreach (NegativeEffect negative in negativeEffects)
    //        {
    //            if (!negative.canAction)
    //                canMove = false;


    //        }
    //        if (canMove)
    //        {
    //            if (isMirror)
    //                HeroState = HeroState.Nothing;
    //            else
    //                HeroState = HeroState.Idle;
    //        }
    //        else
    //            HeroState = HeroState.Control;
    //    }
    //}
}
