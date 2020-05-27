using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class HollyArmor : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float healValue = 20, recoveryMana = 5;

        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.beAttacked += OnBeingAttacked;
        }

        private void OnDestroy()
        {
            AttachHero.beAttacked -= OnBeingAttacked;
        }

        public void OnBeingAttacked(object sender, EventArgs e)
        {
            AttachHero.photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, recoveryMana);
            AttachHero.TargetEnemy.photonView.RPC("RPC_Heal", PhotonTargets.All, healValue, (byte)DamageType.Heal);
        }
    }
}