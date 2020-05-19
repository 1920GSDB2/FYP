using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        public EquipmentSlotManager Instance;

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
                    _equipment.transform.position = Vector3.zero;
                    return;
                }
            }
            DestroyImmediate(_equipment.gameObject);
        }

    }
}

