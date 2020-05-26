using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChannelChat : MonoBehaviour
{
    public TMP_InputField InputBox;
    public Image DisplayBackgroud;
    public Button SendButon;


    public float slerpTime = 0.05f;
    private float currentValue, nextValue;

    // Start is called before the first frame update
    void Start()
    {
        SendButon.onClick.AddListener(delegate { ChatManager.Instance.OnClickSend(); });

    }

    // Update is called once per frame
    void Update()
    {
        float value = Mathf.Lerp(currentValue, nextValue, slerpTime);
        DisplayBackgroud.color = new Color(DisplayBackgroud.color.r, DisplayBackgroud.color.g, DisplayBackgroud.color.b, value);
        currentValue = DisplayBackgroud.color.a;
    }

    public void InputBoxSelected()
    {
        nextValue = 0.5f;
    }
    public void OnEnter()
    {
        nextValue = 0.5f;

    }
    public void OnExit()
    {
        nextValue = 0;

    }
}
