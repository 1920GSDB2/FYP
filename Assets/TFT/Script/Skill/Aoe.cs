using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aoe : MonoBehaviour
{
    
    public float delayOpenCollider=0;
    public Collider collider;
    protected float damage;
    public float finishTime;
    protected bool isMirror,isAlly;
    void Start()
    {
        collider = GetComponent<Collider>();
        StartCoroutine(openCollider());
        Destroy(this.gameObject, finishTime);
    }
    public void setDamage(float damage,bool isMirror,bool isEnemy){
        this.damage = damage;
        this.isMirror = isMirror;
        this.isAlly = isEnemy;
    }
    IEnumerator openCollider() {
        yield return new WaitForSeconds(delayOpenCollider);
        collider.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isMirror)
        {
            if (other.tag == "BattleCharacter")
            {
              
                bool target = other.GetComponent<Character>().isEnemy;
                Debug.Log("Collider " + other.name +" isEnemy "+target+" isAlly "+isAlly);
                if (target!=isAlly)
                {
                    other.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, damage,(byte)DamageType.Magic);
                }
            }
        }
       
        //  target.GetComponent<PhotonView>().RPC("RPC_TargetTakeDamage", PhotonTargets.All, attackDamage, (byte)type, duration);
    }
  
}
