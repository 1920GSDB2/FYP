﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettingManager : MonoBehaviour
{
    public static SoundSettingManager Instance;
    public Slider gameSoundSlider;
    public Slider bgSoundSlider;
    public Slider uiSoundSlider;
    public TextMeshProUGUI gameSoundText, bgSoundText, uiSoundText;
    public Button settingMenuBtn, applyBtn, okBtn;
    private float backGroundSound;
    public float BackGroundSound {
        get{ return backGroundSound; }
        set {
            backGroundSound = value;
            bgSound.volume = backGroundSound;
            Debug.Log(bgSound.volume+"suound ");
        }

    }
    public float GameSound = 1f;
    public float UiSound = 1f;
    public GameObject Setting;
    bool isOpen;
    public AudioSource bgSound;

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {

        bgSound = GetComponent<AudioSource>();
        gameSoundSlider.onValueChanged.AddListener(delegate { OnGameSoundChange(); });
        bgSoundSlider.onValueChanged.AddListener(delegate { OnBackGroundSoundChange(); });
        uiSoundSlider.onValueChanged.AddListener(delegate { OnUiSoundChange(); });
        settingMenuBtn.onClick.AddListener(openSettingMenu);
        applyBtn.onClick.AddListener(applySetting);
        okBtn.onClick.AddListener(closeSettingMenu);
       
    }
    
    public void OnGameSoundChange()
    {
        gameSoundText.text = ((int)(gameSoundSlider.value * 100)).ToString();
    }
    public void OnBackGroundSoundChange()
    {
        bgSoundText.text = ((int)(bgSoundSlider.value * 100)).ToString();
    }

    public void OnUiSoundChange()
    {
        uiSoundText.text = ((int)(uiSoundSlider.value * 100)).ToString();
    }
    public void openSettingMenu()
    {
        isOpen = !isOpen;
        gameSoundSlider.value = GameSound;
        bgSoundSlider.value = BackGroundSound;
        uiSoundSlider.value = UiSound;
        Setting.SetActive(isOpen);
    }
    public void closeSettingMenu()
    {
        Setting.SetActive(false);
    }
    public void applySetting()
    {
        UiSound = uiSoundSlider.value;
        BackGroundSound = bgSoundSlider.value;
        GameSound = gameSoundSlider.value;
    }
}
