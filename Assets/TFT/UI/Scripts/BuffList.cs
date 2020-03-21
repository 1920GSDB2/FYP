using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffList : MonoBehaviour
{
    public Transform buffList;
    public Buff buff;
    public static BuffList Instance;

    public TextMeshProUGUI ButtonText;
    public float delayTime = 0.2f;

    private bool isShowList;
    private Vector2 currPos, nextPos;
    private RectTransform RectTransform;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        RectTransform = GetComponent<RectTransform>();
        currPos = RectTransform.anchoredPosition;
        nextPos = RectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform.anchoredPosition = Vector2.Lerp(currPos, nextPos, delayTime);
        currPos = RectTransform.anchoredPosition;
    }

    public void ClearBuff()
    {
        foreach(Transform child in buffList)
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
