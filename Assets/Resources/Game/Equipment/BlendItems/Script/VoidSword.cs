using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class VoidSword : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float vampirePercentage = 0.2f;
        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.attack += OnHeroAttack;
        }

        private void OnDestroy()
        {
            AttachHero.attack -= OnHeroAttack;
        }

        public void OnHeroAttack(object sender, EventArgs e)
        {
            float healValue = vampirePercentage * AttachHero.AttackDamage -
                AttachHero.TargetEnemy.PhysicalDefense;

            AttachHero.TargetEnemy.photonView.RPC("RPC_Heal", PhotonTargets.All, healValue, (byte)DamageType.Heal);
        }
    }
}

