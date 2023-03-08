using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChannelSelectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<Button> _channels = new List<Button>();
    [SerializeField] Button _joinButton, _leaveButton;
    [SerializeField] Text _sellectedChannel;
    [SerializeField] TMP_Text _nickName;

    public override void OnEnable()
    {
        _nickName.text = PhotonNetwork.LocalPlayer.NickName;
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

        NetworkManager.Instance.JoinOrCreateRoom(_sellectedChannel.text);
        PhotonNetwork.LoadLevel("SampleScene");
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
