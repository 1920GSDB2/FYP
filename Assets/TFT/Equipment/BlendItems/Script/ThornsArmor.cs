using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{

    public class ThornsArmor : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float bouncePercentage = 0.22f;
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
            Character attacker = (Character)sender;
            float bounceValue = (attacker.AttackDamage - AttachHero.PhysicalDefense) * bouncePercentage;
            attacker.photonView.RPC("RPC_TargetTakeDamage",
                    PhotonTargets.All,
                    bounceValue,
                    (byte)DamageType.Magic);
        }
    }
}
