using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int amountPlayers;
    public GameObject players;
    public GameObject cardsRows;
    public Cards cards;
    public GameObject playerArea;
    public GameObject playArea;
    public GameObject playArea1;
    public GameObject playArea2;
    public GameObject playArea3;
    private List<Card> _deck;
    private List<Card> _dealtCards;
    private List<Vector3> _startPositionsPlayerArea;
    private int _playerAreaOffsetY = -570;
    private List<Vector3> _startPositionsPlayArea;
    private int _playAreaOffsetY = -220;
    private List<Vector3> _startPositionsPlayArea1;
    private int _playArea1OffsetY = 30;
    private List<Vector3> _startPositionsPlayArea2;
    private int _playArea2OffsetY = 280;
    private List<Vector3> _startPositionsPlayArea3;
    private int _playArea3OffsetY = 530;
    private string _layerName = "Foreground";
    private bool _isLayoutReady = false;
    private bool _isHandDealt = false;
    private bool _isPlayerCard = true;
    private GameObject _cardRowObject;
    private void Awake()
    {
        // instaniate lists
        _deck = new List<Card>();
        _dealtCards = new List<Card>();
        _startPositionsPlayerArea = new List<Vector3>();
        _startPositionsPlayArea = new List<Vector3>();
        _startPositionsPlayArea1 = new List<Vector3>();
        _startPositionsPlayArea2 = new List<Vector3>();
        _startPositionsPlayArea3 = new List<Vector3>();

        // check amount of players
        if (amountPlayers > 10)
            amountPlayers = 10;
        if (amountPlayers < 2) 
            amountPlayers = 2; 
    }
    private void Start()
    {
        Debug.Log(MainMenuController.players.Count);
        // create deck
        _deck = Deck.CreateDeck();

        // instantiate player and pile objects
        GameObject playerObject;
        GameObject cardPilePlayer;
        for (int i = 0; i < amountPlayers; i++)
        {
            playerObject = new GameObject(string.Format("Player_{0}", i));
            playerObject.transform.parent = players.transform;
            cardPilePlayer  = new GameObject(string.Format("CardPilePlayer_{0}", i));
            cardPilePlayer.transform.parent = playerObject.transform;
        }

        // instantiate row objects
        for (int i = 0; i < 4; i++) {
            GameObject cardRowObject = new GameObject(string.Format("CardRow_{0}", i));
            cardRowObject.transform.parent = cardsRows.transform;
        }

    }

    private void Update() {
        // wait for canvas to load
        if (!_isLayoutReady) {
            if (playerArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x != 0) {
                // get x localPosition of playerarea
                GetPlayerAreaStartPositions();

                // get x localPosition of playareas
                GetPlayAreaStartPositions();

                // stop if
                _isLayoutReady = true;
            }
        }
        if (_isLayoutReady && !_isHandDealt) {
            DealCards();

            // stop if
            _isHandDealt = true;
        }
    }

    private void DealCards()
    {
        System.Random random = new System.Random();
        int randomNumber;
        Card tempCard;
        for (int i = 0; i < 10; i++)
        {
            foreach (Transform player in players.transform)
            {
                randomNumber = random.Next(_deck.Count);
                tempCard = _deck[randomNumber];
                while (_dealtCards.Contains(tempCard))
                {
                    randomNumber = random.Next(_deck.Count - 1);
                    tempCard = _deck[randomNumber];
                }
                GameObject cardObject;
                if (_isPlayerCard) {
                    cardObject = new GameObject(string.Format("{0}", tempCard.CardNumber));
                    SetUpCardObject(randomNumber, cardObject, tempCard, player, _startPositionsPlayerArea, i, _playerAreaOffsetY);
                    _isPlayerCard = false;
                } else {
                    cardObject = new GameObject(string.Format("{0}", tempCard.CardNumber));
                    cardObject.transform.parent = player.transform;
                    _dealtCards.Add(tempCard);
                }
            }
            _isPlayerCard = true;
        }
        for (int i = 0; i < 4; i++) {
            randomNumber = random.Next(_deck.Count - 1);
            tempCard = _deck[randomNumber];
            while (_dealtCards.Contains(tempCard))
            {
                randomNumber = random.Next(_deck.Count - 1);
                tempCard = _deck[randomNumber];
            }
            GameObject cardObject = new GameObject(string.Format("{0}", tempCard.CardNumber));
            GameObject cardRow;
            if (i == 0) {
                cardRow = GameObject.Find(string.Format("CardRow_{0}", i));
                SetUpCardObject(randomNumber, cardObject, tempCard, cardRow.transform, _startPositionsPlayArea, 0, _playAreaOffsetY);
            }
            if (i == 1) {
                cardRow = GameObject.Find(string.Format("CardRow_{0}", i));
                SetUpCardObject(randomNumber, cardObject, tempCard, cardRow.transform, _startPositionsPlayArea1, 0, _playArea1OffsetY);
            }
            if (i == 2) {
                cardRow = GameObject.Find(string.Format("CardRow_{0}", i));
                SetUpCardObject(randomNumber, cardObject, tempCard, cardRow.transform, _startPositionsPlayArea2, 0 , _playArea2OffsetY);
            }
            if (i == 3) {
                cardRow = GameObject.Find(string.Format("CardRow_{0}", i));
                SetUpCardObject(randomNumber, cardObject, tempCard, cardRow.transform, _startPositionsPlayArea3, 0, _playArea3OffsetY);
            }
        }
    }

    private void SetUpCardObject(int randomNumber, GameObject cardObject, Card tempCard, Transform parent, List<Vector3> positions, int index,  int offset) {
        cardObject.transform.parent = parent.transform;
        SetCardTexture(randomNumber, cardObject);
        cardObject.transform.localScale = new Vector3(16.5f, 16.75f, 0f);
        cardObject.transform.localPosition = new Vector3(positions[index].x, offset, 0);
        _dealtCards.Add(tempCard);
    }

    private void GetPlayerAreaStartPositions()
    {
        foreach (Transform image in playerArea.transform)
        {
            RectTransform rt = image.GetComponent<RectTransform>();
            _startPositionsPlayerArea.Add(rt.localPosition);
        }
    }

    private void GetPlayAreaStartPositions() {
        for(int i = 0; i < 5; i++) {
            RectTransform rt = playArea.transform.GetChild(i).GetComponent<RectTransform>();
            _startPositionsPlayArea.Add(rt.localPosition);
            RectTransform rt1 = playArea1.transform.GetChild(i).GetComponent<RectTransform>();
            _startPositionsPlayArea1.Add(rt1.localPosition);
            RectTransform rt2 = playArea2.transform.GetChild(i).GetComponent<RectTransform>();
            _startPositionsPlayArea2.Add(rt2.localPosition);
            RectTransform rt3 = playArea3.transform.GetChild(i).GetComponent<RectTransform>();
            _startPositionsPlayArea3.Add(rt3.localPosition);
        }
    }

    private void SetCardTexture(int randomNumber, GameObject cardObject) {
        Texture2D texture = cards.cards[randomNumber];
        cardObject.AddComponent<SpriteRenderer>();
        cardObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        cardObject.GetComponent<SpriteRenderer>().sortingLayerName = _layerName;
    }
}