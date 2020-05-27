using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class LunaStaff : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float shieldValue = 550;
        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.combatStart += OnCombatStart;
        }

        private void OnDestroy()
        {
            AttachHero.combatStart -= OnCombatStart;
        }

        public void OnCombatStart(object sender, EventArgs e)
        {

            AttachHero.TargetEnemy.photonView.RPC("RPC_addShield", PhotonTargets.All, shieldValue);
        }
    }
}

