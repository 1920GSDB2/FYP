using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;
using UnityEngine.UI;
using System.IO;
using System;

public class Character : MonoBehaviour
{
    public float Health { get; set; }
    protected float MaxHealth { get; set; }
    public float MaxMp;
    public float Mp { get; private set; }
    public float AttackDamage { get; set; }
    public float AttackSpeed { get; set; }
    public float SkillPower { get; set; }
    public float PhysicalDefense { get; set; }
    public float MagicDefense { get; set; }
    protected float Sheild;
    public bool isEnemy;
    public HeroState HeroState;
    public HeroPlace HeroPlace, LastHeroPlace;
    public PhotonView photonView;
    public Animator animator;
    protected bool isAttackCooldown;
    public Vector3 cameraPos;
    public Character targetEnemy;
    public Character testHero;
    public GameObject HeroBarObject;
    protected HpBar heroBar;
    //protected Image hpBar;
   // protected Image mpBar;
    public GameObject bullet;
    public Node targetNode;
    public int networkPlaceId;
    protected bool isMirror = false;
    protected float MpRecoverRate=1;
    public List<NegativeEffect> negativeEffects=new List<NegativeEffect>();
    public float attackRange = 1.7f;
   

    // Start is called before the first frame update

    //  hpBar = HeroBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
    // mpBar = HeroBar.transform.GetChild(0).GetChild(1).GetComponent<Image>();


