using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    // Start is called before the first frame update
    public SelectManager SelectManager;
    private void OnMouseEnter()
    {
        SelectManager.SelectedObject = gameObject;
    }

    private void OnMouseExit()
    {
        if (SelectManager.SelectedObject == gameObject)
        {
            SelectManager.SelectedObject = null;
        }
    }
}
