using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TFT
{

    public class BuffList : MonoBehaviour
    {
        public static BuffList Instance;

        public HeroBuffList HeroBuffList;
        public Transform buffList;
        public Buff buff;

        public TextMeshProUGUI ButtonText;
        public float delayTime = 0.2f;

        public bool isShowList = true;
        private Vector2 currPos, nextPos;
        private RectTransform RectTransform, BuffListTranForm;
        private BuffersManager BuffersManager;

        private void Awake()
        {
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            RectTransform = GetComponent<RectTransform>();
            currPos = RectTransform.anchoredPosition;
            nextPos = RectTransform.anchoredPosition;
            BuffersManager = BuffersManager.Instance;
            BuffListTranForm = buffList.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            RectTransform.anchoredPosition = Vector2.Lerp(currPos, nextPos, delayTime);
            currPos = RectTransform.anchoredPosition;
            BuffHandler();
        }

        private void BuffHandler()
        {
            //HeroBuffList = NetworkManager.Instance.PlayerHeroes[NetworkManager.Instance.FocusPlayerId].BuffList;
            CheckAddBuff();
            CheckRemoveBuff();

            float buffListWidth = ((int)(buffList.transform.childCount / 4) + 1) * 30;
            if (buffList.transform.childCount == 0) buffListWidth = 0;
            BuffListTranForm.sizeDelta = new Vector2(buffListWidth, 155);

        }

        private void CheckAddBuff()
        {
            foreach (KeyValuePair<HeroClass, int> ClassValue in HeroBuffList.ClassValue)
            {
                HeroClass heroClass = ClassValue.Key;
                //Debug.Log(heroClass);
                if (!buffList.Find(heroClass.ToString()))
                {
                    foreach (Buffers buffers in BuffersManager.buffers)
                    {
                        if (buffers.heroClass == heroClass)
                        {
                            AddBuff(buffers, heroClass.ToString());
                        }
                    }
                }
                else if (buffList.Find(heroClass.ToString()).GetComponent<Buff>().CurrentValue < ClassValue.Value)
                {
                    foreach (Buffers buffers in BuffersManager.buffers)
                    {
                        if (buffers.heroClass == heroClass)
                        {
                            UpgradeBuff(buffers, heroClass.ToString());

                        }
                    }
                }
            }

            foreach (KeyValuePair<HeroRace, int> RaceValue in HeroBuffList.RaceValue)
            {
                HeroRace heroRace = RaceValue.Key;
                if (!buffList.Find(heroRace.ToString()))
                {
                    foreach (Buffers buffers in BuffersManager.buffers)
                    {
                        if (buffers.heroRace == heroRace)
                        {
                            AddBuff(buffers, heroRace.ToString());
                        }
                    }
                }
                else if (buffList.Find(heroRace.ToString()).GetComponent<Buff>().CurrentValue < RaceValue.Value)
                {
                    foreach (Buffers buffers in BuffersManager.buffers)
                    {
                        if (buffers.heroRace == heroRace)
                        {
                            UpgradeBuff(buffers, heroRace.ToString());
                        }
                    }
                }
            }

        }

        private void CheckRemoveBuff()
        {
            foreach(Transform buffTransform in buffList)
            {
                Buff buff = buffTransform.GetComponent<Buff>();
                if (HeroBuffList.BuffsName.Contains(buffTransform.name))
                {
                    //The buff is Class
                    if(buff.HeroClass != HeroClass.None)
                    {
                        buff.CurrentValue = HeroBuffList.ClassValue[buff.HeroClass];
                        foreach (Buffers buffers in BuffersManager.buffers)
                        {
                            if (buffers.heroClass == buff.HeroClass)
                            {
                                CheckBuffUpdate(buff, buffers);
                            }
                        }
                    }
                    //The buff is Race
                    else if (buff.HeroRace != HeroRace.None)
                    {
                        buff.CurrentValue = HeroBuffList.RaceValue[buff.HeroRace];
                        foreach (Buffers buffers in BuffersManager.buffers)
                        {
                            if (buffers.heroRace == buff.HeroRace)
                            {
                                CheckBuffUpdate(buff, buffers);
                            }
                        }
                    }
                }
                else
                {
                    Destroy(buffTransform.gameObject);
                }
            }
        }

        public void ClearBuff()
        {
            foreach (Transform child in buffList)
            {
                Destroy(child.gameObject);
            }
        }

        public void AddBuff(Buffers buffer, string name)
        {
            Buff newBuff = (Instantiate(buff.gameObject) as GameObject).GetComponent<Buff>();
            newBuff.HeroClass = buffer.heroClass;
            newBuff.HeroRace = buffer.heroRace;
            newBuff.name = name;
            newBuff.TotalValue = buffer.bronzeNumber;

            CheckBuffUpdate(newBuff, buffer);

            newBuff.gameObject.transform.SetParent(buffList);
            newBuff.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;

            if (!isShowList)
            {
                RectTransform.anchoredPosition = new Vector2(-RectTransform.sizeDelta.x + 15 , -81);
            }

            currPos = RectTransform.anchoredPosition;
            nextPos = RectTransform.anchoredPosition;
        }

        public void UpgradeBuff(Buffers buffer, string buffeNamer)
        {
            Transform upgradeObject = buffList.Find(buffeNamer);
            if (upgradeObject != null)
            {
                Buff buff = upgradeObject.gameObject.GetComponent<Buff>();
                buff.CurrentValue++;

                CheckBuffUpdate(buff, buffer);

            }
        }

        private void CheckBuffUpdate(Buff buff, Buffers buffer)
        {
            if (buff.CurrentValue >= buffer.goldenNumber)
            {
                buff.BuffLevel = BuffLevel.Golden;
                buff.TotalValue = buffer.totalNumber;
            }
            else if (buff.CurrentValue >= buffer.sliverNumber)
            {
                buff.BuffLevel = BuffLevel.Sliver;
                buff.TotalValue = buffer.goldenNumber;
            }
            else if (buff.CurrentValue >= buffer.bronzeNumber)
            {
                buff.BuffLevel = BuffLevel.Bronze;
                buff.TotalValue = buffer.sliverNumber;
            }
            else
            {
                buff.TotalValue = buffer.bronzeNumber;
            }
        }
        public void SwitchPanel()
        {
            if (RectTransform.sizeDelta.x <= 15) return;
            isShowList = !isShowList;
            if (isShowList)
            {
                nextPos.x = 0;
                ButtonText.text = "<";
            }
            else
            {
                nextPos.x = -RectTransform.sizeDelta.x + 15;
                ButtonText.text = ">";
            }
        }
    }

}