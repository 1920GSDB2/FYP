using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Character target;
    public float speed=30f;
    float attackDamage;
    bool canDamage;
    // Start is called before the first frame update


    // Update is called once per frame
    private void Start()
    {
        float dis = Vector3.Distance(transform.position, target.transform.position + Vector3.up);
        float time = dis /( speed*Time.deltaTime*60);
        //Debug.Log("bullet exist TIMe"+time);
        Destroy(this.gameObject, time);
    }
    void Update()
    {
        if(target!=null)
            transform.position = Vector3.Lerp(transform.position, target.transform.position+Vector3.up,  speed*Time.deltaTime);
    }
    public void setBullet(Character c, float damage,bool isDamage) {
        target = c;
        attackDamage = damage;
        canDamage = isDamage;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
            if (collision.gameObject == target.gameObject)
            {
                if(canDamage)
                target.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage);
                Destroy(this.gameObject);
            }     
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            if (canDamage)
                target.photonView.RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage);
            Destroy(this.gameObject);
        }
    }

}
