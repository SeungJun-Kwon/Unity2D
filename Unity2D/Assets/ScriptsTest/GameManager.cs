using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MapData _curMap;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        if(_curMap == null)
            _curMap = Resources.Load("MapData/Test") as MapData;
    }
}
