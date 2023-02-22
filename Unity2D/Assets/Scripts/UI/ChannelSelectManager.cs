using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChannelSelectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Button> _channels = new List<Button>();
    [SerializeField] Button _joinButton, _leaveButton;
    [SerializeField] Text _sellectedChannel;
    [SerializeField] InputField _nickNameInput;

    private void Start()
    {
        foreach (var c in _channels)
            c.interactable = false;
        _joinButton.interactable = false;
        _leaveButton.interactable = false;

        NetworkManager.Instance.JoinedLobbyEvent.AddListener(ActiveChannels);
    }

    public void ActiveChannels()
    {
        foreach (var c in _channels)
            c.interactable = true;
        _joinButton.interactable = true;
    }

    public void OnClickedChannel()
    {
        Text channel = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>();

        if (channel != null)
            _sellectedChannel.text = channel.text;
        else
            _sellectedChannel.text = "null";
    }

    public void OnClickedJoin()
    {
        if (_sellectedChannel.text.Equals("null"))
            return;

        if (!_nickNameInput.text.Equals(""))
        {
            NetworkManager.Instance.JoinOrCreateRoom(_sellectedChannel.text);
            PhotonNetwork.LocalPlayer.NickName = _nickNameInput.text;
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    public void OnClickedChange()
    {
        if (_sellectedChannel.text.Equals("null"))
            return;

        NetworkManager.Instance.ChangeRoom(_sellectedChannel.text);

        _joinButton.interactable = false;
        _leaveButton.interactable = true;
    }

    public void OnClickedLeave()
    {
        NetworkManager.Instance.LeaveRoom();

        foreach (var c in _channels)
            c.interactable = true;
        _joinButton.interactable = true;
        _leaveButton.interactable = false;
    }
}
