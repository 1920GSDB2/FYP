using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class HeroStatusUI : MonoBehaviour
    {
        public static HeroStatusUI Instance;
        public GameObject StatusPanel;
        public Image Icon;
        public TextMeshProUGUI HP, Mana, AD, MP, AS, CC, PD, MD;
        public GameObject StarPanel;
        public Image[] ItemIcons;
        public Button CrossButton;

        public GameObject SkillDisplayPanel;
        public TextMeshProUGUI SkillDescription;

        private Hero DisplayHero;

        private void Awake()
        {
            Instance = this;
            OffPanelUI();
            OffDisplaySkillPanel();
        }
        private void Start()
        {
            CrossButton.onClick.AddListener(delegate { OffPanelUI(); });
        }
        private void Update()
        {
            if (DisplayHero != null)
            {
                ShowPanelUI(DisplayHero);
            }
        }
        public void OffPanelUI()
        {
            StatusPanel.SetActive(false);
            DisplayHero = null;
        }

        public void DisplaySkillPanel()
        {
            if (DisplayHero != null)
            {
                SkillDisplayPanel.SetActive(true);
                SkillDescription.text = DisplayHero.SkillDescription;
            }
        }
        public void OffDisplaySkillPanel()
        {
            SkillDisplayPanel.SetActive(false);
        }
        public void ShowPanelUI(Hero _hero)
        {
            DisplayHero = _hero;
            foreach (Transform star in StarPanel.transform)
            {
                star.gameObject.SetActive(false);
            }
            Icon.sprite = _hero.Icon;
            HP.text = _hero.Health.ToString();
            Mana.text = _hero.MaxMp.ToString();
            AD.text = _hero.AttackDamage.ToString();
            MP.text = _hero.SkillPower.ToString();
            AS.text = _hero.AttackSpeed.ToString();
            CC.text = _hero.BasicCritcalChance.ToString();
            PD.text = _hero.PhysicalDefense.ToString();
            MD.text = _hero.MagicDefense.ToString();
            int _starCount = (int)_hero.HeroLevel;
            foreach (Image ItemIcon in ItemIcons) ItemIcon.gameObject.SetActive(false);
            Equipment[] Equipments = _hero.GetComponent<EquipmentManager>().Equipments.ToArray();
            for (int i = 0; i < Equipments.Length; i++)
            {
                ItemIcons[i].gameObject.SetActive(true);
                ItemIcons[i].sprite = Equipments[i].Icon;
            }
            while (_starCount >= 0)
            {
                StarPanel.transform.GetChild(_starCount).gameObject.SetActive(true);
                _starCount--;
            }
            StatusPanel.SetActive(true);

        }
    }
}
