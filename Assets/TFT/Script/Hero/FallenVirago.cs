
public class FallenVirago : Hero
{
    float tempHealth;
    float tempMaxHp;
    public override void targetDie()
    {
        base.targetDie();
        tempHealth += 300;
        tempMaxHp += 300;
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.maxHp,MaxHealth+300);
        photonView.RPC("RPC_Heal",PhotonTargets.All,300f);
      
    }
    public override void resetStatus()
    {
        base.resetStatus();
        photonView.RPC("RPC_SyncHeroAttribute", PhotonTargets.All, (byte)HeroAttribute.maxHp, MaxHealth -tempHealth);
    }
}
