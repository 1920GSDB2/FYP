using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace TFT
{
    public class GameStatusFlagContainer : MonoBehaviour
    {
        public TextMeshProUGUI Status;
        private GameManager GameManager;
        // Start is called before the first frame update
        void Start()
        {
            GameManager = GameManager.Instance;
            GameManager.statusChange += OnStatusChange;
        }

        public void OnStatusChange(object sender, EventArgs e)
        {
            Status.text = GameManager.GameStatus.ToString();
        }
    }
}
