using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror; 

public class PlayerManager : NetworkBehaviour
{
    public Button drawCardButton;
    public GameObject playArea;
    public GameObject playArea1;
    public GameObject playArea2;
    public GameObject playArea3;
    public GameObject cardGameObject;
    public Cards cards;
    private List<Card> _deck;
    private List<Card> _dealtCards;
    public readonly SyncList<Row> rows = new SyncList<Row>();

    [Client]
    public override void OnStartClient() {
        base.OnStartClient();

        playArea = GameObject.Find("PlayArea");
        playArea1 = GameObject.Find("PlayArea (1)");
        playArea2 = GameObject.Find("PlayArea (2)");
        playArea3 = GameObject.Find("PlayArea (3)");

        cards = Resources.Load<Cards>("ScriptableObjects/Cards");
        cardGameObject = Resources.Load<GameObject>("CardGameObject/CardGameObject");

        rows.Callback += OnRowsChanged;
    }

    [Server]
    public override void OnStartServer() {
        base.OnStartServer();

        drawCardButton = GameObject.Find("DrawCardsButton").GetComponent<Button>();
        drawCardButton.gameObject.SetActive(true);

        playArea = GameObject.Find("PlayArea");
        playArea1 = GameObject.Find("PlayArea (1)");
        playArea2 = GameObject.Find("PlayArea (2)");
        playArea3 = GameObject.Find("PlayArea (3)");

        cards = Resources.Load<Cards>("ScriptableObjects/Cards");
        cardGameObject = Resources.Load<GameObject>("CardGameObject/CardGameObject");

        _deck = Deck.CreateDeck();
        _dealtCards = new List<Card>();

        // drawCardButton
        drawCardButton.onClick.AddListener(CmdDealCards);
    }

    [Command]
    public void CmdDealCards() {
        GetPlayAreaStartPositions();
        DealRowCards();
        foreach(var row in rows)
        {
            row.LoadCards();
        }
        RpcDealCards();
    }

    [ClientRpc]
    public void RpcDealCards() {
        Debug.Log("test");
        Debug.Log(rows.Count);
        foreach(var row in rows) {
            Debug.Log(row);
            row.LoadCards();
        }
     }

    private void DealRowCards()
    {
        System.Random random = new System.Random();
        int randomNumber;
        Card tempCard;

        for (int i = 0; i < rows.Count; i++)
        {
            randomNumber = random.Next(_deck.Count);
            tempCard = _deck[randomNumber];

            while (_dealtCards.Contains(tempCard))
            {
                randomNumber = random.Next(_deck.Count);
                tempCard = _deck[randomNumber];
            }
            rows[i].AddCardToCardList(tempCard);
            _dealtCards.Add(tempCard);
        }
    }

    private void GetPlayAreaStartPositions()
    {
        List<Vector3> startPositionsPlayArea1 = new List<Vector3>();
        List<Vector3> startPositionsPlayArea2 = new List<Vector3>();
        List<Vector3> startPositionsPlayArea3 = new List<Vector3>();
        List<Vector3> startPositionsPlayArea4 = new List<Vector3>();
        for (int i = 0; i < 5; i++)
        {
            RectTransform rt = playArea.transform.GetChild(i).GetComponent<RectTransform>();
            startPositionsPlayArea1.Add(rt.localPosition);
            RectTransform rt1 = playArea1.transform.GetChild(i).GetComponent<RectTransform>();
            startPositionsPlayArea2.Add(rt1.localPosition);
            RectTransform rt2 = playArea2.transform.GetChild(i).GetComponent<RectTransform>();
            startPositionsPlayArea3.Add(rt2.localPosition);
            RectTransform rt3 = playArea3.transform.GetChild(i).GetComponent<RectTransform>();
            startPositionsPlayArea4.Add(rt3.localPosition);
        }

        Row row1 = new Row(-220, startPositionsPlayArea1);
        Row row2 = new Row(30, startPositionsPlayArea2);
        Row row3 = new Row(280, startPositionsPlayArea3);
        Row row4 = new Row(530, startPositionsPlayArea4);

        rows.Add(row1);
        rows.Add(row2);
        rows.Add(row3);
        rows.Add(row4);

        foreach (var row in rows)
        {
            row.cards = cards;
            row.cardGameObject = cardGameObject;
        }
    }
}
