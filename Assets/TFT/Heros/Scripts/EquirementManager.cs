using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class EquirementManager : MonoBehaviour
    {
        public List<Equirement> Equirements = new List<Equirement>(3);
        public Transform ItemList;
        public Dictionary<HeroAttribute, int> ItemAttribute;

        public bool AddEquirement(Equirement _addEquirement)
        {
            if (Equirements.Count == 3 && !Equirements[2].isComponent) return false;
            else if (!Equirements[Equirements.Count - 1].isComponent)
            {
                AddAttribute(_addEquirement);
                return true;
            }
            else
            {
                //Equirement Composite
                BlendItem newItem = Composite(((Item)Equirements[Equirements.Count - 1]).ItemType, ((Item)_addEquirement).ItemType);
                if (newItem != null)
                {
                    GameObject item = Instantiate(newItem.gameObject,ItemList);
                    AddAttribute(item.GetComponent<Equirement>());
                }
                return true;
            }
        }

        private void AddAttribute(Equirement _addEquirement)
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
            Equirements.Add(_addEquirement);
            _addEquirement.transform.parent = ItemList;
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

