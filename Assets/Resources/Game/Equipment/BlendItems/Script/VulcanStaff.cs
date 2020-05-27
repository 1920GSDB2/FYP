using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class VulcanStaff : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float burnTime = 5;

        private bool isFirstAttack;

        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.targetChange += OnTargetChange;
            AttachHero.attack += OnHeroAttack;
            AttachHero.combatStart += OnCombatStart;
        }

        private void OnDestroy()
        {
            AttachHero.targetChange -= OnTargetChange;
            AttachHero.attack -= OnHeroAttack;
        }

        public void OnCombatStart(object sender, EventArgs e)
        {
            isFirstAttack = false;
        }

        public void OnTargetChange(object sender, EventArgs e)
        {
            isFirstAttack = false;
        }

        public void OnHeroAttack(object sender, EventArgs e)
        {
            if (isFirstAttack)
            {
                AttachHero.TargetEnemy.AddNegativeEffect(5, AttachHero.TargetEnemy.NegativeEffectManager.Burn);
            }
        }
    }
}

