
using System.IO;
using UnityEngine;

public class UnitTargetSkill : Skill
{
     public GameObject skillModel;
   // public string skillPrefabName;
     public override void shootSkill(Character target, float damage, bool isMirror){

       //  Bullet bullet = (PhotonNetwork.Instantiate(Path.Combine("Skill", skillPrefabName), transform.position, transform.rotation, 0)).GetComponent<Bullet>();
       // bullet.setBullet(target, damage, isMirror);
         Bullet bullet = Instantiate(skillModel, transform.position, transform.rotation).GetComponent<Bullet>();
         bullet.setBullet(target, damage, isMirror);
        // bullet.setBullet(target,damage,)
     }
    public override void shootSkill(float x, float z)
     {
        Bullet bullet = Instantiate(skillModel, transform.position, transform.rotation).GetComponent<Bullet>();
        Vector3 targetPos = new Vector3(x, bullet.transform.position.y, z);
        bullet.setBullet(targetPos);
    }

}
