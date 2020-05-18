using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class SacrificialSword : Equipment
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float extraHpValue = 0.1f;
        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.targetChange += OnTargetChange;
        }

        private void OnDestroy()
        {
            AttachHero.targetChange -= OnTargetChange;
        }

        public void OnTargetChange(object sender, EventArgs e)
        {
            AttachHero.TargetEnemy.hpChange += OnTargetHpChange;
        }

        public void OnTargetHpChange(object sender, EventArgs e)
        {
            float targetHp = ((Character)sender).Health;
            if(targetHp <= 0)
            {
                float extraHp = AttachHero.MaxHealth * extraHpValue;
                AttachHero.photonView.RPC("RPC_AddMaxHP", PhotonTargets.All, extraHp);
                AttachHero.TargetEnemy.hpChange -= OnTargetHpChange;
            }
        }
        
    }

}