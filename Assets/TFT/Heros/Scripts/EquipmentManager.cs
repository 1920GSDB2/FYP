using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class EquipmentManager : MonoBehaviour
    {
        public List<Equipment> Equipments = new List<Equipment>(3);
        public Transform ItemList;
        public Dictionary<HeroAttribute, int> ItemAttribute;

        Plane movePlane;
        float fixedDistance = 0f;
        float hitDist, t;
        Ray camRay;
        Vector3 startPos, point, corPoint;

        public bool AddEquirement(Equipment _addEquirement)
        {
            if (Equipments.Count == 3 && !Equipments[2].isComponent) return false;
            if(Equipments.Count == 0 || !Equipments[Equipments.Count - 1].isComponent)
            { 
                AddAttribute(_addEquirement);
                return true;
            }
            else
            {
                //Equirement Composite
                BlendItem newItem = Composite(((Item)Equipments[Equipments.Count - 1]).ItemType, ((Item)_addEquirement).ItemType);
                if (newItem != null)
                {
                    GameObject item = Instantiate(newItem.gameObject,ItemList);
                    AddAttribute(item.GetComponent<Equipment>());
                }
                return true;
            }
        }

        private void AddAttribute(Equipment _addEquirement)
        {
            for(int i = 0; i < _addEquirement.HeroAttributes.Length; i++)
            {
                HeroAttribute _heroAttribute = _addEquirement.HeroAttributes[i];
                int _attributeValue = _addEquirement.AttributeValues[i];

                if (!ItemAttribute.ContainsKey(_heroAttribute))
                {
                    ItemAttribute.Add(_heroAttribute, _attributeValue);
                }
                else
                {
                    ItemAttribute[_heroAttribute] += _attributeValue;
                }
            }
            Equipments.Add(_addEquirement);
            //_addEquirement.transform.parent = ItemList;
        }

        private BlendItem Composite(ItemType _lastEquirementType, ItemType _addEquirement)
        {
            BlendItem[] _blendItemTypes = GameManager.Instance.MainGameManager.BlendItemTypes;
            BlendItem _blendItem = null;
            foreach (BlendItem blendItem in _blendItemTypes)
            {
                if ((blendItem.components[0].ItemType == _lastEquirementType && blendItem.components[1].ItemType == _addEquirement) ||
                    (blendItem.components[0].ItemType == _addEquirement && blendItem.components[1].ItemType == _lastEquirementType))
                {
                    _blendItem = blendItem;
                    break;
                }
            }
            return _blendItem;
        }
    }
}

