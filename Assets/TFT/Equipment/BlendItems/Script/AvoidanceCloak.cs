using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class AvoidanceCloak : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float shieldValue = 100;

        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.useSkill += OnUsingSkill;
        }

        private void OnDestroy()
        {
            AttachHero.useSkill -= OnUsingSkill;
        }

        public void OnUsingSkill(object sender, EventArgs e)
        {
            AttachHero.TargetEnemy.photonView.RPC("RPC_addShield", PhotonTargets.All, shieldValue);
        }
    }
}
