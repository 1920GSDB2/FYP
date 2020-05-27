using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyBomberMan : Hero
{
    float attackSpeedIncrease=0.4f;
    float time = 2.5f;
    public Skill dieSkill;
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        // photonView.RPC("RPC_MeleeSkillAnimation", PhotonTargets.All);

        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.Attack_Speed, attackSpeedIncrease + AttackSpeed);
        StartCoroutine(skillDuration());
    }
    public override void die()
    {
        photonView.RPC("RPC_castDieSkill", PhotonTargets.All, photonView.viewID);
        base.die();      
    }
    IEnumerator skillDuration() {
        yield return new WaitForSeconds(2.5f);
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.Attack_Speed, AttackSpeed- attackSpeedIncrease);
    }
    [PunRPC]
    public void RPC_castDieSkill(int id)
    {
        processDieSkill(id);
    }

    void processDieSkill(int id)
    {
        Character target = PhotonView.Find(id).GetComponent<Character>();
        dieSkill.castSkill(target, SkillPower * 0.35f * AttackDamage, isMirror, isEnemy);

    }
}
