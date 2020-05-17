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
        heroBar = HeroBarObject.transform.GetChild(0).GetComponent<HpBar>();
      
        animator = GetComponent<Animator>();
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
