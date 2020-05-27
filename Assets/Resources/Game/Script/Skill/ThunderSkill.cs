using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderSkill : Aoe
{
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.tag == "BattleCharacter"){
            Character Target = other.GetComponent<Character>();
            bool target = other.GetComponent<Character>().isEnemy;
            if (target != isAlly)
            {
                if (!isMirror)
                {
                    Target.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage, (byte)DamageType.Magic);
                }
                //Target.DebugTest();

                Target.AddNegativeEffect(2f, Target.NegativeEffectManager.Stun);
            }
                       
        }
      
    }
}
