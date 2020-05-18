using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TFT
{
    public enum PlayerType
    {
        LocalPlayer,
        RemotePlayer
    }

    public class PlayerInfo : MonoBehaviour
    {
        private int totalHp;
        public int TotalHP
        {
            get { return totalHp; }
            set
            {
                totalHp = value;
                CurrHp = totalHp;
            }
        }
        public int CurrHp;

        public Image playerIcon;
        public TextMeshProUGUI PlayerName;

        [SerializeField]
        private Image imageSlider;
        [SerializeField]
        private TextMeshProUGUI hpText;

        private Button Button;

        private PlayerType playerType;
        public PlayerType PlayerType
        {
            get { return playerType; }
            set
            {
                playerType = value;
                if (value == PlayerType.LocalPlayer)
                {
                    imageSlider.color = Color.yellow;
                }
                else
                {
                    imageSlider.color = Color.red;
                }
            }
        }

        private NetworkManager NetworkManager;

        private void Start()
        {
            NetworkManager = NetworkManager.Instance;
            Button = GetComponent<Button>();
            Button.onClick.AddListener(delegate { ButtonClick(); });
        }

        private void ButtonClick()
        {
            //Debug.Log("Button Click");
            /*for(int i = 0; i < NetworkManager.PlayersName.Length; i++)
            {
                NetworkManager.PlayerArenas[i].GetComponent<PlayerArena>().Camera.SetActive(false);
                if (NetworkManager.PlayersName[i].Equals(PlayerName.text))
                {
                    NetworkManager.PlayerArenas[i].GetComponent<PlayerArena>().Camera.SetActive(true);
                    BuffList.Instance.HeroBuffList = NetworkManager.PlayerHeroes[i].BuffList;
                }
            }*/
             NetworkManager.Instance.watchOtherPlayer(PlayerName.text);
        }

        // Update is called once per frame
        void Update()
        {
            if (CurrHp >= 0)
            {
                imageSlider.fillAmount = (float)CurrHp / TotalHP;
                hpText.text = CurrHp.ToString();
            }
        }
    }

}
