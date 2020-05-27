using UnityEngine;
public class FallenVirago : Hero
{
    float tempHealth;
    float tempMaxHp;
    float tempAttackDamage;
    public override void targetDie()
    {
        base.targetDie();      
        if (!isMirror)
        {
            tempHealth += 300;
            tempMaxHp += 300;
            tempAttackDamage += 40;
            photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.maxHp, MaxHealth + 300);
            photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.Attack, AttackDamage + 40);
            photonView.RPC("RPC_Heal", PhotonTargets.All, 300f, (byte)DamageType.Heal);
        }
      
    }
    public override void ResetStatus()
    {
        base.ResetStatus();
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.maxHp, MaxHealth -tempHealth);
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.Attack, AttackDamage - 40);
        tempHealth = 0;
        tempAttackDamage = 0;
    }
}
