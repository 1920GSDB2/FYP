using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHeal : Aoe
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isMirror)
        {
            if (other.tag == "BattleCharacter")
            {
                bool target = other.GetComponent<Character>().isEnemy;
                //    Debug.Log("Collider " + other.name + " isEnemy " + target + " isAlly " + isAlly);
                if (target == isAlly)
                {
                    other.GetComponent<PhotonView>().RPC("RPC_Heal", PhotonTargets.All, damage, (byte)DamageType.Heal);
                    other.GetComponent<PhotonView>().RPC("RPC_addShield", PhotonTargets.All, damage);
                    //  Debug.Log("Stone Skill Hit enemy"+other.name);
                }
            }
        }
      
    }
}