    // Update is called once per frame
    /*void Update()
    {
        Vector3 targetPostition = new Vector3(HeroBar.transform.position.x, cameraPos.y, HeroBar.transform.position.x);
        HeroBar.transform.LookAt(targetPostition);
        if (HeroState == HeroState.Idle)
        {
            if (targetEnemy == null)
                targetEnemy = NetworkManager.Instance.getCloestEnemyTarget(isEnemy, transform);
            else
            {
                HeroState = HeroState.Walking;
                followEnemy();
            }

        }
        if (HeroState == HeroState.Fight)
        {
            if (!isAttackCooldown)
                photonView.RPC("RPC_AttackAnimation", PhotonTargets.All);
            animator.SetTrigger("Attack");

        }
    }*/
    public virtual void attackTarget()
    {

        //Debug.Log(targetEnemy.name + " Take Damage");
        if (!isMirror)
        {
            if (targetEnemy != null && targetEnemy.Health > 0)
            {

                photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, 10f*MpRecoverRate);
                //GetComponent<PhotonView>().RPC("RPC_IncreaseMp", PhotonTargets.All,10);
                targetEnemy.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, AttackDamage,(byte)DamageType.Physical);
            }
        }
        //  Debug.Log(targetEnemy.name+" have hp: "+ targetEnemy.Health);

    }
    public void shootTarget()
    {

        //Debug.Log(targetEnemy.name + " Take Damage");
        if (!isMirror)
        {
            if (targetEnemy != null)
            {
                photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, 10f* MpRecoverRate);
                // Bullet b = PhotonNetwork.Instantiate(Path.Combine("Bullet", bullet.name), transform.position+Vector3.up*2, transform.rotation, 0).GetComponent<Bullet>();              
                // b.setBullet(targetEnemy, AttackDamage, !isMirror);
                //     photonView.RPC("f", PhotonTargets.All,NetworkManager.Instance.battlePosId,targetEnemy.HeroPlace.PlaceId,targetEnemy.HeroPlace.gridY,
                //       NetworkManager.Instance.playerId,NetworkManager.Instance.opponent.opponentId);
                photonView.RPC("RPC_Shoot", PhotonTargets.All,targetEnemy.photonView.viewID);
                //targetEnemy.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, AttackDamage);
            }
        }
        // Debug.Log(targetEnemy.name+" have hp: "+ targetEnemy.Health);

    }
    [PunRPC] 
    public void RPC_Shoot(int id)
    {
        //processShoot(battlePosId,placeId,YPos,hostId,guestId);
        processShoot(id);
    }
    // void processShoot(int battlePosId, int placeId, int YPos, int hostId, int guestId) {
    void processShoot(int id) {

        
        // Debug.Log("skill power " + (SkillPower * 0.5f * AttackDamage));      
        Bullet b = Instantiate(bullet, transform.position + Vector3.up, transform.rotation).GetComponent<Bullet>();
        Character target = PhotonView.Find(id).GetComponent<Character>();
        b.setBullet(target, AttackDamage, isMirror);
        Debug.Log("target "+target.name +" photon prcess shot");
        /*
        if (NetworkManager.Instance.playerId == hostId || NetworkManager.Instance.playerId == guestId)
        {
            if (targetEnemy != null)
            {
                Bullet b = Instantiate(bullet, transform.position + Vector3.up, transform.rotation).GetComponent<Bullet>();
                b.setBullet(targetEnemy.gameObject, AttackDamage, !isMirror);
            }
        }
        else {
            /* bool isEnemySide;
             if (YPos >= 4)
                 isEnemySide = true;
             else
                 isEnemySide = false;
            Bullet b = Instantiate(bullet, transform.position + Vector3.up, transform.rotation).GetComponent<Bullet>();
            Debug.Log(" watch player shoot");
            //  HeroPlace heroplace=NetworkManager.Instance.GetBattleHeroPlace(battlePosId, placeId, isEnemySide);
            Vector3 targetPos = new Vector3(x, b.transform.position.y, z);
            b.setBullet(targetPos);
            //  b.setBullet(heroplace.transform.position);

        } 
        */
    }
    [PunRPC]
    public void RPC_TargetTakeDamage(float damage,byte damageType)
    {
        if (Health > 0)
        {
            // Debug.Log("call take damage Health " + Health);
            DamageType type = (DamageType)damageType;
            syncAdjustHp(-damage,type);
        }
    }
    [PunRPC]
    public void RPC_TargetTakeDamage(float damage,byte controlType,float duration, byte damageType)
    {
     

        if (Health > 0)
        {
            ControlSkillType control = (ControlSkillType)controlType;
            DamageType _damageType = (DamageType)damageType;
            syncAdjustHp(-damage, _damageType);
            addNegativeEffect(control, duration);
        }
    }
    public void addNegativeEffect(ControlSkillType type,float duration) {
        if (HeroState != HeroState.Die)
        {
            animator.Play("Idle");
            HeroState = HeroState.Control;
            NegativeEffect newEffect = new NegativeEffect(type, false);
            negativeEffects.Add(newEffect);
            StartCoroutine(controlableSkillDuration(duration, newEffect));
        }
    }
    [PunRPC]
    public void RPC_Heal(float index,byte damageType)
    {
        DamageType tpye = (DamageType)damageType;
        syncAdjustHp(index, tpye);
    //    Debug.Log("Heal " + index);
    }
    public virtual void syncAdjustHp(float damage,DamageType type)
    {
        
        if (damage < 0) {
            if (Sheild > 0)
                damage = sheildDefense(damage);
        }
      //  Debug.Log("Shield "+Sheild+" take DAmage "+damage);
            Health += damage;
        if(type!=DamageType.No)
        NetworkManager.Instance.showDamageText(damage.ToString(),type,transform.position);
            if (Health > MaxHealth)
                Health = MaxHealth;
            if (Health < 0)
               Health = 0;
            if (Health <= 0)
            {
            //    Debug.Log("before call Die " + name + " state " + HeroState + "Health "+Health+"damage "+damage);
                if (HeroState != HeroState.Die)
                {
                  //  Debug.Log("call Die " + name + " state " + HeroState + "Health"+ Health);
                    die();
                }
            }
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
        Debug.Log(value / MaxHealth);
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
        checkWithInAttackRange();
        if (HeroState != HeroState.Fight)
            PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, OnPathFind);
        //  float dis = Vector3.Distance(transform.position, targetEnemy.transform.position);

        // Debug.Log("Distance " + dis + " AttackRange " + attackRange);
        /*  if (dis > attackRange)
          {
              // Debug.Log("Distance " + dis + " AttackRange " + attackRange);
              //  Debug.Log("followEnemy");
              PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, OnPathFind);
          }
          else
          {
              if (HeroState != HeroState.Fight)
              {
                  HeroState = HeroState.Fight;
                  // animator.SetBool("Walk", false);
                  photonView.RPC("RPC_StopWalk", PhotonTargets.All);

              }

          }*/
    }
    public void checkWithInAttackRange() {

        float dis = Vector3.Distance(HeroPlace.transform.position, targetEnemy.HeroPlace.transform.position);
        if (dis <= attackRange) {
            HeroState = HeroState.Fight;
            photonView.RPC("RPC_StopWalk", PhotonTargets.All);
            //      return true;
        }
        // return false;
    }

    public void OnPathFind(List<Node> path, bool isFindPath)
    {
        if (isFindPath)
        {
            if (path != null)
            {
                //Debug.Log("Find path? " + isFindPath + " Hero " + name);
                StartCoroutine(FollowStep(path[0]));
            }
        }

    }
    IEnumerator FollowStep(Node step)
    {
        //Debug.Log("FollowStep");
        // withInAttackRange();

        GetComponent<PhotonView>().RPC("RPC_FollowStep", PhotonTargets.Others, NetworkManager.Instance.battlePosId, step.heroPlace.PlaceId, step.heroPlace.gridY);
        MoveToThePlace(step.heroPlace);
        transform.LookAt(step.heroPlace.transform);
        animator.SetBool("Walk", true);
        targetNode = step;
        // Vector3 destination = new Vector3(step.heroPlace.transform.position.x, transform.position.y, step.heroPlace.transform.position.z);
        while (HeroState == HeroState.Walking)
        {

            if (transform.position != step.heroPlace.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, step.heroPlace.transform.position, 3 * Time.deltaTime);
                //   Debug.Log("Move X "+step.heroPlace.gridX+" Y "+ step.heroPlace.gridY);
            }
            else
            {
                //  Debug.Log("finish");
                // isLoop = false;               
                photonView.RPC("RPC_StopWalk", PhotonTargets.All);

                StartCoroutine(FindPathAgain());
                //PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, OnPathFind);                          
                break;
            }
            yield return null;
        }
        yield return null;
    }
    public IEnumerator FindPathAgain()
    {
        //yield return new WaitForSeconds(2);
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
        while (transform.position != step.transform.position && isMirror)
        {
            transform.position = Vector3.MoveTowards(transform.position, step.transform.position, 3 * Time.deltaTime);
            yield return null;
        }
    }
    public void MoveToThePlace(HeroPlace newHeroPlace)
    {
        HeroPlace.leavePlace();
        newHeroPlace.setHeroOnPlace(this);
        //SetHeroPlace(newHeroPlace);
        LastHeroPlace = HeroPlace;
        HeroPlace = newHeroPlace;
    }
    [PunRPC]
    public void RPC_StopWalk()
    {
        animator.SetBool("Walk", false);
    }
    public void readyForBattle(bool isEnemy, int posId)
    {
           
        //   this.isEnemy = isEnemy;
        isMirror = false;
        //Debug.Log("State " + HeroState + " Enemy " + isEnemy);
        // photonView.RPC("setTransformView", PhotonTargets.All);
        photonView.RPC("RPC_ShowHpBar", PhotonTargets.All, posId);
        photonView.RPC("RPC_Mirror", PhotonTargets.Others);
        tag = "BattleCharacter";
        HeroState = HeroState.Idle;       
        Debug.Log("name " + name + " ready ");
    }
    [PunRPC]
    public void setTransformView() {
        //isMirror = !isMirror;
        GetComponent<PhotonTransformView>().enabled = true;
    }

    [PunRPC]
    public void RPC_Mirror() {
        isMirror = true;
    }
    [PunRPC]
    public void RPC_ShowHpBar(int posid)
    {
        if (NetworkManager.Instance.opponent.hero.Contains(this))
            heroBar.setHpBarColor(Color.red);
        HeroBarObject.SetActive(true);
       

        //GameObject camera = NetworkManager.Instance.getCamera(posid);
        if (NetworkManager.Instance.isHomeTeam)
        {
            cameraPos = NetworkManager.Instance.getCamera(posid).transform.position;
        }
        else {
            cameraPos = NetworkManager.Instance.getCamera(posid).transform.position*-1;
        }
       
       // Debug.Log(name + " " + camera.name+" home "+NetworkManager.Instance.isHomeTeam);
    }
    [PunRPC]
    public void RPC_AttackAnimation()
    {
        if (targetEnemy != null)
            transform.LookAt(targetEnemy.transform);
        animator.SetTrigger("Attack");
        //transform.LookAt(targetEnemy.transform);
    }
    [PunRPC]
    public void RPC_MoveToThePlayerHeroPlace(int posId, int placeId)
    {
        HeroPlace heroPlace = NetworkManager.Instance.GetMyGameBoardEnemyHeroPlace(posId, placeId);
        SetHeroPlace(heroPlace);
        // Debug.Log( this.name + " become enemy? " + isEnemy+" Pos "+heroPlace.gridX+" "+heroPlace.gridY);
    }
    public void SetHeroPlace(HeroPlace heroPlace)
    {
        heroPlace.setHeroOnPlace(this);
        //   HeroPlace.leavePlace();
        // transform.parent = heroPlace.gameObject.transform;
        transform.localPosition = Vector3.zero;
        LastHeroPlace = heroPlace;
        HeroPlace = heroPlace;

    }
   
    [PunRPC]
    public void RPC_HitPlayerCharacter(bool isHomeTeam) {
        hitopponentCharacter(isHomeTeam);
    }
    void hitopponentCharacter(bool isHomeTeam) {
        UnityEngine.Object pPrefab = Resources.Load("Effect/heroHitPlayerEffect");
        GameObject b = Instantiate(pPrefab, transform.position, transform.rotation) as GameObject;
        GameObject opponentPlayer = null;

        //if (NetworkManager.Instance.playerId == playerId)
        if (isHomeTeam)
        {
            opponentPlayer = NetworkManager.Instance.PlayerArenas[NetworkManager.Instance.battlePosId].
                                        GetComponent<PlayerArena>().opponentCharacterSlot.GetChild(0).gameObject;

        }
        else
        {
            opponentPlayer = NetworkManager.Instance.PlayerArenas[NetworkManager.Instance.battlePosId].
                                    GetComponent<PlayerArena>().playerCharacterSlot.GetChild(0).gameObject;

        }
        b.GetComponent<Bullet>().setBullet(opponentPlayer, 2f);
    }
   
    public IEnumerator basicAttackCoolDown()
    {
        isAttackCooldown = true;
        yield return new WaitForSeconds(1 / (AttackSpeed * 2.5f));
        isAttackCooldown = false;
    }
    public IEnumerator controlableSkillDuration(float time,NegativeEffect type) {
        yield return new WaitForSeconds(time);
        negativeEffects.Remove(type);
        CheckNegativeEffect();
    }
    public void CheckNegativeEffect() {
        bool canMove=true;
        if (HeroState != HeroState.Die)
        {
            foreach (NegativeEffect negative in negativeEffects)
            {
                if (!negative.canAction)
                    canMove = false;


            }
            if (canMove)
            {
                if (isMirror)
                    HeroState = HeroState.Nothing;
                else
                    HeroState = HeroState.Idle;
            }
            else
                HeroState = HeroState.Control;
        }
    }
}
