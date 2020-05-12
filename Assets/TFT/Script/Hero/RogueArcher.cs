
using TFT;
public class RogueArcher : Hero
{
    public override void UseSkill()
    {

        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_castUnitTargetSkill", PhotonTargets.All, targetEnemy.photonView.viewID);
       
    }
}
