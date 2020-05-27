using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTargetSkill : Skill
{
    public GameObject skillModel;
    public virtual void castSkill(Character target) {
        Instantiate(skillModel, target.transform.position,Quaternion.identity);
    }
}
