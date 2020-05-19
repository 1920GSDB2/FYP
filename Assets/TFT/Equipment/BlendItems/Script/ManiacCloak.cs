using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class ManiacCloak : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float shieldValue = 550;

        private Character[] enemies;

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
            enemies = NetworkManager.Instance.opponent.heroes.ToArray();
        }
    }
}
