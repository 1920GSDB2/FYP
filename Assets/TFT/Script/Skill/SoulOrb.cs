
using UnityEngine;
public class SoulOrb : Bullet
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            if (canDamage)
                target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage,(byte)DamageType.Magic);

             Destroy(this.gameObject);
            //PhotonNetwork.Destroy(this.gameObject);
        }
       
    }
   
}
