using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public enum FunctionPanelType
    {
        StartPanel,
        LobbyPanel,
        InformationPanel
    }
    public enum GameModePanelType
    {
        MatchModePanel,
        CreateCustomPanel,
        JoinCustomPanel
    }
    [Header("Function Buttons")]
    public Button StartButton;
    public Button LobbyButton;
    public Button InformationButton;

    [Header("Game Mode Buttons")]
    public Button MatchButton;
    public Button CreateCustomButton;
    public Button JoinCustomButton;

    [Header("Game Start Buttons")]
    public Button OkayButton;
    public Button BackLobbyButton;

    [Header("Function Panels")]
    public GameObject StartPanel;
    public GameObject LobbyPanel;
    public GameObject InformationPanel;
    GameObject[] FunctionPanelsArr = new GameObject[3];

    [Header("Game Mode Panels")]
    public GameObject MatchModePanel;
    public GameObject CreateCustomPanel;
    public GameObject JoinCustomPanel;
    GameObject[] GameModePanelsArr = new GameObject[3];

    GameModePanelType currentModeType = GameModePanelType.MatchModePanel;
    // Start is called before the first frame update
    void Start()
    {
        //Function Panels Manage
        FunctionPanelsArr[0] = StartPanel;
        FunctionPanelsArr[1] = LobbyPanel;
        FunctionPanelsArr[2] = InformationPanel;
        //Game Mode Panels Manage
        GameModePanelsArr[0] = MatchModePanel;
        GameModePanelsArr[1] = CreateCustomPanel;
        GameModePanelsArr[2] = JoinCustomPanel;

        //Add Listener of Buttons
        StartButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.StartPanel); SwitchGameModePanel(GameModePanelType.MatchModePanel); });
        LobbyButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.LobbyPanel); });
        InformationButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.InformationPanel); });
        MatchButton.onClick.AddListener(delegate { SwitchGameModePanel(GameModePanelType.MatchModePanel); });
        CreateCustomButton.onClick.AddListener(delegate { SwitchGameModePanel(GameModePanelType.CreateCustomPanel); });
        JoinCustomButton.onClick.AddListener(delegate { SwitchGameModePanel(GameModePanelType.JoinCustomPanel); });
        OkayButton.onClick.AddListener(delegate { StartGame(); });
        BackLobbyButton.onClick.AddListener(delegate { SwitchFunctionPanel(FunctionPanelType.LobbyPanel); });

        SwitchFunctionPanel(FunctionPanelType.LobbyPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchFunctionPanel(FunctionPanelType _type)
    {
        foreach (GameObject panel in FunctionPanelsArr)
        {
            panel.SetActive(false);
        }
        switch (_type)
        {
            case FunctionPanelType.StartPanel:
                FunctionPanelsArr[0].SetActive(true);
                break;
            case FunctionPanelType.LobbyPanel:
                FunctionPanelsArr[1].SetActive(true);
                break;
            case FunctionPanelType.InformationPanel:
                FunctionPanelsArr[2].SetActive(true);
                break;
        }
    }

    public void SwitchGameModePanel(GameModePanelType _type)
    {
        currentModeType = _type;
        foreach(GameObject panel in GameModePanelsArr)
        {
            panel.SetActive(false);
        }
        switch (_type)
        {
            case GameModePanelType.MatchModePanel:
                GameModePanelsArr[0].SetActive(true);
                break;
            case GameModePanelType.CreateCustomPanel:
                GameModePanelsArr[1].SetActive(true);
                break;
            case GameModePanelType.JoinCustomPanel:
                GameModePanelsArr[2].SetActive(true);
                break;
        }
    }

    public void StartGame()
    {
        switch (currentModeType)
        {
            case GameModePanelType.MatchModePanel:
                //...//
                break;
            case GameModePanelType.CreateCustomPanel:
                //...//
                break;
            case GameModePanelType.JoinCustomPanel:
                //...s//
                break;
        }
    }
}
