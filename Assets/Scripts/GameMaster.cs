using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    [SyncVar]
    public int playerCount;

    [SerializeField]
    public List<Camera> allCameras = new List<Camera>();

    [SyncVar]
    public SyncListInt availableCameras = new SyncListInt() ;

    public List<Player> players = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        playerCount = 0;

    }

    public void AddPlayer(Player player,int order)
    {
        if (availableCameras.Count == 0)
        {

            for (int i = 0; i < 4; i++)
            {
                Debug.Log("new");
                availableCameras.Add(i);
            }
        }

        players.Add(player);
        playerCount++;
        availableCameras.Remove(order);
    }

    public void RemovePlayer(Player player,int order)
    {
        players.Remove(player);
        playerCount--;
        availableCameras.Add(order);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
