using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPositions;
    public GameObject battleArenaGameObject;

    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Photon callback methods
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
      
            SpawnPlayer();
        }





    }
    #endregion
    #region Private Methods
    private void SpawnPlayer()
    {
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerBattle.PLAYER_SELECTION_NUMBER,out playerSelectionNumber))
        {
            Debug.Log("player selection number is " + (int)playerSelectionNumber);

            int randomSpawnPoint = Random.Range(0, spawnPositions.Length - 1);
            Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;

            GameObject playerGameObject = Instantiate(playerPrefabs[(int)playerSelectionNumber], instantiatePosition, Quaternion.identity);

            PhotonView _photonView = playerGameObject.GetComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(_photonView))
            {
                object[] data = new object[]
                {
                    playerGameObject.transform.position - battleArenaGameObject.transform.position, playerGameObject.transform.rotation, _photonView.ViewID, playerSelectionNumber
                    //extract Battle Arena position from player gameObject!
                };

                RaiseEventOptions raisEventsOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };
                
                //Raise Events
                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raisEventsOptions, sendOptions);
            }else
            {
                Debug.Log("Failed to Allocate the ViewID");
                Destroy(playerGameObject);
            }

        }
           
    }
    #endregion
}
