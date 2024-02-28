using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPositions;

    public GameObject battleArenaGameobject;

    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; //onevent 등록 취소
    }

    #region Photon Callback Methods

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion recievedRotation = (Quaternion)data[1];
            int receivedPlayerSelectionData = (int)data[3];

            GameObject player = Instantiate(playerPrefabs[receivedPlayerSelectionData], receivedPosition + battleArenaGameobject.transform.position, recievedRotation); //원격 기준의 배틀 안 player 위치
            PhotonView _photonView = player.GetComponent<PhotonView>();
            _photonView.ViewID = (int)data[2];
        }
    }


    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log("Player selection number is " + (int)playerSelectionNumber);


                Vector3 instantiatePosition = spawnPositions[0].position;

                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    instantiatePosition = spawnPositions[0].position;
                }
                else if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    instantiatePosition = spawnPositions[1].position;
                }

                //PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
                GameObject playerGameobject = Instantiate(playerPrefabs[(int)playerSelectionNumber], instantiatePosition, Quaternion.identity);

                PhotonView _photonView = playerGameobject.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(_photonView)) //viewid 할당 성공
                {

                    object[] data = new object[]// 배열인가?
                    {
                    playerGameobject.transform.position - battleArenaGameobject.transform.position, playerGameobject.transform.rotation, _photonView.ViewID, playerSelectionNumber 
                    };

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others, 
                        CachingOption = EventCaching.AddToRoomCache
                    };

                    SendOptions sendOptions = new SendOptions
                    {
                        Reliability = true 
                    };

                    PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);
                    //raise events!

                }
                else
                {
                    Debug.Log("Failed to allocate a viewID");
                    Destroy(playerGameobject);
                }
            }
        }
    }
    #endregion
}
