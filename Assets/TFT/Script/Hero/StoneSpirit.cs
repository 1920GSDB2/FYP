public class StoneSpirit : Hero
{
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_castAoeSkill",PhotonTargets.All,photonView.viewID);
    }
}
