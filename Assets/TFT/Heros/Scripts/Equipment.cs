using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public class Equipment : MonoBehaviour
    {
        private SelectManager SelectManager;
        public Image Icon;
        public HeroAttribute[] HeroAttributes;
        public int[] AttributeValues;
        //[HideInInspector]
        public bool isComponent;
        Transform lastTransform, currTransform;

        public bool isDrag;

        Plane movePlane;
        float fixedDistance = 0f;
        float hitDist, t;
        Ray camRay;
        Vector3 startPos, point, corPoint;

        public virtual void Start()
        {
            SelectManager = SelectManager.Instance;
        }
        // Update is called once per frame
        public virtual void Update()
        {
            if (SelectManager.DragObject != gameObject && transform.parent != lastTransform)
            {
                currTransform = transform.parent;
                currTransform.parent.parent.GetComponent<EquipmentManager>().AddEquirement(this);
                lastTransform = currTransform;
            }
        }

        private void OnMouseEnter()
        {
            SelectManager.SelectedObject = gameObject;
        }

        private void OnMouseExit()
        {
            if (TFT.SelectManager.Instance.SelectedObject == gameObject)
            {
                TFT.SelectManager.Instance.SelectedObject = null;
            }
        }
    }
}