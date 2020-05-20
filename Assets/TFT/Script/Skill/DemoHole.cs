using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoHole : Aoe
{
    public float interval;
   
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
                    other.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage, (byte)DamageType.Magic);
                    //  Debug.Log("Stone Skill Hit enemy"+other.name);
                }
            }
        }
    }
    protected override IEnumerator openCollider()
    {
        Debug.Log("open collider");
        yield return new WaitForSeconds(delayOpenCollider);
        collider.enabled = true;
        StartCoroutine(durationDamage());
    }
    IEnumerator durationDamage() {
        yield return new WaitForSeconds(0.1f);
        collider.enabled = false;
        yield return new WaitForSeconds(interval);
        collider.enabled = true;
        StartCoroutine(durationDamage());
    }
}
