using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class PrimalStaff : BlendItem
    {
        [Header("Specific Attribute")]
        [SerializeField]
        private float manaValue = 50;
        // Start is called before the first frame update
        public override void OnInstallEquip()
        {
            AttachHero.useSkill += OnHeroUseSkill;
        }

        private void OnDestroy()
        {
            if (AttachHero != null)
                AttachHero.useSkill -= OnHeroUseSkill;
        }

        public void OnHeroUseSkill(object sender, EventArgs e)
        {
            AttachHero.photonView.RPC("RPC_IncreaseMp", PhotonTargets.All, manaValue);
        }
    }
}