using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBar;
    public Image mpBar;
    public Image damageBar;
    public Image shieldBar;
    private Color damageColor;
    // Start is called before the first frame update
    void Start()
    {
        damageColor = damageBar.color;
    }
    // Update is called once per frame
    void Update()
    {
        if (damageColor.a > 0) {
            damageColor.a -= 1 * Time.deltaTime;
            damageBar.color = damageColor;
        }
      
        if (Input.GetKeyDown(KeyCode.I))
        {
            float a = hpBar.fillAmount;
            a -= 0.2f;
            setHpBarWithDamage(a/1);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            addShieldBar(0.2f);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            reduceShieldBar(0.2f);
        }
    }
    public void setHpBarWithDamage(float precentage) {
        damageBar.fillAmount = hpBar.fillAmount;
        damageColor.a = 1f;
        setHpBar(precentage);
    }
    public void setHpBar(float precentage) {
     
        hpBar.fillAmount = precentage;
    }
    public void setMpBar(float precentage)
    {
        mpBar.fillAmount = precentage;
    }
    public void addShieldBar(float precentage)
    {
        shieldBar.fillAmount += precentage;
    }
    public void reduceShieldBar(float precentage) {
        shieldBar.fillAmount -= precentage;
    }
    public void setHpBarColor(Color color)
    {
       hpBar.color = color;
    }
}
