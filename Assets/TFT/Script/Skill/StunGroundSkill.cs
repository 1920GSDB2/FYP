using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGroundSkill : Aoe
{
    private void OnTriggerEnter(Collider other)
    {   
        if (other.tag == "BattleCharacter")
        {
            Character TargetEnemy = other.GetComponent<Character>();
            bool target = other.GetComponent<Character>().isEnemy;
            //    Debug.Log("Collider " + other.name + " isEnemy " + target + " isAlly " + isAlly);
            if (target != isAlly)
            {
                if (!isMirror)
                {
                    other.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage, (byte)DamageType.Magic);
                }
                TargetEnemy.AddNegativeEffect(2.5f, TargetEnemy.NegativeEffectManager.Knock);
            }
        } 
    }
}
