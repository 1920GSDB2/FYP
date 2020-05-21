using TFT;
public class GodOfWizard : Hero
{
    public override void setAttribute()
    {
        AttackSpeed = 1.2f;
        SkillPower = 10f;
        ManaRecoveryRate = 2.5f;
    }
    public override void UseSkill()
    {

        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        if (!isMirror)
        {
            Character target = NetworkManager.Instance.getRandomCharacter(isEnemy);
            photonView.RPC("RPC_castAoeSkill", PhotonTargets.All, target.photonView.viewID);
        }

    }
    public void godOfWizardAttack() {
        if (!isMirror)
        {
            if (TargetEnemy != null)
            {
                photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, 10f * ManaRecoveryRate);
                Character target = NetworkManager.Instance.getRandomCharacter(isEnemy);
                photonView.RPC("RPC_Shoot", PhotonTargets.All, target.photonView.viewID);
            }
        }
    }
}
