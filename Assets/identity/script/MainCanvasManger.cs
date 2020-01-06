using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManger : MonoBehaviour
{
    public static MainCanvasManger Instance;
    [SerializeField]
    private LobbyCanvas lobbyCanvas;
    public LobbyCanvas LobbyCanvas {
        get { return lobbyCanvas; }
    }
    [SerializeField]
    private CurrentRoomCanvas currentRoomCanvas;
    public CurrentRoomCanvas CurrentRoomCanvas
    {
        get { return currentRoomCanvas; }
    }
    private void Awake()
    {
        Instance = this;
    }
}
