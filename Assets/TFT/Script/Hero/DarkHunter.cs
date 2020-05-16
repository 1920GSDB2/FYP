public class DarkHunter : Hero
{
    int attackCount;

    public override void attackTarget()
    {
        
        base.attackTarget();
        if (!isMirror)
        {
            attackCount++;
            if (attackCount >= 3)
            {
                stealMp();
                attackCount = 0;
            }
        }
          
    }
    public void stealMp() {
        float reduceMp = targetEnemy.Mp / 2;
        float heal = reduceMp * 2.5f;
        targetEnemy.photonView.RPC("RPC_ReduceMp", PhotonTargets.All, reduceMp);
        photonView.RPC("RPC_Heal", PhotonTargets.All, heal,(byte)DamageType.Heal);
    }
    public override void UseSkill()
    {
        if (!isMirror)
        {
            photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
            stealMp();
        }
    }

}
