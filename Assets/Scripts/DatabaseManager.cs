using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance { get; private set; }
    public GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void InstantiateObject()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("DatabaseManager");
            instance = obj.AddComponent<DatabaseManager>();
        }
    }

    /// <summary>
    /// Verify User from database.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    public static void VerifyAccount(string id, string pw)
    {
        try
        {
            instance.StartCoroutine(instance.LoginToDB(id, pw));
        }
        catch (Exception)
        {
            InstantiateObject();
            instance.StartCoroutine(instance.LoginToDB(id, pw));
        }
    }

    IEnumerator LoginToDB(string _id, string _pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("method", "Login");
        form.AddField("id", _id);
        form.AddField("pw", _pw);
        //WWW www = new WWW(connectManager.databaseIP, form);
        //yield return www;
        //Debug.Log(www.text);
        UnityWebRequest www = UnityWebRequest.Post(gameManager.databaseIP, form);
        
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            if (!jsonString.Equals("Verify Account Fail, Try Again!"))
            {
                gameManager.userData = JsonUtility.FromJson<UserData>(jsonString);
                SceneManager.LoadScene(gameManager.lobbyScene);
            }
        }   
    }
}
