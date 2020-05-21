
using System.IO;
using UnityEngine;

public class ShootUnitTargetSkill : Skill
{
     public GameObject skillModel;

   // public string skillPrefabName;
     public override void shootSkill(Character target, float damage, bool isMirror){

       //  Bullet bullet = (PhotonNetwork.Instantiate(Path.Combine("Skill", skillPrefabName), transform.position, transform.rotation, 0)).GetComponent<Bullet>();
       // bullet.setBullet(target, damage, isMirror);
         Bullet bullet = Instantiate(skillModel, transform.position, transform.rotation).GetComponent<Bullet>();
   //     Debug.Log("Set Bullet " + damage);
         bullet.setBullet(target, damage, isMirror);
        // bullet.setBullet(target,damage,)
     }
   

}
