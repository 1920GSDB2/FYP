using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TFT;

[Serializable]
public class Monster : Character
{   
   // public HeroPlace spawnHeroPlace;
    //int position;
    private void Start()
    {
      //  int position = spawnHeroPlace.PlaceId;
    }
    public override void die()
    {

        base.die();
    }


}
