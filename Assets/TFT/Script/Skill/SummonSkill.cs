using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TFT;

public class SummonSkill : Skill
{
    public string summonName;

    public override void summon(HeroPlace heroPlace, bool isEnemy,bool isMirror)
    {
     
        if (!isMirror)
        {
            bool isEnemyPlace=false;
            Monster monster = PhotonNetwork.Instantiate(Path.Combine("Prefabs", summonName), Vector3.zero, Quaternion.identity, 0).GetComponent<Monster>();          
            NetworkManager.Instance.addBattleHeroAdapter(monster,isEnemy, monster.GetComponent<PhotonView>().viewID);
            HeroPlace summonPlace = NetworkManager.Instance.getNeighboursHeroPlace(heroPlace);
            if (summonPlace.gridY >= 3)
                isEnemyPlace = true;
            monster.GetComponent<PhotonView>().RPC("RPC_MoveToThePlayerHeroPlace", PhotonTargets.All, NetworkManager.Instance.battlePosId, summonPlace.PlaceId, isEnemyPlace);
            
        }
    }
}
