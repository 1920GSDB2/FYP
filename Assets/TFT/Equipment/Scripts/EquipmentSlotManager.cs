using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        public static EquipmentSlotManager Instance;

        [SerializeField]
        private EquipmentSlot[] equipmentSlots;

        private void Awake()
        {
            Instance = this;
        }

        public void AddEquipment(Equipment _equipment)
        {
            foreach (EquipmentSlot equipmentSlot in equipmentSlots)
            {
                if(equipmentSlot.transform.childCount <= 0)
                {
                    _equipment.transform.parent = equipmentSlot.transform;
                    _equipment.gameObject.transform.localPosition = Vector3.zero;
                    _equipment.GetComponent<Collider>().enabled = true;
                    return;
                }
            }
            DestroyImmediate(_equipment.gameObject);
        }

    }
}

