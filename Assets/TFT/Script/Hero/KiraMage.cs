

using TFT;

public class KiraMage : Hero
{
   
    public override void UseSkill() {

        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
        Character character = NetworkManager.Instance.getFurthestEnemyTarget(!isEnemy,transform);
        //  photonView.RPC("RPC_castUnitTargetSkill", PhotonTargets.All, targetEnemy.transform.position.x, targetEnemy.transform.position.z, NetworkManager.Instance.playerId, NetworkManager.Instance.opponent.opponentId);
        photonView.RPC("RPC_castUnitTargetSkill", PhotonTargets.All, character.photonView.viewID);
    }
}
