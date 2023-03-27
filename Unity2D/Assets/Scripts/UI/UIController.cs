using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private static UIController Instance;

    [SerializeField] Text _roomName;
    [SerializeField] PlayerUI _playerUI;
    [SerializeField] InventoryManager _inventoryManager;

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
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (_inventoryManager.gameObject.activeSelf)
                _inventoryManager.gameObject.SetActive(false);
            else
                _inventoryManager.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        _roomName.text = NetworkManager.Instance._roomName;
    }
}
