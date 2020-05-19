using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT {
    public class EquipmentSlot : MonoBehaviour
    {
        SelectManager SelectManager;
        SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Start()
        {
            SelectManager = SelectManager.Instance;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseOver()
        {
            if (SelectManager.DragObject != null &&
                SelectManager.DragObject as Equipment != null)
            {
                SelectManager.ParentObject = gameObject;
                spriteRenderer.color = Color.black;
            }
        }
        void OnMouseExit()
        {
            spriteRenderer.color = Color.white;

            SelectManager.ParentObject = null;
        }
    }
}

