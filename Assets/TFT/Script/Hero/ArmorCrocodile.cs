using System;
using UnityEngine;

public class ArmorCrocodile : Hero
{
    public override void setAttribute()
    {
        MpRecoverRate = 1f;
        AttackDamage /= 2;
        SkillPower = 200;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_MeleeSkillAnimation", PhotonTargets.All);

    }
    public void SkillAttack()
    {
        if(!isMirror)
          photonView.RPC("RPC_MeleeSkill", PhotonTargets.All, TargetEnemy.photonView.viewID);
     //   Debug.Log("use melee Skill");
    }

    public override void processMeleeSkill(int id)
    {
        Character target = PhotonView.Find(id).GetComponent<Character>();
        bool reset = false;

        if (!isMirror) {
        //     Debug.Log("Target " + targetEnemy.name + " Hp " + targetEnemy.Health + " Hp predict" + (targetEnemy.Health - SkillPower) + " skill da" + SkillPower + " health " + targetEnemy.Health);
            if (target.Health - SkillPower <= 0) {
                reset = true;
            //    Debug.Log("reset Mp " + (targetEnemy.Health - SkillPower));
            }
        }
        skill.meleeHit(target, SkillPower, isMirror);
        if (reset)
        {
            photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, MaxMp);
        }

    }
}
