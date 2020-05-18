
public class Executioner : Hero
{
    public override void setAttribute()
    {
        MpRecoverRate = 4f;
        SkillPower = 9999;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_MeleeSkillAnimation", PhotonTargets.All);

    }
    public void SkillAttack()
    {
        if (!isMirror)
            photonView.RPC("RPC_MeleeSkill", PhotonTargets.All, TargetEnemy.photonView.viewID);
    }
}
