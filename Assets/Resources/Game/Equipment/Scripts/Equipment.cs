using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public abstract class Equipment : MonoBehaviour, ISelectable
    {
        public string EquipmentName;

        public Sprite Icon;
        [TextArea]
        public string ItemDetail;
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
        
        //[HideInInspector]
        public bool isComponent;
        
        [SerializeField]
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
            parent.parent.parent.GetComponent<EquipmentManager>().AddEquement(this);
            IsUse = true;
            transform.localPosition = Vector3.zero;
        }
        public void PutDown()
        {
            if (SelectManager.ParentObject.GetComponent<Hero>()!=null)
            {
                AttachHero = SelectManager.ParentObject.GetComponent<Hero>();
                Transform parent = AttachHero.GetEquipmentSlot();
                transform.parent = parent;
                if (parent.parent.parent.GetComponent<EquipmentManager>().AddEquement(this))
                {
                    AttachHero.photonView.RPC("RPC_InstallEquipment", PhotonTargets.Others, ItemId);
                    IsUse = true;
                }
                else
                {
                    EquipmentSlotManager.Instance.AddEquipment(this);
                }
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