using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillListInterface : MonoBehaviour
{
   
}
interface IStunAbleSkill {
    void stun();
}
interface IFearSkill
{
    float duration { get; set;}
    ControlSkillType type { get; set; }
}
