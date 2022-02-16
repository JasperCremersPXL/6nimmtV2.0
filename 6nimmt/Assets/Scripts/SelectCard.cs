using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelectCard : NetworkBehaviour
{
    public PlayerManager PlayerManager;

    public void OnClick() 
    {
        GameObject DropZone = GameObject.Find("DropZone");
        if(DropZone.transform.childCount > 0)
        {
            GameObject card = DropZone.transform.GetChild(0).gameObject;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.CmdSelectCard(card);
        }
    }
}
