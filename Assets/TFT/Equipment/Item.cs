using System;
using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine;

namespace TFT
{
    public class Item : Equipment
    {
        public ItemType ItemType;
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            isComponent = true;
        }
    }
}

