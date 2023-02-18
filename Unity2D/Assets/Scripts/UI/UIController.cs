using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text _roomName;

    private void OnEnable()
    {
        _roomName.text = NetworkManager.Instance._roomName;
    }
}
