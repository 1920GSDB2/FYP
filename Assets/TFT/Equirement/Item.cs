using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Equirement
{
    public ItemType ItemType;
    // Start is called before the first frame update
    public override void Start()
    {
        isComponent = true;
    }
    
}
