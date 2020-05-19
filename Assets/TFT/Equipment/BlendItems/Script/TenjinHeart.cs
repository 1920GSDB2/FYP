using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class TenjinHeart : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float recoverPercent = 0.01f;
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
            StopAllCoroutines();
            StartCoroutine(RecoverHp());
        }

        private IEnumerator RecoverHp()
        {
            float recoveryValue = AttachHero.MaxHealth * recoverPercent;
            while (AttachHero.Health > 0)
            {
                AttachHero.TargetEnemy.photonView.RPC("RPC_Heal", PhotonTargets.All, recoveryValue, (byte)DamageType.Heal);
                yield return new WaitForSeconds(1);
            }
        }
    }
}