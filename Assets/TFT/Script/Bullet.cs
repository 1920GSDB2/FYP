using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Character target;
    public float speed=3;
    float attackDamage;
    bool canDamage;
    // Start is called before the first frame update
 

    // Update is called once per frame
    void Update()
    {
        if(target!=null)
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position,  speed* Time.deltaTime);
    }
    public void setBullet(Character c, float damage,bool isDamage) {
        target = c;
        attackDamage = damage;
        canDamage = isDamage;
    }
    private void OnCollisionEnter(Collision collision)
    {       
            if (collision.gameObject == target.gameObject)
            {
                if(canDamage)
                target.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage);
                PhotonNetwork.Destroy(this.gameObject);
            }     
    }
}
