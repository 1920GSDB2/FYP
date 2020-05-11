using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TFT;

[Serializable]
public class Monster : Character
{   
   // public HeroPlace spawnHeroPlace;
    //int position;
    private void Start()
    {

        MaxHealth = 1000;
        Health = MaxHealth;
        photonView = GetComponent<PhotonView>();
        hpBar = HeroBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        mpBar = HeroBar.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {

        Vector3 targetPostition = new Vector3(HeroBar.transform.position.x, cameraPos.y, HeroBar.transform.position.x);
        HeroBar.transform.LookAt(targetPostition);
        if (photonView.isMine)
        {
            if (HeroState == HeroState.Idle)
            {
                if (targetEnemy == null)
                    targetEnemy = NetworkManager.Instance.getCloestEnemyTarget(isEnemy, transform);
                else
                {
                    HeroState = HeroState.Walking;
                    checkWithInAttackRange();
                    FollowEnemy();
                }

            }
            if (HeroState == HeroState.Walking)
                checkWithInAttackRange();
            if (HeroState == HeroState.Fight)
            {
                if (!isAttackCooldown)
                    photonView.RPC("RPC_AttackAnimation", PhotonTargets.All);
                animator.SetTrigger("Attack");

            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            //  targetEnemy = testHero;        
            //animator.SetBool("Attack",true);
            //  gameObject.SetActive(false);
            //  Debug.Log(name + " Health " + Health + " / " + MaxHealth);
            //   photonView.RPC("test", PhotonTargets.All);
            
            Health = 0;
            photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All,200f);
            //  photonView.RPC("RPC_Animation", PhotonTargets.All);

        }

        if (Input.GetKeyDown(KeyCode.O))
        {
        }
    }
    public override void die()
    {
        if (photonView.isMine)
        {
            HeroState = HeroState.Die;
               HeroPlace.leavePlace();
               NetworkManager.Instance.battleHeroDie(isEnemy, this);
               MonsterWaveManager.Instance.monsterDie();
            PhotonNetwork.Destroy(this.gameObject);
        }
    }


}
