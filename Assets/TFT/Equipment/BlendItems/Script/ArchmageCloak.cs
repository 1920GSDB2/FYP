using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class ArchmageCloak : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private int blockTimes = 3;
        private int currentTimes = 0;

        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.beControlled += OnBeingControlled;
            AttachHero.combatStart += OnCombatStart;
        }

        private void OnDestroy()
        {
            AttachHero.beControlled -= OnBeingControlled;
            AttachHero.combatStart -= OnCombatStart;
        }

        public void OnCombatStart(object sender, EventArgs e)
        {
            currentTimes = 0;
        }

        public void OnBeingControlled(object sender, EventArgs e)
        {
            AttachHero.isBlind = false;
            AttachHero.isSlience = false;
            AttachHero.isStun = false;
            currentTimes++;
        }
    }
}
