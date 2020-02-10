using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExportDebugging : MonoBehaviour
{
    public Text IdText;
    public Text ArenaIndex;
    public Text PlayerPositionText;
    public Text LogMessage;
    TFT.GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = TFT.GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            IdText.text = gameManager.playerId.ToString();

            for(int i =0; i< gameManager.PlayerArenas.Length; i++)
            {
                if(gameManager.PlayerArenas[i] == gameManager.SelfPlayerArena.gameObject)
                {
                    ArenaIndex.text = i.ToString();
                    break;
                }
            }

            string playerPositionText = "";
            for(int i = 0; i< gameManager.playerPosition.Length; i++)
            {
                if (i == 0) playerPositionText += "[";
                playerPositionText += gameManager.playerPosition[i];
                if (i != gameManager.playerPosition.Length - 1)
                    playerPositionText += ", ";
                else playerPositionText += "]";
            }
            PlayerPositionText.text = playerPositionText;
        }
    }
}
