using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController Instance;

    public Text _roomName;
    public PlayerUI _playerUI;
    public InventoryManager _inventoryManager;
    public EquipmentManager _equipmentManager;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        _roomName.text = NetworkManager.Instance._roomName;
    }
}
