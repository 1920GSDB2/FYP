
public class UndeadSummoner : Hero
{
    public override void setAttribute()
    {
      
        MpRecoverRate = 5f;
    }
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        if (!isMirror)
            SummonUnit();

    }
}
