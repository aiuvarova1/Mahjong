using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {



    public Camera Camera { get; set; }
    string name;
    public string wind;
    public int order;

}
