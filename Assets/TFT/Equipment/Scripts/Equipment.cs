using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public abstract class Equipment : MonoBehaviour, ISelectable
    {
        public string EquipmentName;
        
        public Attribute[] Attributes;
        protected SelectManager SelectManager;
        //public HeroAttribute[] HeroAttributes;
        //public int[] AttributeValues;
        [SerializeField]
        private Hero attachHero;
        public Hero AttachHero
        {
            get { return attachHero; }
            set
            {
                attachHero = value;
                OnInstallEquip();
            }
        }

        public abstract void OnInstallEquip();
        
        [HideInInspector]
        public bool isComponent;
        
        private bool isUse;
        public bool IsUse
        {
            get { return isUse; }
            set
            {
                isUse = value;
                GetComponent<Collider>().enabled = false;
                transform.localScale = Vector3.one;
                transform.eulerAngles = Vector3.zero;
            }
        }

        private int ItemId
        {
            get
            {
                Main.GameManager gm = GameManager.Instance.MainGameManager;
                if (isComponent)
                {
                    for(int i = 0; i < gm.ItemTypes.Length; i++)
                    {
                        if (gm.ItemTypes[i].EquipmentName.Equals(EquipmentName))
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < gm.BlendItemTypes.Length; i++)
                    {
                        if (gm.BlendItemTypes[i].EquipmentName.Equals(EquipmentName))
                        {
                            return i + gm.ItemTypes.Length;
                        }
                    }
                }
                return -1;
            }
        }

        protected virtual void Start()
        {
            SelectManager = SelectManager.Instance;
        }

        private void OnMouseEnter()
        {
            SelectManager.SelectedObject = this;
        }

        private void OnMouseExit()
        {
            SelectManager.SelectedObject = null;
        }
        public void InstallEquipment(Hero _hero)
        {
            AttachHero = _hero;
            Transform parent = AttachHero.GetEquipmentSlot();
            transform.parent = parent;
            parent.parent.parent.GetComponent<EquipmentManager>().AddEquirement(this);
            transform.localPosition = Vector3.zero;
            IsUse = false;
        }
        public void PutDown()
        {
            if (SelectManager.ParentObject.GetComponent<Hero>()!=null)
            {
                AttachHero = SelectManager.ParentObject.GetComponent<Hero>();
                Transform parent = AttachHero.GetEquipmentSlot();
                transform.parent = parent;
                parent.parent.parent.GetComponent<EquipmentManager>().AddEquirement(this);
                IsUse = false;
                AttachHero.photonView.RPC("RPC_InstallEquipment", PhotonTargets.Others, ItemId);
            }
            else
            {
                GetComponent<Collider>().enabled = true;
                transform.parent = SelectManager.ParentObject.transform;
            }
            transform.localPosition = Vector3.zero;
        }

        public void DragUp()
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}