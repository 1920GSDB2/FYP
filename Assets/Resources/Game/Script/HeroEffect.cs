
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class NegativeEffect {
    public bool canAction;
    public ControlSkillType type;
    public bool isHandle;
    public NegativeEffect(ControlSkillType type, bool canAction) {
        this.type = type;
        this.canAction = canAction;
    }
    
}
