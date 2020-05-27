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
        private float mpPercent = 2.25f;

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

        public void OnEnemyUseSkill(object sender, EventArgs e)
        {
            Character Enemy = (Character)sender;

            float damage = Enemy.MaxMp * mpPercent;

            Enemy.photonView.RPC("RPC_TargetTakeDamage",
                    PhotonTargets.All,
                    AttachHero.AttackDamage * damage,
                    (byte)DamageType.Magic);
        }

        public void OnCombatStart(object sender, EventArgs e)
        {
            foreach(Character enemy in enemies)
            {
                enemy.useSkill -= OnEnemyUseSkill;
            }
            enemies = NetworkManager.Instance.opponent.heroes.ToArray();
            foreach (Character enemy in enemies)
            {
                enemy.useSkill += OnEnemyUseSkill;
            }
        }
    }
}
