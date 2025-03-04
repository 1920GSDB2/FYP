﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFT
{
    [System.Serializable]
    public class PlayerRank
    {
        private readonly string PlayerName;
        private readonly string PlayerId;
        public int PlayerHP { get; private set; }
        private PlayerInfo PlayerInfo;

        public PlayerRank(string _playerName, string _playerId, int _playerHP, PlayerInfo _playerList, bool _isRemote)
        {
            PlayerName = _playerName;
            PlayerId = _playerId;
            PlayerHP = _playerHP;
            PlayerInfo = _playerList;

            PlayerInfo.TotalHP = _playerHP;
            PlayerInfo.PlayerName.text = _playerName;
            PlayerInfo.PlayerType = _isRemote ? PlayerType.RemotePlayer : PlayerType.LocalPlayer;
        }


        public void DeductHP(int _value)
        {
            PlayerHP -= _value;
            if (PlayerHP <= 0) {
                PlayerHP = 0;
                //Debug.Log("player name " + NetworkManager.Instance.PlayersId[NetworkManager.Instance.playerId] + " deduce name " + PlayerName);
                if (PlayerId == NetworkManager.Instance.PlayersId[NetworkManager.Instance.playerId]) {
                    NetworkManager.Instance.playerDie();
                }
            } 
            PlayerInfo.CurrHp = PlayerHP;
        }
    }
}

