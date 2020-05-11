using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public GameObject castSkillEffect;
    public virtual void castSkill() { }
    public virtual void shootSkill(Character target,float damage,bool isMirror) { }
    public virtual void shootSkill(float x, float z) { }
}

