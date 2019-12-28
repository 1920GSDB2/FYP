using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffList : MonoBehaviour
{
    public Transform buffList;
    public Buff buff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBuff(Buffers buffer, string name)
    {
        Buff newBuff = (Instantiate(buff.gameObject) as GameObject).GetComponent<Buff>();
        newBuff.HeroClass = buffer.heroClass;
        newBuff.HeroRare = buffer.heroRare;
        newBuff.TotalValue = buffer.totalNumber;
        newBuff.name = name;
        //newBuff.gameObject.transform.parent = buffList;
        newBuff.gameObject.transform.SetParent(buffList);
        newBuff.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        buffList.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 24 * buffList.gameObject.transform.childCount);
    }
    public void UpgradeBuff(string buffer)
    {
        Transform upgradeObject = buffList.Find(buffer);
        if (upgradeObject != null)
            upgradeObject.gameObject.GetComponent<Buff>().CurrentValue++;
    }
}
