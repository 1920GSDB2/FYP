using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionStore : MonoBehaviour
{

    [Header("Character Sprite")]
    public Sprite EagleSprite;
    public Sprite EyeBatSpirte;
    public Sprite defaultSpirte;
    public static CollectionStore Instance;
    private void Start()
    {
        Instance = this;
    }
    public int GetPrice(TFTCharacter tFTCharacter) {
        switch (tFTCharacter) {
            case TFTCharacter.Eagle:    return 10;
            case TFTCharacter.EyeBat:   return 20;
            default:                    return 0;
        }
    }
    public Sprite GetSprite(TFTCharacter tFTCharacter) {
        switch (tFTCharacter)
        {
            case TFTCharacter.Eagle: return EagleSprite;
            case TFTCharacter.EyeBat: return EyeBatSpirte;
            default: return defaultSpirte;
        }
    }
    public Sprite GetSprite(string name)
    {
        switch (name)
        {
            case "Eagle": return EagleSprite;
            case "EyeBat": return EyeBatSpirte;
            default: return defaultSpirte;
        }
    }
    public string GetName(TFTCharacter tFTCharacter)
    {
        switch (tFTCharacter)
        {
            case TFTCharacter.Eagle: return "Eagle";
            case TFTCharacter.EyeBat: return "EyeBat";
            default: return null;
        }
    }
}
