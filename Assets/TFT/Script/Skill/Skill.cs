using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TFT;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public GameObject castSkillEffect;
    public virtual void castSkill(Character target,float damage,bool isMirror,bool isEnemy) { }
    public virtual void castSkill(float x,float y,float z) { }
    public virtual void shootSkill(Character target,float damage,bool isMirror) { }
    public virtual void shootSkill(float x, float z) { }
    public virtual void meleeHit(Character target,float damage,bool isMirror) { }
    public virtual void summon(HeroPlace heroPlace, bool isMirror) { }
}

