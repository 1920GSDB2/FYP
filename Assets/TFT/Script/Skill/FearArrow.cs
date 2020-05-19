
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
                Character TargetEnemy = other.GetComponent<Character>();
                TargetEnemy.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage,(byte)DamageType.Magic);
                TargetEnemy.AddNegativeEffect(2.5f, TargetEnemy.NegativeEffectManager.Blind);

            }

            Destroy(this.gameObject);

        }

    } 
}
