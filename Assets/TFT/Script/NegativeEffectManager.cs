using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeEffectManager : MonoBehaviour
{
    IEnumerator SlientIEnumerator;
    IEnumerator StunIEnumerator;
    IEnumerator KnockIEnumerator;
    IEnumerator BurnIEnumerator;
    IEnumerator BlindIEnumerator;

    float up;

    Character Character;

    private void Awake()
    {
        Character = GetComponent<Character>();
    }

    //Cannot attack
    public void Slient(float _time)
    {
        if (SlientIEnumerator != null)
        {
            StopCoroutine(SlientIEnumerator);
        }
        SlientIEnumerator = SlientExcue(_time);
        StartCoroutine(SlientIEnumerator);
    }

    //Stun
    public void Stun(float _time)
    {
        if (StunIEnumerator != null)
        {
            StopCoroutine(StunIEnumerator);
        }
        StunIEnumerator = StunExcue(_time);
        StartCoroutine(StunIEnumerator);

    }

    public void Knock(float _time)
    {
        if (KnockIEnumerator != null)
        {
            StopCoroutine(KnockIEnumerator);
        }
        KnockIEnumerator = KnockExcue(_time);
        StartCoroutine(KnockIEnumerator);
    }

    public void Burn(float _time)
    {
        if (BurnIEnumerator != null)
        {
            StopCoroutine(BurnIEnumerator);
        }
        BurnIEnumerator = BurnExcue(_time);
        StartCoroutine(BurnIEnumerator);
    }

    public void Blind(float _time)
    {
        if(BlindIEnumerator != null)
        {
            StopCoroutine(BlindIEnumerator);
        }
        BlindIEnumerator = BlindExcue(_time);
        StartCoroutine(BlindIEnumerator);
    }


    public IEnumerator SlientExcue(float _time)
    {
        Character.isSlience = true;
        yield return new WaitForSeconds(_time);
        Character.isSlience = false;
        SlientIEnumerator = null;
    }

    public IEnumerator StunExcue(float _time)
    {
        
        Character.isSlience = true;
        Character.isBlind = true;
        Character.isStun = true;
        yield return new WaitForSeconds(_time);
        Character.isSlience = false;
        Character.isBlind = false;
        Character.isStun = false;
        StunIEnumerator = null;
    }

    public IEnumerator KnockExcue(float _time)
    {
        float countTime = 0;
        //float knockSpeed = 1/(_time / Time.deltaTime / 2) + _time / 2
        Character.isSlience = true;
        Character.isBlind = true;
        Character.isStun = true;
        while (transform.position.y <= 1)
        {
            countTime += Time.deltaTime;
            transform.position += transform.up * 1 * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(_time - countTime * 2);
        countTime += _time - countTime * 2;
        while (transform.position.y > 0)
        {
            countTime += Time.deltaTime;
            transform.position -= transform.up * 1 * Time.deltaTime;
            yield return null;
        }
      //  Debug.Log(countTime);
        Character.isSlience = false;
        Character.isBlind = false;
        Character.isStun = false;
        KnockIEnumerator = null;
    }

    public IEnumerator BurnExcue(float _time)
    {
        int count = (int)(_time / 0.5f);
        for(int i = 0; i < count; i++)
        {
            //Deduct 2% HP
            Character.TakePercentageDamage(2);
            yield return new WaitForSeconds(0.5f);
        }
        BurnIEnumerator = null;
    }

    public IEnumerator BlindExcue(float _time)
    {
        Character.isBlind = true;
        yield return new WaitForSeconds(_time);
        Character.isBlind = false;
        BlindIEnumerator = null;
    }
}
