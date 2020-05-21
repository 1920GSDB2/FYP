using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OpponentManager
{
    public int host;
    public int guest;
    public bool isShadow;
    
    public OpponentManager(int _hostId, int _guestId, bool? _isShadow = null)
    {
        host = _hostId;
        guest = _guestId;
        isShadow = _isShadow??false;
    }

}
