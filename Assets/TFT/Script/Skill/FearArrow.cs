
using UnityEngine;

public class FearArrow : Bullet ,IFearSkill
{
    public ControlSkillType type { get; set; } = ControlSkillType.Fear;
    public float duration { get; set; } = 1.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            if (canDamage)
            {
                target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage,(byte)type,duration,(byte)DamageType.Magic);

            }

            Destroy(this.gameObject);

        }

    } 
}
