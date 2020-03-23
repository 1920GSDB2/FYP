using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    public class BlendItem : Equipment
    {
        public Item[] components;
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            isComponent = false;
        }

    }

}
