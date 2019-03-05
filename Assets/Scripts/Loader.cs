using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameMaster gameMasterPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (GameMaster.instance == null) Instantiate(gameMasterPrefab);
    }

}
