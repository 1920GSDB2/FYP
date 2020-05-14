using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSkill : Skill
{
    public ControlSkillType type;
    public float duration;
    public GameObject hitEffect;

    public override void meleeHit(Character target,float damage,bool isMirror)
    {
       GameObject effect= Instantiate(hitEffect, target.transform.position, Quaternion.identity) as GameObject;
       Destroy(effect, 1.5f);
        if (type != ControlSkillType.No && type != null)
        {
            if(!isMirror)
            target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage, (byte)type, duration);
        }
        else {
            if (!isMirror)
            {
                target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage);
                Debug.Log("deal melee dmaage");
            }
        }

    }
}
