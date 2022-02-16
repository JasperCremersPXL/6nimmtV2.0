using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerManager : NetworkBehaviour 
{
    public GameObject Card;
    public GameObject PlayerArea;
    public GameObject Row1;
    public GameObject Row2;
    public GameObject Row3;
    public GameObject Row4;
    public GameObject DropZone;
    public CardManager CardManager;
    public bool CanSelectCard = true;
    public List<GameObject> CardsInHand;

    private bool cardsDealt = false;

    public override void OnStartClient() 
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        Row1 = GameObject.Find("Row1");
        Row2 = GameObject.Find("Row2");
        Row3 = GameObject.Find("Row3");
        Row4 = GameObject.Find("Row4");
        DropZone = GameObject.Find("DropZone");
        CardsInHand = new List<GameObject>();
    }

    [Server]
    public override void OnStartServer() 
    {
        base.OnStartServer();

    }

    [Command]
    public void CmdDealCards()
    {
        if(!cardsDealt)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject card = Instantiate(Card, new Vector2(0,0), Quaternion.identity);
                CardsInHand.Add(card);
                int cardNumber = CardManager.GetCardNumber(card);
                NetworkServer.Spawn(card, connectionToClient);
                RpcShowCards(card, cardNumber, "dealt");
            }
            cardsDealt = true;
        }
    }

    [Command]
    public void CmdSelectCard(GameObject card) 
    {
        CardManager.PlayCard(card);
        RpcDisableCards();
    }

    [ClientRpc]
    void RpcShowCards(GameObject card, int cardNumber, string type) 
    {
        if (type == "dealt")
        {
            if(hasAuthority) 
            {
                card.transform.SetParent(PlayerArea.transform, false);
                card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/{cardNumber}");
            }
        }
        else if (type == "played")
        {

        }
    }

    [ClientRpc]
    void RpcDisableCards() 
    {
        if(hasAuthority)
        {
            foreach(GameObject cardInHand in CardsInHand) 
            {
                cardInHand.GetComponent<DragDrop>().isDraggable = false;
            }
        }
    }
}