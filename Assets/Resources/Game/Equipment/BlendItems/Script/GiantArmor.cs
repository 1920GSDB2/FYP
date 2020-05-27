using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class GiantArmor : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float healPercentage = 0.15f, belowHpPercentage = 0.1f, recoverySecond = 5f;

        private bool isUseSkill;

        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.combatStart += OnCombatStart;
            AttachHero.hpChange += OnHpChange;
        }

        private void OnDestroy()
        {
            AttachHero.combatStart -= OnCombatStart;
            AttachHero.hpChange -= OnHpChange;
        }

        public void OnCombatStart(object sender, EventArgs e)
        {
            StopAllCoroutines();

            isUseSkill = false;
        }

        public void OnHpChange(object sender, EventArgs e)
        {
            float hpPercentage = AttachHero.Health / AttachHero.MaxHealth;
            if(hpPercentage <= belowHpPercentage && !isUseSkill)
            {
                StartCoroutine(RecoverHp());
            }
        }

        private IEnumerator RecoverHp()
        {
            float second = 0.1f;
            float recoveryValue = AttachHero.MaxHealth * healPercentage * recoverySecond / second;
            while (second <= recoverySecond)
            {
                second += 0.1f;
                AttachHero.TargetEnemy.photonView.RPC("RPC_Heal", PhotonTargets.All, recoveryValue, (byte)DamageType.Heal);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}