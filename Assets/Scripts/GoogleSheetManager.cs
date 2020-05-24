using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;
using System;
using UnityEngine.Events;

public class GoogleSheetManager : MonoBehaviour
{
    public static GoogleSheetManager Instance;
    [SerializeField]
    private Main.GameManager GameManager;

    public string playerId;
    public int money;
    public Friends Friends;
    public Skins Skins;

    public string spreadsheetId;
    public string worksheetName;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Read();
        //Write();
        if (GameManager.userData.id != null && !GameManager.userData.id.Equals(""))
        {
            playerId = GameManager.userData.id;
        }
        else
        {
            playerId = "test8999";
        }
        ReadOnStart();
    }
    private void ReadOnStart()
    {
        SpreadsheetManager.Read(new GSTU_Search(spreadsheetId, worksheetName), LoadPlayerCollection);
    }

    private void LoadPlayerCollection(GstuSpreadSheet ss)
    {
        Debug.Log("Player Id: " + playerId);
        if (ss.rows.ContainsKey(playerId))
        {
            SetPlayerData(ss.rows[playerId]);

        }
        else
        {
            Friends = new Friends();
            Skins = new Skins();

            List<string> newData = new List<string>();
            newData.Add(playerId);
            newData.Add(money.ToString());
            newData.Add(JsonUtility.ToJson(Friends));
            newData.Add(JsonUtility.ToJson(Skins));
            SpreadsheetManager.Append(new GSTU_Search(spreadsheetId, worksheetName), new ValueRange(newData), null);
        }
        //UpdateStats(ss.rows["test7"]);
    }
    private void SetPlayerData(List<GSTU_Cell> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].columnId)
            {
                case "Id":
                    {
                        playerId = list[i].value.ToString();
                        break;
                    }
                case "Money":
                    {
                        money = int.Parse(list[i].value);
                        break;
                    }
                case "Friends":
                    {
                        Friends = JsonUtility.FromJson<Friends>(list[i].value);
                        break;
                    }
                case "Skins":
                    {
                        Skins = JsonUtility.FromJson<Skins>(list[i].value);
                        break;
                    }
            }
        }
    }
    void UpdateStats(List<GSTU_Cell> list)
    {
        Debug.Log(list.Count);
        for(int i =0;i < list.Count; i++)
        {
            Debug.Log(list[i].columnId + ": " + list[i].value);
        }
    }
    private void Write()
    {
        List<string> friends = new List<string>
        {
            "sd",
            "qw",
            "gg",
            "wp"
        };
        List <string> skin = new List<string>
        {
           "sd",
            "qw",
            "gg",
            "wp",
            "sd",
            "qw",
            "gg",
            "wp"
        };
        List<object> writeData = new List<object>
        {
            "test5",
            500.ToString(),
            friends,
            skin
        };
        List<string> list = new List<string> {
            "2020年5月24日 上午09:10:37",
            "test22",
            "test22",
            "test22",
            "test22@gmail.com"
        };
        SpreadsheetManager.Append(new GSTU_Search(spreadsheetId, worksheetName), new ValueRange(list), null);
    }

    private void Read()
    {
        SpreadsheetManager.Read(new GSTU_Search(spreadsheetId, worksheetName), ReadHandler);
    }
    private void ReadHandler(GstuSpreadSheet sheetRef)
    {
        //Debug.Log(sheetRef["test1"].ToString());
        //int money = int.Parse();
        foreach(var value in sheetRef["test5", "Money", true])
        {
            Debug.Log("money: " + value.value.ToString());

        }
        //sheetRef["​Badger​", ​"​Health​"].​value; ​//This will return the value of the Badgers health 
        //foreach​ ​(​var​ value ​in​ spreadsheetRef​["​Badger​",​ ​"Items"​,​ ​true​]) {
        //    Debug​.​log​(​value​.​value​.​ToString​()); //Debug out all the badgers items 
        //} 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
