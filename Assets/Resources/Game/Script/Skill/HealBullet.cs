using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBullet : Bullet
{
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            if (canDamage)
                target.GetComponent<PhotonView>().RPC("RPC_Heal", PhotonTargets.All, attackDamage, (byte)DamageType.Heal);

            Destroy(this.gameObject);
        }
    }
}
