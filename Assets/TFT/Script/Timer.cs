using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TFTGameManager TFTGameManager;
    // Start is called before the first frame update
    void Start()
    {
        TFTGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TFTGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
