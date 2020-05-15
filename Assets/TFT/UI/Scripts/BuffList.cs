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

        private bool isShowList;
        private Vector2 currPos, nextPos;
        private RectTransform RectTransform;
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
            HeroBuffList = NetworkManager.Instance.PlayerHeroes[NetworkManager.Instance.FocusPlayerId].BuffList;
            foreach (KeyValuePair<HeroClass, int> ClassValue in HeroBuffList.ClassValue)
            {
                HeroClass heroClass = ClassValue.Key;
                Debug.Log(heroClass);

                if (!buffList.Find(heroClass.ToString()))
                {
                    foreach (Buffers buffers in BuffersManager.buffersClass)
                    {
                        if (buffers.heroClass == heroClass)
                        {
                            AddBuff(buffers, heroClass.ToString());
                        }
                    }
                }
                else if(buffList.Find(heroClass.ToString()).GetComponent<Buff>().CurrentValue < ClassValue.Value)
                {
                    UpgradeBuff(heroClass.ToString());
                }
            }
        }

        public void ClearBuff()
        {
            foreach (Transform child in buffList)
            {
                Destroy(child.gameObject);
            }
            buffList.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 0);
        }

        public void AddBuff(Buffers buffer, string name)
        {
            //buffList.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 0);
            Buff newBuff = (Instantiate(buff.gameObject) as GameObject).GetComponent<Buff>();
            newBuff.HeroClass = buffer.heroClass;
            newBuff.HeroRare = buffer.heroRare;
            newBuff.TotalValue = buffer.totalNumber;
            newBuff.name = name;
            //newBuff.gameObject.transform.parent = buffList;
            newBuff.gameObject.transform.SetParent(buffList);
            //newBuff.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            //buffList.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 24 * buffList.gameObject.transform.childCount);
            if (!isShowList)
            {
                RectTransform.anchoredPosition = new Vector2(-RectTransform.sizeDelta.x + 10, -115);
            }

            currPos = RectTransform.anchoredPosition;
            nextPos = RectTransform.anchoredPosition;
        }

        public void UpgradeBuff(string buffer)
        {
            Transform upgradeObject = buffList.Find(buffer);
            if (upgradeObject != null)
                upgradeObject.gameObject.GetComponent<Buff>().CurrentValue++;
        }

        public void SwitchPanel()
        {
            if (RectTransform.sizeDelta.x <= 10) return;
            isShowList = !isShowList;
            if (isShowList)
            {
                nextPos.x = 0;
                ButtonText.text = "<";
            }
            else
            {
                nextPos.x = -RectTransform.sizeDelta.x + 10;
                ButtonText.text = ">";
            }
        }
    }

}