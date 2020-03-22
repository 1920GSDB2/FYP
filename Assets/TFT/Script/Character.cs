using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TFT;
using UnityEngine.UI;


public class Character : MonoBehaviour
{
    public float Health { get; set; }
    protected float MaxHealth { get;  set; }
    public float AttackDamage { get; set; }
    public float AttackSpeed { get; set; }
    public float SkillPower { get; set; }
    public float PhysicalDefense { get; set; }
    public float MagicDefense { get; set; }
    public bool isEnemy;
    public HeroState HeroState;
    public HeroPlace HeroPlace, LastHeroPlace;
    public PhotonView photonView;  
    public Animator animator;    
    protected bool isAttackCooldown;
    protected Vector3 cameraPos;
    public Character targetEnemy;
    public Character testHero;
    public GameObject HeroBar;
    protected Image hpBar;
    protected Image mpBar;
    
    protected float attackRange = 1.75f;

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
    public void attackTarget()
    {

        //Debug.Log(targetEnemy.name + " Take Damage");
        if (targetEnemy != null)
            targetEnemy.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, AttackDamage);
        // Debug.Log(targetEnemy.name+" have hp: "+ targetEnemy.Health);
        if (targetEnemy.Health <= 0)
        {
            targetEnemy = null;
            HeroState = HeroState.Idle;
        }
    }
 
    [PunRPC]
    public void RPC_TargetTakeDamage(float damage)
    {
        syncAdjustHp(-damage);
    }
    [PunRPC]
    public void RPC_Heal(float index)
    {
        syncAdjustHp(index);
    }
    public void syncAdjustHp(float damage)
    {
        Health += damage;
        if (Health > MaxHealth)
            Health = MaxHealth;
        if (Health <= 0)
        {
            die();
        }
        hpBar.fillAmount = Health / MaxHealth;
    }
    public virtual void die() { }

    protected void followEnemy()
    {
        float dis = Vector3.Distance(transform.position, targetEnemy.transform.position);

        // Debug.Log("Distance " + dis + " AttackRange " + attackRange);
        if (dis > attackRange)
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

        }
    }
    public void OnPathFind(List<Node> path, bool isFindPath)
    {
        if (isFindPath)
        {
            if (path != null)
                StartCoroutine(FollowStep(path[0]));
        }
    }
    IEnumerator FollowStep(Node step)
    {
        //Debug.Log("FollowStep");
        GetComponent<PhotonView>().RPC("RPC_FollowStep", PhotonTargets.Others, step.heroPlace.PlaceId, step.heroPlace.gridY);
        MoveToThePlace(step.heroPlace);
        transform.LookAt(step.heroPlace.transform);
        animator.SetBool("Walk", true);
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
                StartCoroutine(FindPathAgain());
                //PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, OnPathFind);                          
                break;
            }
            yield return null;
        }

    }
    IEnumerator FindPathAgain()
    {
        //yield return new WaitForSeconds(2);
        yield return new WaitForSeconds(0.3f);
        followEnemy();
    }
    [PunRPC]
    public void RPC_FollowStep(int placeId, int YPos)
    {
        SyncFollowStep(placeId, YPos);
    }
    public void SyncFollowStep(int placeId, int YPos)
    {
        bool isEnemyPlace;
        if (YPos <= 3)
            isEnemyPlace = true;
        else
            isEnemyPlace = false;
        HeroPlace heroPlace = NetworkManager.Instance.GetOpponentHeroPlace(placeId, isEnemyPlace);
        StartCoroutine(RPC_FollowHeroPlace(heroPlace));
    }
    public IEnumerator RPC_FollowHeroPlace(HeroPlace step)
    {
        transform.LookAt(step.transform);
        animator.SetBool("Walk", true);
        while (transform.position != step.transform.position)
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
    [PunRPC]
    public void RPC_ShowHpBar(int posid)
    {
        HeroBar.SetActive(true);
        cameraPos = NetworkManager.Instance.getCamera(posid).transform.position;
    }
    [PunRPC]
    public void RPC_AttackAnimation()
    {
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
    public IEnumerator basicAttackCoolDown()
    {
        isAttackCooldown = true;
        yield return new WaitForSeconds(1 / (AttackSpeed * 2.5f));
        isAttackCooldown = false;
    }
}
