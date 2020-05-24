using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;
using System;

public class GoogleSheetManager : MonoBehaviour
{
    public Friends Friends;
    public Skins Skins;

    public string spreadheetId;
    public string worksheetName;

    // Start is called before the first frame update
    void Start()
    {
        //Read();
        Write();
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
        SpreadsheetManager.Append(new GSTU_Search(spreadheetId, worksheetName), new ValueRange(list), null);
    }

    private void Read()
    {
        SpreadsheetManager.Read(new GSTU_Search(spreadheetId, worksheetName), ReadHandler);
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
