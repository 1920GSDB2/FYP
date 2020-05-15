using System;
using UnityEngine;
public class GodOfWar : Hero
{
    public override void UseSkill()
    {
        photonView.RPC("RPC_ReduceMp", PhotonTargets.All, MaxMp);
       


    }
}
