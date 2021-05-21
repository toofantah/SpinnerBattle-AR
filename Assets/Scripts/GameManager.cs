using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject UI_InformPanelGameObject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGamesButtonGameObject;
    public GameObject adjust_Button;
    public GameObject raycastCenter_Image;
    // Start is called before the first frame update
    void Start()
    {
        UI_InformPanelGameObject.SetActive(true);
       
    }

    // Update is called once per frame
    void Update()
    {
        //PhotonNetwork.JoinRandomRoom();
    }

    #region UI CAllBack Methods!

    public void JoinRandomRoom()
    {
        uI_InformText.text = "Searching for available rooms...";
        PhotonNetwork.JoinRandomRoom();
        searchForGamesButtonGameObject.SetActive(false);
    }

    public void OnQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }

        
    }
    #endregion

    #region Photon Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
       

        Debug.Log(message);
        uI_InformText.text = message;
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom()
    {
        adjust_Button.SetActive(false);
        raycastCenter_Image.SetActive(false);
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = " Joined to" + PhotonNetwork.CurrentRoom.Name + " : Waiting for Other player...";
        }
        else
        {
            uI_InformText.text = " Joined to" + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(UI_InformPanelGameObject, 2.0f));
            searchForGamesButtonGameObject.SetActive(false);
        }
        Debug.Log(PhotonNetwork.NickName + " is joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        Debug.Log(newPlayer.NickName + " Joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text = newPlayer.NickName + " Joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;
        searchForGamesButtonGameObject.SetActive(false);
        StartCoroutine(DeactivateAfterSeconds(UI_InformPanelGameObject, 2.0f));

    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }



    #endregion

    #region Private Methods
    void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        
        PhotonNetwork.CreateRoom(randomRoomName,roomOptions);//Creating the Room
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
    #endregion

}
