using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Collection : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image icon;
  
    public void setCollection(Sprite icon,string name) {
        nameText.text = name;
        this.icon.sprite = icon;
    }
}
