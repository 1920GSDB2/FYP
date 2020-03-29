using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class Equipment : MonoBehaviour
    {
        public SelectManager SelectManager;
        public Image Icon;
        public HeroAttribute[] HeroAttributes;
        public int[] AttributeValues;
        //[HideInInspector]
        public bool isComponent;
        public Transform lastTransform, currTransform;

        public bool isDrag;
        private bool isUse;
        public bool IsUse
        {
            get { return isUse; }
            set
            {
                isUse = value;
                GetComponent<Collider>().enabled = false;
                transform.localScale = Vector3.one;
            }
        }

        Plane movePlane;
        float fixedDistance = 0f;
        float hitDist, t;
        Ray camRay;
        Vector3 startPos, point, corPoint;

        public virtual void Start()
        {
            SelectManager = SelectManager.Instance;
            currTransform = transform.parent;
        }
        // Update is called once per frame
        public virtual void Update()
        {
            if (!IsUse && SelectManager.DragObject != null &&
                SelectManager.DragObject == gameObject &&
                //transform.parent.parent.GetComponent<EquipmentManager>() != null &&
                transform.parent != lastTransform)
            {
                //GetComponent<MeshRenderer>().enabled = false;
                SelectManager.DragObject = null;
                currTransform = transform.parent;
                currTransform.parent.parent.GetComponent<EquipmentManager>().AddEquirement(this);
                lastTransform = currTransform;
                IsUse = false;
            }
        }

        private void OnMouseEnter()
        {
            SelectManager.SelectedObject = gameObject;
            lastTransform = transform.parent;
        }

        private void OnMouseExit()
        {
            if (SelectManager.SelectedObject == gameObject)
            {
                SelectManager.SelectedObject = null;
            }
        }
    }
}