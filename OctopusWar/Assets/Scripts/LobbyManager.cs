using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject uI_LoginGameobject;

    [Header("Lobby UI")]
    public GameObject uI_LobbyGameobject;
    //public GameObject uI_3DGameObject;

    [Header("Connection Status UI")]
    public GameObject uI_ConnectionStatusGameobject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;


    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Connected");
            uI_ConnectionStatusGameobject.SetActive(false);
            uI_LoginGameobject.SetActive(false);

            //uI_3DGameObject.SetActive(true);
            uI_LobbyGameobject.SetActive(true);
        }
        else
        {
            uI_LobbyGameobject.SetActive(false);
            //uI_3DGameObject.SetActive(false);
            uI_ConnectionStatusGameobject.SetActive(false);

            uI_LoginGameobject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }

    #endregion

    #region UI Callback Methods
    public void OnEnterGameButtonClicked()
    {

        string playerName = playerNameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            uI_LobbyGameobject.SetActive(false);
            //uI_3DGameObject.SetActive(false);
            uI_LoginGameobject.SetActive(false);

            showConnectionStatus = true;
            uI_ConnectionStatusGameobject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is invalid or empty");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        //SceneManager.LoadScene("Scene_Loading");
        //SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
        SceneLoader.Instance.LoadScene("BattleArena_H");
    }

    public void OnJoinRoomButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_SearchRoom");
    }
    #endregion

    #region PHOTON Callback Methods
    public override void OnConnected()
    {
        Debug.Log("We connected to Internet");

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");

        uI_LoginGameobject.SetActive(false);
        uI_ConnectionStatusGameobject.SetActive(false);

        uI_LobbyGameobject.SetActive(true);
        //uI_3DGameObject.SetActive(true);
    }


    #endregion
}