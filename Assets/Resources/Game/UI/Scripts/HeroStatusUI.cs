using System;
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
        public TextMeshProUGUI HeroName;
        public Image Icon;
        public TextMeshProUGUI HP, Mana, AD, MP, AS, CC, PD, MD;
        public GameObject StarPanel;
        public Image[] ItemIcons;
        public Button CrossButton;

        //public Vector3 SelfColor = new Vector3(0.4117647f, 0.1607843f, 1);
        public Color32 SelfColor, EnemyColor;

        public GameObject SkillDisplayPanel;
        public TextMeshProUGUI SkillDescription;

        public GameObject ItemDisplayPanel;
        public TextMeshProUGUI ItemDescription;

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

        public void DisplayItemPanel(int _id)
        {
            if (DisplayHero != null)
            {
                ItemDisplayPanel.SetActive(true);
                ItemDescription.text = DisplayHero.GetComponent<EquipmentManager>().Equipments[_id].ItemDetail;
            }
        }

        public void OffDisplayItemPanel()
        {
            ItemDisplayPanel.SetActive(false);
        }
        public void ShowPanelUI(Hero _hero)
        {
            DisplayHero = _hero;
            foreach (Transform star in StarPanel.transform)
            {
                star.gameObject.SetActive(false);
            }

            if (_hero.photonView.isMine)
            {
                //StatusPanel.GetComponent<Image>().color = new Color(SelfColor.r, SelfColor.g, SelfColor.b, 1);
                StatusPanel.GetComponent<Image>().color = SelfColor;
            }
            else
            {
                //StatusPanel.GetComponent<Image>().color = new Color(EnemyColor.r, EnemyColor.g, EnemyColor.b, 1);
                StatusPanel.GetComponent<Image>().color = EnemyColor;
            }

            HeroName.text = _hero.name.Split(new string[] { "(Clone)" }, StringSplitOptions.None)[0];
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
