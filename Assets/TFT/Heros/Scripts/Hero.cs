using System;
using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine.UI;
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
    PhotonView photonView;
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

    float attackRange = 1.75f;
    public float Health { get; set; }
    public float AttackDamage { get; set; }
    public float AttackSpeed { get; set; }
    public float SkillPower { get; set; }
    public float PhysicalDefense { get; set; }
    public float MagicDefense { get; set; }

    bool isAttackCooldown;
    string lastTransform;
    public Animator animator;
    public GameObject HeroBar;
    public Image hpBar;
    public Image mpBar;
    //TFTGameManager gameManager;
    public Hero targetEnemy;
    public Hero testHero;
    //  List<Node> path;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        hpBar = HeroBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        mpBar = HeroBar.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        //  HeroBar.transform.LookAt(NetworkManager.Instance.getCamera().transform);

        lastTransform = transform.parent.name;
        //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
        MouseSelect = GameManager.Instance.GetComponent<MouseSelect>();

        HeroPlace = transform.parent.GetComponent<HeroPlace>();
        
        //HeroState = HeroState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
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
                photonView.RPC("RPC_Attack", PhotonTargets.All);

        }
        /* if (HeroState==HeroState.Idle)
         {
             if (targetEnemy != null)
             {
                 //  followEnemy();
                 HeroState = HeroState.Walking;
                 // PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, OnPathFind);
                 followEnemy();
                 //  targetEnemy = null;

             }
         }*/




        if (Input.GetKeyDown(KeyCode.I))
        {
            //  targetEnemy = testHero;

            animator.SetTrigger("Attack");
            //  photonView.RPC("RPC_Animation", PhotonTargets.All);
        }

        /*   if (Input.GetKeyDown(KeyCode.L))
           {
               targetEnemy = testHero;
               if (targetEnemy != null)
               {
                   PathFindingManager.Instance.requestPath(HeroPlace, targetEnemy.HeroPlace, OnPathFind);

                   // targetEnemy = null;
               }
           }*/

    }
    void followEnemy() {
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
                photonView.RPC("RPC_StopWalk", PhotonTargets.All);
                
            }
           
        }
    }
    [PunRPC]
    public void RPC_Attack()
    {        
        animator.SetTrigger("Attack");
        //transform.LookAt(targetEnemy.transform);
    }
    //called by OathfindingManager when request a path
    #region pathFinding Method
    public void OnPathFind(List<Node> path, bool isFindPath)
    {
        if (isFindPath)
        {
            if (path != null)
                StartCoroutine(FollowStep(path[0]));
        }
    }
    //called by OathfindingManager when request next step
    public void OnStepFind(Node step, bool isFindStep)
    {
        Debug.Log(isFindStep);
        if (isFindStep)
        {
            // step.heroPlace.settColor(Color.blue);
            StartCoroutine(FollowStep(step));

        }
    }
    #endregion
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
    /* void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {
         if (stream.isWriting)
         {
             stream.SendNext(transform.position);
             stream.SendNext(transform.rotation);
         }
         else
         {
             transform.position = (Vector3)stream.ReceiveNext();
             transform.rotation = (Quaternion)stream.ReceiveNext();
         }
     }*/
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
    public void MoveToThePlace(HeroPlace newHeroPlace)
    {
        HeroPlace.leavePlace();
        newHeroPlace.setHeroOnPlace(this);
        //SetHeroPlace(newHeroPlace);
        LastHeroPlace = HeroPlace;
        HeroPlace = newHeroPlace;
    }
    #region RPC move hero
    [PunRPC]
    public void RPC_AddToGameBoard(int posId, int placeId)
    {
        /*   HeroPlace.leavePlace();
           place.setHeroOnPlace(this);

           LastHeroPlace = HeroPlace;
           HeroPlace = place;*/
        //Debug.Log(placeId);
        HeroPlace heroPlace = NetworkManager.Instance.GetGameboardHeroPlace(posId, placeId);
        SetHeroPlace(heroPlace);
        // enemyArena.GameBoard.GetChild(_heroPos).GetChild(0).parent = enemyArena.HeroList.GetChild(_newPos);
    }
    [PunRPC]
    public void RPC_MoveToThePlayerHeroPlace(int posId, int placeId)
    {
        HeroPlace heroPlace = NetworkManager.Instance.GetPlayerHeroPlace(posId, placeId);
        SetHeroPlace(heroPlace);
        // Debug.Log( this.name + " become enemy? " + isEnemy+" Pos "+heroPlace.gridX+" "+heroPlace.gridY);
    }
    [PunRPC]
    public void RPC_AddToHeroList(int posId, int placeId)
    {
        HeroPlace heroPlace = NetworkManager.Instance.GetHeroListHeroPlace(posId, placeId);
        if (!photonView.isMine)
            transform.Rotate(new Vector3(0, 180, 0));
        SetHeroPlace(heroPlace);
    }
    void SetHeroPlace(HeroPlace heroPlace)
    {
        transform.parent = heroPlace.gameObject.transform;
        transform.localPosition = Vector3.zero;
        LastHeroPlace = heroPlace;
        HeroPlace = heroPlace;
    }
    #endregion
    // Hero will follow the whole path and walk to the destination
    #region follow path

    [PunRPC]
    void RPC_FollowStep(int placeId, int YPos) {
        SyncFollowStep(placeId, YPos);
    }
    void SyncFollowStep(int placeId,int YPos) {
        bool isEnemyPlace;
        if (YPos <= 3)
            isEnemyPlace = true;
        else
            isEnemyPlace = false;
        HeroPlace heroPlace = NetworkManager.Instance.GetOpponentHeroPlace(placeId, isEnemyPlace);
        StartCoroutine(RPC_FollowHeroPlace(heroPlace));
    }
    IEnumerator FollowPath(List<Node> path)
    {
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
            transform.position = Vector3.MoveTowards(transform.position, currentNode.heroPlace.transform.position, 5 * Time.deltaTime);
            yield return null;
        }
    }
    //Hero will just move to the next step
    IEnumerator FollowStep(Node step)
    {
        Debug.Log("FollowStep");
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

    IEnumerator RPC_FollowHeroPlace(HeroPlace step)
    {
        transform.LookAt(step.transform);
        animator.SetBool("Walk", true);
        while (transform.position != step.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, step.transform.position, 3 * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator FindPathAgain() {
        //yield return new WaitForSeconds(2);
        yield return new WaitForSeconds(0.3f);
        followEnemy();
    }
    #endregion

    [PunRPC]
    public void RPC_ShowHpBar(int posid) {
        HeroBar.SetActive(true);
        Vector3 cameraPos = NetworkManager.Instance.getCamera(posid).transform.position;
        Vector3 targetPostition = new Vector3(HeroBar.transform.position.x, cameraPos.y, HeroBar.transform.position.x);
        HeroBar.transform.LookAt(targetPostition);
    }
    /*[PunRPC]
    public void RPC_ChangeHeroStatus(int HeroStatus) {
        HeroState = (HeroState)HeroStatus;
        Debug.Log(HeroState);
        switch (HeroState) {
            case HeroState.Fight:
                animator.SetBool("Walk", false);               
            break;
        }
    }*/
    [PunRPC]
    public void RPC_StopWalk() {
        animator.SetBool("Walk", false);
    }
    IEnumerator basicAttackCoolDown()
    {
        isAttackCooldown = true;
        yield return new WaitForSeconds(1/(AttackSpeed*2.5f));
        isAttackCooldown = false;
    }

}