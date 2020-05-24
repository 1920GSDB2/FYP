using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public static EndGame Instance;

    Material mat;

    public TextMeshProUGUI EndText;

    public Button ContinueButton, EndButton, LittleBack;

    public float time = 0.05f;

    private float current = 1, next;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        mat = GetComponent<Image>().material;
        ContinueButton.onClick.AddListener(delegate { OnStayView(); });
        EndButton.onClick.AddListener(delegate { OnBack(); });
        LittleBack.onClick.AddListener(delegate { OnBack(); });
    }

    private void Update()
    {
        current = Mathf.Lerp(current, next, time);
        mat.SetFloat("_DissolveAmount", current);
    }

    public void OnEnd()
    {
        next = 0;
    }

    public void OnStayView()
    {
        next = 1;
        LittleBack.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(false);
        EndButton.gameObject.SetActive(false);
        EndText.gameObject.SetActive(false);
    }

    public void OnBack()
    {
      
        next = 1;

        current = 1;

        LittleBack.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(true);
        EndButton.gameObject.SetActive(true);
        EndText.gameObject.SetActive(false);

        if (PhotonNetwork.isMasterClient)
        {
            if (PhotonNetwork.playerList.Length > 1)
                PhotonNetwork.SetMasterClient(PhotonNetwork.masterClient.GetNext());
        }
        StartCoroutine(DisconnectLoading());
    }
    private IEnumerator DisconnectLoading()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.inRoom)
            yield return null;
        SceneManager.LoadScene("Lobby");
    }

}
