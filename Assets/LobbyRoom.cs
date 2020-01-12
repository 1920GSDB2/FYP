using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LobbyRoom : MonoBehaviour
{
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI Owner;
    public TextMeshProUGUI GameMode;
    public TextMeshProUGUI PlayerNumber;
    Button EnterRoomButton;

    // Start is called before the first frame update
    void Start()
    {
        EnterRoomButton = GetComponent<Button>();
        EnterRoomButton.onClick.AddListener(delegate { EnterRoom(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterRoom()
    {

    }
}
