using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    public static NetworkManager Instance;

    public UnityEvent JoinedLobbyEvent, JoinedRoomEvent, LeftLobbyEvent, LeftRoomEvent;

    [HideInInspector] public string _roomName;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (Instance != this)
                Destroy(this);
        }

        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결");

        PhotonNetwork.JoinLobby();
    }

    public void JoinOrCreateRoom(string roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 5 }, null);
        _roomName = roomName;
    }

    public void ChangeRoom(string roomName)
    {
        float count = 0f;
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        while (!PhotonNetwork.InLobby)
        {
            count++;
            if(count > 100000)
                throw new Exception("Infinite Loop");
        }

        JoinOrCreateRoom(roomName);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 연결");
        JoinedLobbyEvent.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }

    public override void OnJoinedRoom()
    {
        JoinedRoomEvent.Invoke();
        Spawn();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogFormat("{0}\n{1}", returnCode, message);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비 나감");
        LeftLobbyEvent.Invoke();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방 나감");
        LeftRoomEvent.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate("Prefabs/Player", Vector3.zero, Quaternion.identity);
    }
}
