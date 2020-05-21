using System;
using UnityEngine;
public class GodOfWar : Hero
{
    public override void setAttribute()
    {
        ManaRecoveryRate = 2f;
        SkillPower = 1;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_castAoeSkill", PhotonTargets.All, TargetEnemy.photonView.viewID);
    }
}
