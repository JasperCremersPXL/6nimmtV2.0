using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

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
    public Text ScoreText;
    public GameObject ScoreManager;
    private bool cardsDealt = false;
    private static int numberOfPlayers = 0;
    // private static List<int> chosenCards = new List<int>();
    // private static List<Mirror.NetworkConnection> clients = new List<Mirror.NetworkConnection>();
    private static Dictionary<NetworkConnection, GameObject> connectionScoreManagers = new Dictionary<NetworkConnection, GameObject>();

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
        if (Row1.transform.childCount == 0)
        {
            CmdGetRowCards();
        }
        numberOfPlayers++;

        // clients.Add(connectionToClient);

        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        //CardManager = Instantiate(CardManagerPrefab, new Vector2(0, 0), Quaternion.identity);
        CardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
        CardManager.InstantiateRows();
        if (!RowCardsDealt.RowCardsAreDealt)
        {
            DealRowCards();
            RowCardsDealt.RowCardsAreDealt = true;
        }
    }

    public void DealRowCards()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject card = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
            int cardnumber = CardManager.GetCardNumber(card);
            CardManager.AddCardToRow(card, i);
            NetworkServer.Spawn(card, connectionToClient);
        }
    }

    [Command]
    public void CmdGetRowCards()
    {
        Debug.Log(CardManager.Rows.Count);
        for (int i = 0; i < CardManager.Rows.Count; i++)
        {
            foreach (var card in CardManager.Rows[i].GetComponent<RowManager>().CardsInRow)
            {
                RpcShowCards(card, card.GetComponent<CardInfo>().CardNumber, "dealt", i);
            }
        }
    }

    [Command]
    public void CmdDealCards()
    {
        if (!cardsDealt)
        {
            GameObject scoreManager = Instantiate(ScoreManager, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(scoreManager, connectionToClient);

            connectionScoreManagers.Add(connectionToClient, scoreManager);

            for (int j = 0; j < 10; j++)
            {
                GameObject card = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
                int cardNumber = CardManager.GetCardNumber(card);
                card.GetComponent<CardInfo>().connectionToClient = connectionToClient;
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
        CardManager.PlayCard(card);
        if (CardManager.CardsPlayedThisRound.Count >= numberOfPlayers)
        {
            CardManager.CardsPlayedThisRound.OrderByDescending(Card => Card.GetComponent<CardInfo>().CardNumber);
            while(CardManager.CardsPlayedThisRound.Count > 0)
            {
                card = CardManager.CardsPlayedThisRound[0];
                CardManager.CardsPlayedThisRound.Remove(card);
                int lowestDiff = 999999999;
                int lowestDiffIndex = -1;
                for (int j = 0; j < CardManager.Rows.Count; j++)
                {
                    int currentDiff = CardManager.Rows[j].GetComponent<RowManager>().GetDifference(card.GetComponent<CardInfo>().CardNumber);
                    if (currentDiff < lowestDiff)
                    {
                        lowestDiff = currentDiff;
                        lowestDiffIndex = j;
                    }
                }
                if (lowestDiffIndex != -1)
                {
                    if (CardManager.Rows[lowestDiffIndex].GetComponent<RowManager>().IsFull)
                    {
                        var score = CardManager.Rows[lowestDiffIndex].GetComponent<RowManager>().GetRowScore();
                        // Punten
                        CardManager.Rows[lowestDiffIndex].GetComponent<RowManager>().ClearRowAndAddCard(card);

                        RpcDestroyCardsInRow($"Row{lowestDiffIndex + 1}");
                    }
                    CardManager.Rows[lowestDiffIndex].GetComponent<RowManager>().AddCardToRow(card);
                }
                else
                {
                    int lowestScore = 150;
                    int lowestScoreIndex = -1;

                    for (int j = 0; j < CardManager.Rows.Count; j++)
                    {
                        int rowScore = CardManager.Rows[j].GetComponent<RowManager>().GetRowScore();
                        
                        if (rowScore < lowestScore)
                        {
                            lowestScore = rowScore;
                            lowestScoreIndex = j;
                        }
                    }
                    Debug.Log($"player ... had to take row {lowestScoreIndex + 1}");

                    CardManager.Rows[lowestScoreIndex].GetComponent<RowManager>().ClearRowAndAddCard(card);

                    Debug.Log(card.GetComponent<CardInfo>().connectionToClient);
                    var value = connectionScoreManagers[card.GetComponent<CardInfo>().connectionToClient];
                    ScoreManager scoreManager = value.GetComponent<ScoreManager>();
                    RpcUpdateScore(card.GetComponent<CardInfo>().connectionToClient, scoreManager, lowestScore);
                    RpcDestroyCardsInRow($"Row{lowestScoreIndex+1}");
                }
            }
            for (int i = 0; i < CardManager.Rows.Count; i++)
            {
                foreach (var obj in CardManager.Rows[i].GetComponent<RowManager>().CardsInRow)
                {
                    RpcPlaceCards(obj, $"Row{i + 1}", obj.GetComponent<CardInfo>().CardNumber);
                }
            }
            CardManager.CardsPlayedThisRound.Clear();
        }
    }

    [TargetRpc]
    void RpcUpdateScore(NetworkConnection target, ScoreManager scoreManager, int score) {
        scoreManager.Score += score;
        Debug.Log($"Score: {scoreManager.Score}");
        ScoreText.text = $"Score: {scoreManager.Score}";
    }

    [ClientRpc]
    void RpcPlaceCards(GameObject card, string rowId, int cardNumber)
    {
        GameObject row = GameObject.Find(rowId);
        card.transform.SetParent(row.transform, false);
        card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/{cardNumber}");
        card.GetComponent<DragDrop>().isDraggable = false;
    }

    [ClientRpc]
    void RpcDestroyCardsInRow(string rowId)
    {
        GameObject row = GameObject.Find(rowId);
        for (int i = 0; i < row.transform.childCount;i++)
        {
            Destroy(row.transform.GetChild(i).gameObject);
        }
    }

    [ClientRpc]
    void RpcShowCards(GameObject card, int cardNumber, string type, int rowIndex)
    {
        if (type == "dealt")
        {
            if (rowIndex < 0)
            {
                if (hasAuthority)
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
            card.transform.SetParent(Rows[rowIndex].transform, false);
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/{cardNumber}");
            card.GetComponent<DragDrop>().isDraggable = false;
        }
    }

    [ClientRpc]
    void RpcDisableCards()
    {
        if (hasAuthority)
        {
            foreach (GameObject card in CardsInHand)
            {
                card.GetComponent<DragDrop>().isDraggable = false;
            }
        }
    }
}