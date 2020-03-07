using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OpponentManager
{
    public Opponent host;
    public Opponent guest;
    
    public OpponentManager(int _hostId, int _guestId, bool? _isShadow = null)
    {
        host = new Opponent(_hostId);
        guest = new Opponent(_guestId, _isShadow ?? false);
    }

}
