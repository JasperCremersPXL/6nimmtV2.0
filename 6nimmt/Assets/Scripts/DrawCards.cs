using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawCards : NetworkBehaviour
{
    public PlayerManager PlayerManager;

    public void OnClick() 
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();  
        //Debug.Log(PlayerManager.dealtCards.Count);
        PlayerManager.CmdDealCards();
    }
}
