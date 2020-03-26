using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TFT
{
    public class HeroStatusUI : MonoBehaviour
    {
        private SelectManager SelectManager;
        public GameObject StatusPanel;
        public TextMeshProUGUI Name, HP, Mana, AD, MP, AS, CC, PD, MD;

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
                Hero _draggingObject = SelectManager.DragObject.GetComponent<Hero>();
                Name.text = _draggingObject.name;
                HP.text = _draggingObject.Health.ToString();
                Mana.text = _draggingObject.MaxMp.ToString();
                AD.text = _draggingObject.AttackDamage.ToString();
                MP.text = _draggingObject.SkillPower.ToString();
                AS.text = _draggingObject.AttackSpeed.ToString();
                CC.text = _draggingObject.BasicCritcalChance.ToString();
                PD.text = _draggingObject.PhysicalDefense.ToString();
                MD.text = _draggingObject.MagicDefense.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
