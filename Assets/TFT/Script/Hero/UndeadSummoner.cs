using System.Collections;
using UnityEngine;

public class UndeadSummoner : Hero
{
    bool isInvincible = false;
    public override void setAttribute()
    {
   
        MpRecoverRate = 2f;
        SkillPower = 2f;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_castAoeSkill", PhotonTargets.All, photonView.viewID);
        photonView.RPC("RPC_MeleeSkillAnimation", PhotonTargets.All);
        photonView.RPC("Sync_Skill", PhotonTargets.All);

    }
    public override void syncAdjustHp(float damage, DamageType type)
    {
        if (isInvincible)
        {
            if (damage < 0)
                damage = 0;
        }
        base.syncAdjustHp(damage, type);

    }
    [PunRPC]
    public void Sync_Skill() {
        isInvincible = true;
        StartCoroutine(skillduration());
    }
    IEnumerator skillduration()
    {
       
        yield return new WaitForSeconds(3);
        isInvincible = false;
    }
    //AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
}
