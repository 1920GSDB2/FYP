using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class Equipment : MonoBehaviour, ISelectable
    {
        public SelectManager SelectManager;
        public HeroAttribute[] HeroAttributes;
        public int[] AttributeValues;
        //[HideInInspector]
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

        public virtual void Start()
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

        public void PutDown()
        {
            if (SelectManager.ParentObject.GetComponent<Hero>()!=null)
            {
                Transform parent = SelectManager.ParentObject.GetComponent<Hero>().GetEquipmentSlot();
                transform.parent = parent;
                parent.parent.parent.GetComponent<EquipmentManager>().AddEquirement(this);
                IsUse = false;
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