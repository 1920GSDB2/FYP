using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class GuardianSword : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float shieldValue = 10;
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
            AttachHero.TargetEnemy.photonView.RPC("RPC_addShield", PhotonTargets.All, shieldValue);
        }
    }
}

