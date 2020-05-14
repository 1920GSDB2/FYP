

public class RogueWarrior : Hero
{
    public override void setAttribute()
    {
        MpRecoverRate = 2.25f;
        SkillPower = 50 + AttackDamage * 1.5f;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_MeleeSkillAnimation", PhotonTargets.All);

    }
    public void SkillAttack() {
            photonView.RPC("RPC_MeleeSkill", PhotonTargets.All, targetEnemy.photonView.viewID);
    }
   
}
