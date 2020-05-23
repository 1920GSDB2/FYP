using UnityEngine;
public class FallenVirago : Hero
{
    float tempHealth;
    float tempMaxHp;
    public override void targetDie()
    {
        base.targetDie();      
        if (!isMirror)
        {
            tempHealth += 300;
            tempMaxHp += 300;
            photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.maxHp, MaxHealth + 300);
            photonView.RPC("RPC_Heal", PhotonTargets.All, 300f, (byte)DamageType.Heal);
        }
      
    }
    public override void ResetStatus()
    {
        base.ResetStatus();
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.maxHp, MaxHealth -tempHealth);
        Debug.Log("maxHealth" + MaxHealth + "temp " + tempHealth);
        tempHealth = 0;      
    }
}
