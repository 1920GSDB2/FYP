using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Opponent
{
    public int opponentId;
    public bool isShadow;
    public List<Character> heroes = new List<Character>() ;
    public Opponent(int _opponentId, bool? _isShadow = null)
    {
        opponentId = _opponentId;
        isShadow = _isShadow ?? false;
    }
}
