using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBoard : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        addEquipmentToSlot();
    }
    public void addEquipmentToSlot() {
        int slotNumber=-1;
        for (int i = 0; i < transform.childCount-1; i++) {
            if (transform.GetChild(i).childCount == 0)
            {
                slotNumber = i;
                break;
            }
        }
    }
}
