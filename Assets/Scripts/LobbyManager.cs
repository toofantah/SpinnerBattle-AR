using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField PlayerNameInputField;
    public GameObject UI_loginGameObject;


    [Header("Lobby UI")]
    public GameObject UI_LobbyGameObject;
    public GameObject UI_3DGameObject;


    [Header("Connection Status UI")]
    public GameObject UI_ConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            //Activate Lobby Only
            UI_LobbyGameObject.SetActive(true);
            UI_ConnectionStatusGameObject.SetActive(false);


            UI_3DGameObject.SetActive(true);
            UI_loginGameObject.SetActive(false);
        }else
        {
            //Login and Activatation before Connection
            UI_LobbyGameObject.SetActive(false);
            UI_ConnectionStatusGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);


            UI_loginGameObject.SetActive(true);
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

    #region UI Called back Methods

    public void OnEnterGameButtonClicked()
    {

      
        
        string playerName = PlayerNameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            UI_LobbyGameObject.SetActive(false);
            UI_loginGameObject.SetActive(false);
            UI_3DGameObject.SetActive(false);

            showConnectionStatus = true;

            UI_ConnectionStatusGameObject.SetActive(true);
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("player name is Invalid or ermpty");
        }
    }

    public void OnClickMatchButtonClicked()
    {
        // SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }

    #endregion
    #region Photon CallBacks Method
    public override void OnConnected()
    {
        Debug.Log("Connected to the internet");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "is connected to photon Server");

        UI_3DGameObject.SetActive(true);
        UI_LobbyGameObject.SetActive(true);

        UI_loginGameObject.SetActive(false);
        UI_ConnectionStatusGameObject.SetActive(false);


    }

    #endregion
}
