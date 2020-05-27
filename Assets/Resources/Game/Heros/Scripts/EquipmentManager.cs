using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class EquipmentManager : MonoBehaviour
    {
        private Hero Hero;
        public List<Equipment> Equipments = new List<Equipment>(3);
        public Transform ItemList;
        public Dictionary<HeroAttribute, int> ItemAttribute = new Dictionary<HeroAttribute, int>();

        Plane movePlane;
        float fixedDistance = 0f;
        float hitDist, t;
        Ray camRay;
        Vector3 startPos, point, corPoint;

        private void Start()
        {
            Hero = GetComponent<Hero>();
        }

        public bool AddEquement(Equipment _addEquirement)
        {
            if (Equipments.Count == 3 && !Equipments[2].isComponent)
            {
                return false;
            }
            if(Equipments.Count == 0 || !Equipments[Equipments.Count - 1].isComponent ||!_addEquirement.isComponent)
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
                    int _pos = Equipments.Count - 1;
                    GameObject _item = Instantiate(newItem.gameObject,ItemList.GetChild(_pos));
                    _item.transform.localScale = Vector3.one;

                    Destroy(Equipments[Equipments.Count - 1].gameObject);
                    Destroy(_addEquirement.gameObject);
                    Equipments.RemoveAt(Equipments.Count - 1);

                    AddAttribute(newItem);
                }
                else
                {
                    AddAttribute(_addEquirement);
                }
                return true;
            }
        }

        private void AddAttribute(Equipment _addEquirement)
        {
            for(int i = 0; i < _addEquirement.Attributes.Length; i++)
            {
                HeroAttribute _heroAttribute = _addEquirement.Attributes[i].HeroAttribute;
                int _attributeValue = _addEquirement.Attributes[i].AttributeValue;

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
            AttributeHandler();

            //_addEquirement.transform.parent = ItemList;
        }
        private void AttributeHandler()
        {
            //Reset Added Attribute
            Hero.ResetAttribute();
            foreach (HeroAttribute attriType in Enum.GetValues(typeof(HeroAttribute)))
            {
                if (ItemAttribute.ContainsKey(attriType))
                {
                    AddHeroAttribute(attriType, ItemAttribute[attriType]);
                }
            }
        }
        private void AddHeroAttribute(HeroAttribute _type, int _value)
        {
            switch (_type)
            {
                case HeroAttribute.Attack:
                    GetComponent<Hero>().AttackDamage += _value;
                    break;
                case HeroAttribute.Attack_Speed:
                    GetComponent<Hero>().AttackSpeed += _value;
                    break;
                case HeroAttribute.Critical_Cahnce:
                    GetComponent<Hero>().BasicCritcalChance += _value;
                    break;
                case HeroAttribute.Magic_Defense:
                    GetComponent<Hero>().MagicDefense += _value;
                    break;
                case HeroAttribute.Mana:
                    GetComponent<Hero>().MaxMp += _value;
                    break;
                case HeroAttribute.Health:
                    GetComponent<Hero>().Health += _value;
                    break;
                case HeroAttribute.Physic_Defense:
                    GetComponent<Hero>().PhysicalDefense += _value;
                    break;
                case HeroAttribute.Skill_Damage:
                    GetComponent<Hero>().SkillPower += _value;
                    break;
            }
        }
        private BlendItem Composite(ItemType _lastEquirementType, ItemType _addEquirement)
        {
            BlendItem[] _blendItemTypes = (BlendItem[]) GameManager.Instance.MainGameManager.BlendItemTypes;
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

