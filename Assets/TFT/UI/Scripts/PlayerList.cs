using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    LocalPlayer,
    RemotePlayer
}

public class PlayerList : MonoBehaviour
{
    public Image playerIcon;
    public TextMeshProUGUI playerName;
    public PlayerHP playerHP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
