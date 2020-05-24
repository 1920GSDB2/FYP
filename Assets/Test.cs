using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TestFriend
{
    public string name;
    public string[] friends;
}
public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public string associatedSheet;
    public string associatedWorksheet;  

    List<string> list;
    void Start()
    {
        //WriteTest();
        UpdateHandler();

    }
    private void UpdateHandler()
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), UpdateTest);
    }
    private void WriteTest()
    {
        List<string> list1 = new List<string>{
            "Test1",
            "Test2",
            "Test3",
            "Test4",
            "asdfTest5",
            "Test6",
        };
        List<string> list2 = new List<string>{
            "Test5",
            "Testasdf6",
            "Test7",
            "Test8",
            "Tesdafst9",
            "Test10",
        };
        List<List<string>> combineList = new List<List<string>>{
            list1,
            list2
        };
        TestFriend test = new TestFriend();
        test.name = "ABC";
        test.friends = list1.ToArray();
        List<string> list3 = new List<string>{
            JsonUtility.ToJson(test),
        };
        JsonUtility.ToJson(list1);
        SpreadsheetManager.Append(new GSTU_Search(associatedSheet, associatedWorksheet), new ValueRange(combineList), null);
    }
    private void UpdateTest(GstuSpreadSheet ss)
    {
        List<string> list2 = new List<string>{
            "Test5",
            "Testasdas6",
            "Test7",
            "Testasd8",
            "Tesasdasdt9",
            "Test10",
        };
        TestFriend test = new TestFriend();

        test.name = "Bdd";
        test.friends = list2.ToArray();
        ss["Bdd", "data"].UpdateCellValue(associatedSheet, associatedWorksheet, JsonUtility.ToJson(test));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
