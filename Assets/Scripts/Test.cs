using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Test : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        Debug.LogError("Start");
        if (!isLocalPlayer)
        {
            Debug.Log("not local " );
            return;

        }
        Debug.LogError("Local");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
