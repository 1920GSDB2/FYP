using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRank
{
    private readonly string PlayerName;
    public int PlayerHP { get; private set; }
    private PlayerInfo PlayerInfo;

    public PlayerRank(string _playerName, int _playerHP, PlayerInfo _playerList, bool _isRemote)
    {
        PlayerName= _playerName;
        PlayerHP = _playerHP;
        PlayerInfo = _playerList;

        PlayerInfo.TotalHP = _playerHP;
        PlayerInfo.PlayerName.text = _playerName;
        PlayerInfo.PlayerType = _isRemote ? PlayerType.RemotePlayer : PlayerType.LocalPlayer;
    }


    public void DeductHP(int _value)
    {
        PlayerHP -= _value;
        PlayerInfo.CurrHp = PlayerHP;
    }
}
