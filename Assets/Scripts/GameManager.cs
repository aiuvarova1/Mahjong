using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    public void GetReady()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
