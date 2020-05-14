using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeSkill : Skill
{
    public GameObject hitEffect;

    public override void castSkill(Character target, float damage, bool isMirror,bool isEnemy)
    {

        Aoe effect = Instantiate(hitEffect, target.transform.position, target.transform.rotation).GetComponent<Aoe>();
        effect.setDamage(damage,isMirror,isEnemy);
        Destroy(effect.gameObject, 3f);
    }

}
