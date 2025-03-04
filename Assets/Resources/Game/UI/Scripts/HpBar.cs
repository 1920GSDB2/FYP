﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{

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
            if (damageColor.a > 0)
            {
                damageColor.a -= 0.5f * Time.deltaTime;
                damageBar.color = damageColor;
            }
            //Vector3 targetPostition = 
            //    new Vector3(transform.position.x,
            //    NetworkManager.Instance.CurrentCamera.transform.position.y, 
            //    transform.position.x);
            //transform.LookAt(targetPostition);
            //transform.LookAt(NetworkManager.Instance.CurrentCamera.transform);
            Camera camera = NetworkManager.Instance.CurrentCamera;
            //Vector3 targetPostition = new Vector3(transform.position.x, -camera.transform.position.y, transform.position.z);
            //transform.LookAt(targetPostition);
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.back,
                camera.transform.rotation * Vector3.up);


        }
        public void setHpBarWithDamage(float precentage)
        {
            damageBar.fillAmount = hpBar.fillAmount;
            damageColor.a = 1f;
            setHpBar(precentage);
        }
        public void setHpBar(float precentage)
        {

            hpBar.fillAmount = precentage;
        }
        public void setMpBar(float precentage)
        {
            mpBar.fillAmount = precentage;
        }
        public void setShieldBar(float precentage)
        {
            shieldBar.fillAmount = precentage;
        }
        public void addShieldBar(float precentage)
        {
            shieldBar.fillAmount += precentage;
        }
        public void reduceShieldBar(float precentage)
        {
            shieldBar.fillAmount -= precentage;
        }
        public void setHpBarColor(Color color)
        {
            hpBar.color = color;
        }
        public float getShieldbarFillAmount()
        {
            return shieldBar.fillAmount;
        }
    }

}