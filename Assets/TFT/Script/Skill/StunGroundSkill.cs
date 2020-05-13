using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGroundSkill : Aoe
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isMirror)
        {
            if (other.tag == "Character")
            {
                bool target = other.GetComponent<Character>().isEnemy;
                if (target != isAlly)
                {
                    other.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage);
                }
            }
        }
        //  target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage, (byte)type, duration);
    }
}
