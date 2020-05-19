using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorKnight : Hero
{
    public override void setAttribute()
    {
        MpRecoverRate = 2f;
        AttackDamage /= 2;
        SkillPower = 50 + AttackDamage * 1.5f;
    }
    public override void UseSkill()
    {

        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_MeleeSkillAnimation", PhotonTargets.All);

    }
    public void SkillAttack() {
            if(!isMirror)
            photonView.RPC("RPC_MeleeSkill", PhotonTargets.All, TargetEnemy.photonView.viewID);
    }
}
