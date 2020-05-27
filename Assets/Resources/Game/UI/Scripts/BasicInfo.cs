using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicInfo : MonoBehaviour
{
    public Text nameUI;
    public Text priceUI;
    public Image bgColor;
    public string name;
    public Rarity rarity;
    public int price;
    // Start is called before the first frame update
    void Start()
    {
        switch (rarity)
        {
            case Rarity.Common:
                price = 1;
                bgColor.color = new Color(0.07450981f, 0.145098f, 0.1921569f);
                break;
            case Rarity.Uncommon:
                price = 2;
                bgColor.color = new Color(0.05882353f, 0.3254902f, 0.145098f);
                break;
            case Rarity.Rare:
                price = 3;
                bgColor.color = new Color(0.1372549f, 0.2392157f, 0.7921569f);
                break;

            case Rarity.Epic:
                price = 4;
                bgColor.color = new Color(0.4117647f, 0.08235294f, 0.3137255f);
                break;

            case Rarity.Legendary:
                price = 5;
                bgColor.color = new Color(1, 0.533f, 0);
                break;
        }
        priceUI.text = price.ToString();
        nameUI.text = name;
    }

    private void Update()
    {
        switch (rarity)
        {
            case Rarity.Common:
                price = 1;
                bgColor.color = new Color(0.07450981f, 0.145098f, 0.1921569f);
                break;
            case Rarity.Uncommon:
                price = 2;
                bgColor.color = new Color(0.05882353f, 0.3254902f, 0.145098f);
                break;
            case Rarity.Rare:
                price = 3;
                bgColor.color = new Color(0.1372549f, 0.2392157f, 0.7921569f);
                break;

            case Rarity.Epic:
                price = 4;
                bgColor.color = new Color(0.4117647f, 0.08235294f, 0.3137255f);
                break;

            case Rarity.Legendary:
                price = 5;
                bgColor.color = new Color(1, 0.533f, 0);
                break;
        }
        priceUI.text = price.ToString();
        nameUI.text = name;
    }
}
