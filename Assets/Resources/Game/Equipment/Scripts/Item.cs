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

        public override void OnInstallEquip()
        {
            
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            isComponent = true;
        }
    }
}

