using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class BlendItem : Equipment
    {
        public Item[] components;

        public override void OnInstallEquip()
        {
            
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            isComponent = false;
        }

    }

}
