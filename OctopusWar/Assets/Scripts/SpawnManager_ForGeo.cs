using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using System;
using Unity.VisualScripting;
using Google.CreativeLab.BalloonPop;

public class SpawnManagerForGeo : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPositions;

    public GameObject battleArenaGameobject;

    [Header("GPS")]
    [SerializeField] XROrigin xrOrigin;
    private AREarthManager _earthManager;
    private ARAnchorManager _anchorManager;
    private bool isAnchorSet = true;
    static public float ESTIMATED_CAM_HEIGHT_FROM_FLOOR = 1.3f;
    [SerializeField] Camera aRCamera; //public Mercator mercator;

    //private bool isCouroutineRunning = false;

    

    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0,
        PlayerGeoEventCode = 1,
    }
    private void Awake()
    {
        _anchorManager = xrOrigin.GetComponent<ARAnchorManager>();
        if (xrOrigin == null || _anchorManager == null)
        {
            Debug.Log("no anchor manager");
        }
        GameObject earthManagerGO = new GameObject("AREarthManager", typeof(AREarthManager));
        _earthManager = earthManagerGO.GetComponent<AREarthManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnchorSet)
        {
            if (_earthManager == null)
                return;


            TrackingState trackingState = _earthManager.EarthTrackingState;
            if (trackingState != TrackingState.Tracking)
            {
                return;
            }

            //if (!isCouroutineRunning)
            // {
            Debug.Log("isAnchorSet = start");
            StartCoroutine(WaitingArenaGps());
            //}
        }
    }
    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; //onevent ��� ���
    }

    #region GeoLocation Methods

    IEnumerator WaitingArenaGps()
    {
        yield return StartCoroutine(arenaGps());
    }
    IEnumerator arenaGps()
    {
        //isCouroutineRunning = true;
        Debug.Log("arenaGps Start");

        if (_earthManager == null || _anchorManager == null)
        {
            Debug.Log("null break");
            //isCouroutineRunning=false;
            yield break;
        }


        // EarthManager�� ���� ������ Ȯ���մϴ�.
        TrackingState trackingState = _earthManager.EarthTrackingState;
        if (trackingState != TrackingState.Tracking)
        {
            Debug.Log("null break");
            //isCouroutineRunning=false;
            yield break;
        }
        GeospatialPose cameraPose = _earthManager.CameraGeospatialPose;
        Debug.Log("cameraPose is" + cameraPose.Altitude);
        //�Ʒ����� gps ��ǥ�� �޾ƿ� gps �� �ڱ� �ڴ����.

        Mercator.GeoCoordinate geoArenaPose = getDistance(cameraPose);
        //��Ŀ ȸ�� x
        Quaternion anchorRot = Quaternion.AngleAxis(0, new Vector3(0.0f, 1.0f, 0.0f)); //��Ŀ�� ȸ������ �ʿ�������� ������. 
                                                                                       //Gps ��� ARAnchor

        float alt = (float)geoArenaPose.altitude - ESTIMATED_CAM_HEIGHT_FROM_FLOOR;
        ARGeospatialAnchor newAnchor = _anchorManager.AddAnchor(geoArenaPose.latitude, geoArenaPose.longitude, alt, anchorRot);

        if (newAnchor != null)
        {
            isAnchorSet = true; //�ڷ�ƾ �׸�
            raiseGpsEvent(geoArenaPose); //�̺�Ʈ ����
                                         //battleArenaGameobject.transform.position = newAnchor.transform.position;(y���� ����)

            battleArenaGameobject.SetActive(false);

            battleArenaGameobject.transform.SetParent(newAnchor.transform, false);
            battleArenaGameobject.transform.localPosition = Vector3.zero;
            battleArenaGameobject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            newAnchor.gameObject.SetActive(true);
            battleArenaGameobject.SetActive(true);


            //battleArenaGameobject.transform.localScale = Vector3.one;



            //battleArenaGameobject.transform.SetParent(newAnchor.transform, false);
            Debug.Log("geo success :" + geoArenaPose.latitude);

            yield return new WaitForSeconds(2);

            SetPlayer(newAnchor);
        }
        //isCouroutineRunning = false;

    }

    private Mercator.GeoCoordinate getDistance(GeospatialPose cameraPose)
    {
        float distance = Vector3.Distance(battleArenaGameobject.transform.position, aRCamera.transform.position);
        Debug.Log($"Distance: {distance}");

        Mercator.GeoCoordinate geoCoord = new Mercator.GeoCoordinate(cameraPose.Latitude, cameraPose.Longitude, cameraPose.Altitude);
        Mercator.GeoCoordinate geoCoordAhead = geoCoord.CalculateDerivedPosition(distance, cameraPose.Heading);
        Debug.Log("Derived Position - Latitude: " + geoCoordAhead.latitude + ", Longitude: " + geoCoordAhead.longitude + ", Altitude: " + geoCoordAhead.altitude);
        return geoCoordAhead;
    }

    //�Ʒ��� ��ġ �۽�
    private void raiseGpsEvent(Mercator.GeoCoordinate _geoPose)
    {
        double alt = _geoPose.altitude - ESTIMATED_CAM_HEIGHT_FROM_FLOOR;
        object[] data = new object[]
        {
            _geoPose.latitude, _geoPose.longitude, alt, battleArenaGameobject.transform.position.y
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

        PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerGeoEventCode, data, raiseEventOptions, sendOptions);
    }
    #endregion
    #region Photon Callback Methods

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode) //player ��ġ
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion recievedRotation = (Quaternion)data[1];
            int receivedPlayerSelectionData = (int)data[3];

            GameObject player = Instantiate(playerPrefabs[receivedPlayerSelectionData], receivedPosition + battleArenaGameobject.transform.position, recievedRotation); //���� ������ ��Ʋ �� player ��ġ
            PhotonView _photonView = player.GetComponent<PhotonView>();
            _photonView.ViewID = (int)data[2];
        }

        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerGeoEventCode) // �� gps ��ġ�� �Ʒ��� ��ġ�� ����
        {
            Debug.Log("event Start");
            GeospatialPose geoPose;

            object[] data = (object[])photonEvent.CustomData;
            geoPose.Latitude = (double)data[0];
            geoPose.Longitude = (double)data[1];
            geoPose.Altitude = (double)data[2];



            Quaternion anchorRot = Quaternion.AngleAxis(0, new Vector3(0.0f, 1.0f, 0.0f)); //��Ŀ�� ȸ������ �ʿ�������� ������.
                                                                                           //Gps ��� ARAnchor
            ARGeospatialAnchor newAnchor = _anchorManager.AddAnchor(geoPose.Latitude, geoPose.Longitude, geoPose.Altitude, anchorRot);
            if (newAnchor != null)
            {

                float distance = Vector3.Distance(newAnchor.transform.position, aRCamera.transform.position);
                Debug.Log($"me and synchronized arena Distance: {distance}");

                battleArenaGameobject.SetActive(false);
                battleArenaGameobject.transform.SetParent(newAnchor.transform, false);
                battleArenaGameobject.transform.localPosition = Vector3.zero;
                battleArenaGameobject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                newAnchor.gameObject.SetActive(true);
                battleArenaGameobject.SetActive(true);
                Debug.Log("GPS Synchornize  success" + geoPose.Latitude);

                SetPlayer(newAnchor);
            }


        }
    }


    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //��Ŀ���� ����
            isAnchorSet = false;
        }
    }
    public void SetPlayer(ARGeospatialAnchor anchor)
    {

        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerGame.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log("Player selection number is " + (int)playerSelectionNumber);

                Vector3 instantiatePosition = spawnPositions[0].position;
                //Vector3 instantiatePosition = spawnPositions[0].position;
                //instantiatePosition = battleArenaGameobject.transform.Find("MyRespawn").position;

                Transform respawnArena = battleArenaGameobject.transform.Find("MyRespawn");
                Transform enemyspawnArena = battleArenaGameobject.transform.Find("EnemyRespawn");



                //PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
                GameObject playerGameobject = Instantiate(playerPrefabs[(int)playerSelectionNumber], battleArenaGameobject.transform.position, Quaternion.identity); //player ��ü ����

                PhotonView _photonView = playerGameobject.GetComponent<PhotonView>();

                playerGameobject.transform.SetParent(anchor.transform, false);
                Debug.Log("anhor has a child called playerGameobject");

                if (respawnArena != null)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                    {
                        instantiatePosition = respawnArena.position;
                        Debug.Log("right position");
                    }
                }
                else if (enemyspawnArena != null)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                    {
                        instantiatePosition = enemyspawnArena.position;
                        Debug.Log("right position");
                    }
                }

                playerGameobject.transform.position = instantiatePosition;
                Rigidbody rb = playerGameobject.GetComponentInChildren<Rigidbody>();
                rb.useGravity = true;

                if (PhotonNetwork.AllocateViewID(_photonView)) //viewid �Ҵ� ����
                {

                    object[] data = new object[]// �迭�ΰ�?
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
