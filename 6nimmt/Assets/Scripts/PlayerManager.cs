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
    public GameObject PlayedCards;
    public CardManager CardManagerPrefab;
    public CardManager CardManager;
    public bool CanSelectCard = true;
    public List<GameObject> CardsInHand;
    public List<GameObject> Rows;

    private bool cardsDealt = false;

    public override void OnStartClient() 
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        Row1 = GameObject.Find("Row1");
        Row2 = GameObject.Find("Row2");
        Row3 = GameObject.Find("Row3");
        Row4 = GameObject.Find("Row4");
        Rows = new List<GameObject>();
        Rows.Add(Row1);
        Rows.Add(Row2);
        Rows.Add(Row3);
        Rows.Add(Row4);
        DropZone = GameObject.Find("DropZone");
        PlayedCards = GameObject.Find("PlayedCards");
        CardsInHand = new List<GameObject>();
        if(Row1.transform.childCount == 0)
        {
            CmdGetRowCards();
        }
    }

    [Server]
    public override void OnStartServer() 
    {
        base.OnStartServer();
        CardManager = Instantiate(CardManagerPrefab, new Vector2(0,0), Quaternion.identity);
        CardManager.InstantiateRows();
        DealRowCards();
    }

    public void DealRowCards() 
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject card = Instantiate(Card, new Vector2(0,0), Quaternion.identity);
            int cardnumber = CardManager.GetCardNumber(card);
            CardManager.AddCardToRow(card, i);
            NetworkServer.Spawn(card, connectionToClient);
        }
    }

    [Command]
    public void CmdGetRowCards() 
    {
        for(int i = 0; i < CardManager.Rows.Count; i++) 
        {
            foreach(var card in CardManager.Rows[i].GetComponent<RowManager>().CardsInRow)
            {
                RpcShowCards(card, card.GetComponent<CardInfo>().CardNumber, "dealt", i);
            }
        }
    }

    [Command]
    public void CmdDealCards()
    {
        if(!cardsDealt)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject card = Instantiate(Card, new Vector2(0,0), Quaternion.identity);
                int cardNumber = CardManager.GetCardNumber(card);
                NetworkServer.Spawn(card, connectionToClient);
                RpcShowCards(card, cardNumber, "dealt", -1);
            }
            cardsDealt = true;
        }
    }

    public void PlayCard(GameObject card) 
    {
        CmdPlayCard(card);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCards(card, card.GetComponent<CardInfo>().CardNumber, "played", -1);
        RpcDisableCards();
    }

    [ClientRpc]
    void RpcShowCards(GameObject card, int cardNumber, string type, int rowIndex) 
    {
        if (type == "dealt")
        {
            if(rowIndex < 0) 
            {
                if(hasAuthority) 
                {
                    card.transform.SetParent(PlayerArea.transform, false);
                    card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/{cardNumber}");
                    CardsInHand.Add(card);
                }
            } 
            else
            {
                GameObject row = Rows[rowIndex];
                card.transform.SetParent(row.transform, false);
                card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/{cardNumber}");
                card.GetComponent<DragDrop>().isDraggable = false;
            }
        }
        else if (type == "played")
        {
            card.transform.SetParent(PlayedCards.transform, false);
        }
    }

    [ClientRpc]
    void RpcDisableCards() 
    {
        if(hasAuthority)
        {
            foreach(GameObject card in CardsInHand) 
            {
                card.GetComponent<DragDrop>().isDraggable = false;
            }
        }
    }
}