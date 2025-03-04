﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    public Main.GameManager GameManager;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static void InstantiateObject()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("DatabaseManager");
            Instance = obj.AddComponent<DatabaseManager>();
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
            Instance.StartCoroutine(Instance.LoginToDB(id, pw));
        }
        catch (Exception)
        {
            InstantiateObject();
            Instance.StartCoroutine(Instance.LoginToDB(id, pw));
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
        UnityWebRequest www = UnityWebRequest.Post(GameManager.databaseIP, form);
        
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            LoginUI.Instance.LoginFailed("Login Timeout");
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            Debug.Log(jsonString);
            if (!jsonString.Equals("Verify Account Fail, Try Again!"))
            {
                GameManager.userData = JsonUtility.FromJson<UserData>(jsonString);
                SceneManager.LoadScene(GameManager.lobbyScene);
            }
            else if(jsonString.Equals("Verify Account Fail, Try Again!"))
            {
                LoginUI.Instance.LoginFailed("Login Failed");
            }
            else
            {
                LoginUI.Instance.LoginFailed("Login Timeout");
            }
        }
        LoginUI.Instance.IsLoading = false;
    }
}
