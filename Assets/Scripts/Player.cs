using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {



    public Camera Camera { get; set; }
    string name;
    string wind;
    public int order;

    public GameMaster gameMaster;

    //void Start()
    //{
    //    GameObject script = GameObject.Find("GameMaster");
    //    gameMaster = (GameMaster)script.GetComponent(typeof(GameMaster));
    //    Debug.Log($"{gameMaster.playerCount} players now");
    //    Debug.Log($"{gameMaster.availableCameras.Count} cameras now");
    //    //gameMaster = FindObjectOfType<GameMaster>();

    //    if (!isLocalPlayer)
    //        return;

    //    order = Random.Range(0, gameMaster.availableCameras.Count);

    //    Camera = Instantiate(gameMaster.allCameras[order]);
    //    Debug.Log(Camera.name);

    //    Camera.enabled = false;

    //    Debug.Log(gameMaster.players.Count);

    //    //for (int i = 0; i < gameMaster.players.Count; i++)
    //    //{
    //    //    Player p = gameMaster.players[i].player.GetComponent<Player>();
    //    //    PlayerUI ui= gameMaster.players[i].player.GetComponent<PlayerUI>();
    //    //    if (gameMaster.players[i].player != null)
    //    //    {
    //    //        p.Camera.enabled = false;
    //    //        ui.UI.enabled = false;
    //    //    }
    //    //}

    //    CmdIncrease();

    //    Camera.enabled = true;

    
}
