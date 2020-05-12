
using TFT;
using UnityEngine;

public class Lonnie : Hero
{
    public override void setAttribute()
    {
        MpRecoverRate = 2.5f;
    }
 
    public override void UseSkill()
    {

        Character target = NetworkManager.Instance.getRandomCharacter(isEnemy);
        Debug.Log("target " + target.name + " id " + target.photonView.viewID);
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        photonView.RPC("RPC_castUnitTargetSkill", PhotonTargets.All, target.photonView.viewID);

    }
}
