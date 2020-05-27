using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayerBullet : Bullet
{
    private void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            if (canDamage)
            {
            //    target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage, (byte)damageType);
            }

            Destroy(this.gameObject);
        }
    }
}
