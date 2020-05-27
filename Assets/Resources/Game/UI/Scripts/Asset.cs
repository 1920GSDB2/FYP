using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Asset : MonoBehaviour
{
    [SerializeField]
    [Range(0, 999)]
    private int assetValue;
    public int AssetValue
    {
        get { return assetValue; }
        set
        {
            assetValue = value;
            AssetText.text = value.ToString();
        }
    }
    public TextMeshProUGUI AssetText;

}
