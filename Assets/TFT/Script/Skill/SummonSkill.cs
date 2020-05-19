using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TFT;

public class SummonSkill : Skill
{
    public string summonName;

    public override void summon(Character target, float damage, bool isMirror)
    {
        Monster monster = PhotonNetwork.Instantiate(Path.Combine("Prefabs", summonName), Vector3.zero, Quaternion.identity, 0).GetComponent<Monster>();
        monster.GetComponent<PhotonView>().RPC("RPC_MoveToThePlayerHeroPlace", PhotonTargets.All, NetworkManager.Instance.battlePosId, 0);
    }
}
