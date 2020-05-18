using System.Collections;
using UnityEngine;

public class Vikander : Hero
{
    bool isInvincible=false;
    float time = 2;
    public override void setAttribute()
    {
        MpRecoverRate = 2f;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_useSkill", PhotonTargets.All);
       

    }
    [PunRPC]
    public void RPC_useSkill() {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        isInvincible = true;
        StartCoroutine(skillduration());
    }
    public override void syncAdjustHp(float damage,DamageType type)
    {
        if (isInvincible) {
            if (damage < 0)
                damage = 0;
        }
        base.syncAdjustHp(damage,type);
        
    }
    IEnumerator skillduration() {
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
}
