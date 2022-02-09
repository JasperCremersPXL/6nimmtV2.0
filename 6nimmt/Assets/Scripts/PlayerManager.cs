using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror; 

public class PlayerManager : NetworkBehaviour
{
    public GameObject PlayerArea;
    public GameObject DrawButton;

    public override void OnStartClient() {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        DrawButton = GameObject.Find("DrawButton");
    }
}
