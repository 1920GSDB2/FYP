using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGroundSkill : Aoe
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isMirror)
        {
            if (other.tag == "BattleCharacter")
            {
                bool target = other.GetComponent<Character>().isEnemy;
               //    Debug.Log("Collider " + other.name + " isEnemy " + target + " isAlly " + isAlly);
                if (target != isAlly)
                {
                    other.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage,(byte)DamageType.Magic);
                  //  Debug.Log("Stone Skill Hit enemy"+other.name);
                }
            }
        }
      //  Debug.Log("Stone Skill enter" + other.name);

        //  target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage, (byte)type, duration);
    }
}
