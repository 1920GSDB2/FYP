using TFT;
public class GodOfWizard : Hero
{
    public override void setAttribute()
    {
        AttackSpeed = 1.2f;
        SkillPower = 10f;
        MpRecoverRate = 2.5f;
    }
    public override void UseSkill()
    {

        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        Character target = NetworkManager.Instance.getRandomCharacter(isEnemy);
        photonView.RPC("RPC_castAoeSkill", PhotonTargets.All,target.photonView.viewID);

    }
    public void godOfWizardAttack() {
        if (!isMirror)
        {
            if (targetEnemy != null)
            {
                photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, 10f * MpRecoverRate);
                Character target = NetworkManager.Instance.getRandomCharacter(isEnemy);
                photonView.RPC("RPC_Shoot", PhotonTargets.All, target.photonView.viewID);
            }
        }
    }
}
