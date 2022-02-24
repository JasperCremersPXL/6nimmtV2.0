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
    public bool cardsDealt = false;
    private static int numberOfPlayers = 0;
    private static List<NetworkConnection> clients = new List<NetworkConnection>();
    private static Dictionary<NetworkConnection, GameObject> connectionScoreManagers = new Dictionary<NetworkConnection, GameObject>();
    private static int totalHandsPlayed;
    private bool gameOver = false;
    private bool connectionAdded = false;
    private bool cardsAreSorted = false;
    private int EmergencyStop = 0;
    private int WhileCounter = 0;

    [Server]
    void Update() {
        // TODO op 10 zetten!!!
        if (!gameOver) {
            if (totalHandsPlayed == 10) {
                var values = connectionScoreManagers.Values;
                foreach(GameObject score in values) {
                    Debug.Log($"Endscore: {score.GetComponent<ScoreManager>().Score}");
                    // TODO op 66 zetten!!!
                    if (score.GetComponent<ScoreManager>().Score >= 66) {
                        Debug.Log("LOSER");
                        gameOver = true;
                        return;
                    }
                }
                var players = FindObjectsOfType<PlayerManager>(); 
                foreach(PlayerManager player in players) {
                    player.cardsDealt = false;
                }
                RowCardsDealt.RowCardsAreDealt = false;
                RpcDestroyCardsInAllRows();
                CardManager.ResetRound();
                if (!RowCardsDealt.RowCardsAreDealt)
                {
                    DealRowCards();
                    RowCardsDealt.RowCardsAreDealt = true;
                }
                CmdGetRowCards();
                totalHandsPlayed = 0;
            }
        } else {
            Debug.Log("FINALLY");
        }
            
    }

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
        clients.Add(connectionToClient);
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        ScoreText.text = $"Score: 0";
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
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
        if (!connectionAdded) 
        {
            GameObject scoreManager = Instantiate(ScoreManager, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(scoreManager, connectionToClient);
            connectionScoreManagers.Add(connectionToClient, scoreManager);
            connectionAdded = true;
        }
        if (!cardsDealt)
        {
            //TODO op 10 zetten!!!
            for (int j = 0; j < 10; j++)
            // for (int j = 0; j < 1; j++)
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
        if (CardManager.CardsPlayedThisRound.Count >= numberOfPlayers) {
            Debug.Log(CardManager.CardsPlayedThisRound.Count);
            EmergencyStop = CardManager.CardsPlayedThisRound.Count;
            WhileCounter = 0;
            while(CardManager.CardsPlayedThisRound.Count > 0 && WhileCounter < EmergencyStop)
            {
                card = CardManager.CardsPlayedThisRound[0];
                CardManager.CardsPlayedThisRound.Remove(card);
                int lowestDiff = 105;
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
                        CardManager.Rows[lowestDiffIndex].GetComponent<RowManager>().ClearRowAndAddCard(card);
                        var value = connectionScoreManagers[card.GetComponent<CardInfo>().connectionToClient];
                        ScoreManager scoreManager = value.GetComponent<ScoreManager>();
                        RpcUpdateScore(card.GetComponent<CardInfo>().connectionToClient, scoreManager, score);
                        scoreManager.Score += score;
                        RpcDestroyCardsInRow($"Row{lowestDiffIndex + 1}");
                    }
                    CardManager.Rows[lowestDiffIndex].GetComponent<RowManager>().AddCardToRow(card);
                }
                else
                {
                    int lowestScore = 105;
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
                    var value = connectionScoreManagers[card.GetComponent<CardInfo>().connectionToClient];
                    ScoreManager scoreManager = value.GetComponent<ScoreManager>();
                    RpcUpdateScore(card.GetComponent<CardInfo>().connectionToClient, scoreManager, lowestScore);
                    scoreManager.Score += lowestScore;
                    RpcDestroyCardsInRow($"Row{lowestScoreIndex+1}");
                }
                WhileCounter++;
            }
            if (EmergencyStop > WhileCounter) {
                Debug.Log("Shit is going down");
            }
            for (int i = 0; i < CardManager.Rows.Count; i++)
            {
                foreach (var obj in CardManager.Rows[i].GetComponent<RowManager>().CardsInRow)
                {
                    RpcPlaceCards(obj, $"Row{i + 1}", obj.GetComponent<CardInfo>().CardNumber);
                }
            }
            totalHandsPlayed++;
        }    
    }

    [ClientRpc]
    void RpcSetAllClientsCardsDealtsStatus(bool status) {
        cardsDealt = status;
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
    void RpcDestroyCardsInAllRows()
    {
        for(int i = 1; i < 5; i++) {
            GameObject row = GameObject.Find($"Row{i}");
            for (int j = 0; j < row.transform.childCount; j++)
            {
                Destroy(row.transform.GetChild(j).gameObject);
            }
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