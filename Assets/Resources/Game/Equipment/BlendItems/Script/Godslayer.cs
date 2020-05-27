using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class Godslayer : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float belowHP = 0.5f, extraDamage = 0.5f;

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
            float targetHpPercent = AttachHero.TargetEnemy.Health / AttachHero.TargetEnemy.MaxHealth;
            if(targetHpPercent <= belowHP)
            {
                AttachHero.TargetEnemy.photonView.
                    RPC("RPC_TargetTakeDamage", 
                    PhotonTargets.All, 
                    AttachHero.AttackDamage * extraDamage, 
                    (byte)DamageType.Physical);
            }
        }
    }
}

