using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClickHandler : MonoBehaviour
{

    public void OnMouseDown()
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.SelectTile(gameObject);
    }
}
