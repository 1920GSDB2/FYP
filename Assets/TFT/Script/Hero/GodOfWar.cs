using System;
using UnityEngine;
public class GodOfWar : Hero
{
    public override void setAttribute()
    {
        MpRecoverRate = 2f;
        SkillPower = 1;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        if(!isMirror)
        photonView.RPC("RPC_castAoeSkill", PhotonTargets.All, TargetEnemy.photonView.viewID);
    }
}
