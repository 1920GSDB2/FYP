using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamFlag : MonoBehaviour
{
    public TextMeshProUGUI GameboardCard, Level;
    public static TeamFlag Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        GameboardCard.text = "0";
        Level.text = "1";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
