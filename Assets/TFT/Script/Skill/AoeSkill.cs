using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeSkill : Skill
{
    public GameObject hitEffect;

    public override void castSkill(Character target, float damage, bool isMirror,bool isEnemy)
    {
        Quaternion rotation = Quaternion.Euler(0,target.transform.rotation.eulerAngles.y,0);    
        Aoe effect = Instantiate(hitEffect, target.transform.position, rotation).GetComponent<Aoe>();
        effect.setDamage(damage,isMirror,isEnemy);
        Destroy(effect.gameObject, 3f);
    }

}
