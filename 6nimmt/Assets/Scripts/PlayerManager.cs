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
    public Text ScoreText;
    public GameObject ScoreManager;
    public bool CardsDealt = false;
    public GameObject GameOverPanel;
    private static int _numberOfPlayers = 0;
    private static List<NetworkConnection> _clients = new List<NetworkConnection>();
    private static Dictionary<NetworkConnection, GameObject> _connectionScoreManagers = new Dictionary<NetworkConnection, GameObject>();
    private static int _totalHandsPlayed;
    private bool _gameOver = false;
    private bool _connectionAdded = false;
    private int _emergencyStop = 0;
    private int _whileCounter = 0;

    [Server]
    void Update() {
        // TODO op 10 zetten!!!
        if (!_gameOver) {
            if (_totalHandsPlayed == 10) {
                var values = _connectionScoreManagers.Values;
                foreach(GameObject score in values) {
                    Debug.Log($"Endscore: {score.GetComponent<ScoreManager>().Score}");
                    // TODO op 66 zetten!!!
                    if (score.GetComponent<ScoreManager>().Score >= 66) {
                        _gameOver = true;
                        RpcActivateGameOverPanel();
                        return;
                    }
                }
                var players = FindObjectsOfType<PlayerManager>(); 
                foreach(PlayerManager player in players) {
                    player.CardsDealt = false;
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
                _totalHandsPlayed = 0;
            }
        } 
    }

    [Client]
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
        _numberOfPlayers++;
        _clients.Add(connectionToClient);
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
        if (!_connectionAdded) 
        {
            GameObject scoreManager = Instantiate(ScoreManager, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.Spawn(scoreManager, connectionToClient);
            _connectionScoreManagers.Add(connectionToClient, scoreManager);
            _connectionAdded = true;
        }
        if (!CardsDealt)
        {
            //TODO op 10 zetten!!!
            for (int j = 0; j < 10; j++)
            // for (int j = 0; j < 1; j++)
            {
                GameObject card = Instantiate(Card, new Vector2(0, 0), Quaternion.identity);
                int cardNumber = CardManager.GetCardNumber(card);
                card.GetComponent<CardInfo>().ConnectionToClient = connectionToClient;
                NetworkServer.Spawn(card, connectionToClient);
                RpcShowCards(card, cardNumber, "dealt", -1);
            }
            CardsDealt = true;
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
        if (CardManager.CardsPlayedThisRound.Count >= _numberOfPlayers) {
            // Debug.Log(CardManager.CardsPlayedThisRound.Count);
            _emergencyStop = CardManager.CardsPlayedThisRound.Count;
            _whileCounter = 0;
            while(CardManager.CardsPlayedThisRound.Count > 0 && _whileCounter < _emergencyStop)
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
                        var value = _connectionScoreManagers[card.GetComponent<CardInfo>().ConnectionToClient];
                        ScoreManager scoreManager = value.GetComponent<ScoreManager>();
                        RpcUpdateScore(card.GetComponent<CardInfo>().ConnectionToClient, scoreManager, score);
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
                    var key = card.GetComponent<CardInfo>().ConnectionToClient;
                    Debug.Log($"player {key} had to take row {lowestScoreIndex + 1}");
                    CardManager.Rows[lowestScoreIndex].GetComponent<RowManager>().ClearRowAndAddCard(card);
                    var value = _connectionScoreManagers[key];
                    ScoreManager scoreManager = value.GetComponent<ScoreManager>();
                    RpcUpdateScore(key, scoreManager, lowestScore);
                    scoreManager.Score += lowestScore;
                    RpcDestroyCardsInRow($"Row{lowestScoreIndex+1}");
                }
                _whileCounter++;
            }
            if (_emergencyStop > _whileCounter) {
                // Fail safe voor de bug dat er een kaart blijft liggen in de dropzone van een speler en het spel onspeelbaar wordt
                // Bug kan al opgelost zijn... We don't know ¯\_(ツ)_/¯
                Debug.Log("Emergency stop triggered!!!");
            }
            for (int i = 0; i < CardManager.Rows.Count; i++)
            {
                foreach (var obj in CardManager.Rows[i].GetComponent<RowManager>().CardsInRow)
                {
                    RpcPlaceCards(obj, $"Row{i + 1}", obj.GetComponent<CardInfo>().CardNumber);
                }
            }
            _totalHandsPlayed++;
        }    
    }

    [ClientRpc]
    void RpcSetAllClientsCardsDealtsStatus(bool status) {
        CardsDealt = status;
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
        card.GetComponent<DragDrop>().IsDraggable = false;
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
                card.GetComponent<DragDrop>().IsDraggable = false;
            }
        }
        else if (type == "played")
        {
            card.transform.SetParent(Rows[rowIndex].transform, false);
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/{cardNumber}");
            card.GetComponent<DragDrop>().IsDraggable = false;
        }
    }

    [ClientRpc]
    void RpcDisableCards()
    {
        if (hasAuthority)
        {
            foreach (GameObject card in CardsInHand)
            {
                card.GetComponent<DragDrop>().IsDraggable = false;
            }
        }
    }

    [ClientRpc]
    void RpcActivateGameOverPanel() {
        //GameOverPanel.SetActive(true);
        GameObject mainCanvas = GameObject.Find("Main Canvas");
        GameObject gameOverPanel = Instantiate(GameOverPanel);
        gameOverPanel.transform.SetParent(mainCanvas.transform);
        RectTransform gameOverPanelRectTransform = gameOverPanel.GetComponent<RectTransform>();
        gameOverPanelRectTransform.anchorMin = Vector2.zero;
        gameOverPanelRectTransform.anchorMax = new Vector2(1,1);
        gameOverPanelRectTransform.offsetMin = Vector2.zero;
        gameOverPanelRectTransform.offsetMax = Vector2.zero;

    }
}