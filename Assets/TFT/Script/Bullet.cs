using System.Collections;
using System.Collections.Generic;
using TFT;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target;
    public float speed=30f;
    protected float attackDamage;
    protected bool canDamage;
    public GameObject hitEffect;
    public Vector3 targetPos;
    DamageType damageType=DamageType.Physical;
    // Start is called before the first frame update


    // Update is called once per frame
    private void Start()
    {
        float dis = 0;
        if (target != null) { 
            dis = Vector3.Distance(transform.position, target.transform.position + Vector3.up);

            float time = dis / (speed * Time.deltaTime * 60);
            //Debug.Log("bullet exist TIMe"+time);
            Destroy(this.gameObject, time);
       }
    }
    void Update()
    {
        if(target!=null)
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position+Vector3.up,  speed*Time.deltaTime);
        else if(targetPos!=null)
            transform.position = Vector3.MoveTowards(transform.position,targetPos+Vector3.up, speed * Time.deltaTime);
    }
    public void setBullet(Character c, float damage,bool isDamage,DamageType type) {
        target = c.gameObject;
        attackDamage = damage;
        canDamage = isDamage;
        damageType  = type;
    }
    public void setBullet(Character c, float damage, bool isDamage, int cirtical)
    {
        target = c.gameObject;
        attackDamage = damage;
        canDamage = isDamage;
    }
    public void setBullet(Character c, float damage, bool isDamage)
    {
        target = c.gameObject;
        attackDamage = damage;
        canDamage = isDamage;
    }
    public void setBullet(GameObject c, float speed)
    {
        target = c;
        this.speed = speed;
        canDamage = false;
    }
    public void setBullet(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        canDamage = false;
        float dis = Vector3.Distance(transform.position, targetPos);
        float time = dis / (speed * Time.deltaTime * 60);
        Destroy(this.gameObject, time);
       
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
            {
                target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage, (byte)damageType);
            }

            Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        if (hitEffect != null)
        {
            if (target != null)
            {
                GameObject effect = Instantiate(hitEffect, target.transform.position, Quaternion.identity) as GameObject;
                Destroy(effect, 2f);
            }
            else {
                GameObject effect = Instantiate(hitEffect,transform.position, Quaternion.identity) as GameObject;
                Destroy(effect, 2f);
            }
        }
       // Debug.Log("skill destory");
    }
}
