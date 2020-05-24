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
    public string defaultId = "test";

    public string playerId;
    public int money;
    public Friends Friends;
    public Skins Skins;

    public Transform FriendListTransform;
    public Friend FriendPrefab;

    public string spreadsheetId;
    public string worksheetName;
    public EventHandler finishLoad;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        //Read();
        //Write();
        if (GameManager.userData.id != null && !GameManager.userData.id.Equals(""))
        {
            playerId = GameManager.userData.id;
        }
        else
        {
            playerId = defaultId;
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
            Skins.SkinList = new List<string> { };

            List<string> newData = new List<string>()
            {
                playerId,
                money.ToString(),
                JsonUtility.ToJson(Friends),
                JsonUtility.ToJson(Skins)
            };
            //newData.Add(playerId);
            //newData.Add(money.ToString());
            //newData.Add(JsonUtility.ToJson(Friends));
            //newData.Add(JsonUtility.ToJson(Skins));
            SpreadsheetManager.Append(new GSTU_Search(spreadsheetId, worksheetName), new ValueRange(newData), null);
        }
        finishLoad?.Invoke(this,EventArgs.Empty);
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
                        SetFriend();
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
    
    private void SetFriend()
    {
        foreach(string friendName in Friends.FriendList)
        {
            GameObject friend = Instantiate(FriendPrefab.gameObject, FriendListTransform);

            friend.GetComponent<Friend>().FriendName = friendName;
        }

    }
    
    public bool BuyCharacter(TFTCharacter character)
    {
        int price = CollectionStore.Instance.GetPrice(character);
        if (money-price >= 0)
        {
            money = money - price;
            Debug.Log("Money: "+money);
            if (!Skins.SkinList.Contains(character.ToString()))
            {
                Skins.SkinList.Add(character.ToString());
            }
            SpreadsheetManager.Read(new GSTU_Search(spreadsheetId, worksheetName), BuyCharacter);
            return true;
        }
        return false;
    }
    private void BuyCharacter(GstuSpreadSheet ss)
    {
        BatchRequestBody updateRequest = new BatchRequestBody();
        updateRequest.Add(ss[playerId, "Money"].
            AddCellToBatchUpdate(spreadsheetId, worksheetName, money.ToString()));

        updateRequest.Add(ss[playerId, "Skins"].
           AddCellToBatchUpdate(spreadsheetId, worksheetName, JsonUtility.ToJson(Skins)));

        updateRequest.Send(spreadsheetId, worksheetName, null);
    }

    public bool ChangeCharacter(TFTCharacter character)
    {
        if (Skins.SkinList.Contains(character.ToString()))
        {
            Skins.currSkin = character.ToString();
            SpreadsheetManager.Read(new GSTU_Search(spreadsheetId, worksheetName), ChangeCharacter);
            return true;
        }
        return false;

    }

    private void ChangeCharacter(GstuSpreadSheet ss)
    {
        ss[playerId, "Skins"].UpdateCellValue(spreadsheetId, worksheetName, JsonUtility.ToJson(Skins));
    }

    public void AddMoney(int money)
    {
        money += money;
        SpreadsheetManager.Read(new GSTU_Search(spreadsheetId, worksheetName), ChangeCharacter);

    }

    private void AddMoney(GstuSpreadSheet ss)
    {
        ss[playerId, "Money"].UpdateCellValue(spreadsheetId, worksheetName, money.ToString());
    }
}
