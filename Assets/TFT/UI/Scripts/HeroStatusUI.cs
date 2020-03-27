using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class HeroStatusUI : MonoBehaviour
    {
        private SelectManager SelectManager;
        public GameObject StatusPanel;
        public Image Icon;
        public TextMeshProUGUI HP, Mana, AD, MP, AS, CC, PD, MD;
        public GameObject StarPanel;
        bool isShowingUI;

        // Start is called before the first frame update
        void Start()
        {
            SelectManager = SelectManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            StatusPanel.SetActive(ShowPanelUI());
            
        }

        private bool ShowPanelUI()
        {
            if (SelectManager.DragObject != null && SelectManager.DragObject.GetComponent<Hero>() != null)
            {
                if (isShowingUI) return true;
                foreach(Transform star in StarPanel.transform)
                {
                    star.gameObject.SetActive(false);
                }
                Hero _draggingObject = SelectManager.DragObject.GetComponent<Hero>();
                //Name.text = _draggingObject.name;
                Icon.sprite = _draggingObject.Icon;
                HP.text = _draggingObject.Health.ToString();
                Mana.text = _draggingObject.MaxMp.ToString();
                AD.text = _draggingObject.AttackDamage.ToString();
                MP.text = _draggingObject.SkillPower.ToString();
                AS.text = _draggingObject.AttackSpeed.ToString();
                CC.text = _draggingObject.BasicCritcalChance.ToString();
                PD.text = _draggingObject.PhysicalDefense.ToString();
                MD.text = _draggingObject.MagicDefense.ToString();
                int _starCount = (int)_draggingObject.HeroLevel;
                while (_starCount >= 0)
                {
                    StarPanel.transform.GetChild(_starCount).gameObject.SetActive(true);
                    _starCount--;
                }
                isShowingUI = true;
                return true;
            }
            else
            {
                isShowingUI = false;
                return false;
            }
        }
    }
}
