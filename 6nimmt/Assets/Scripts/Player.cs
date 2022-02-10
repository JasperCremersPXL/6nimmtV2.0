using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    public Cards cards;
    public string Name { get; set; }
    public int Score { get; set; }
    public List<Card> CardsInHand { get; set; }
    public List<Vector3> PlayerCardPositions { get; set; }
    private List<Card> _cardsTaken;
    private int _playerAreaOffsetY = -570;
    private string _layerName = "Foreground";
    private List<GameObject> _cardObjects;


    public Player(string name)
    {
        Name = name;
        Score = 0;
        CardsInHand = new List<Card>();
        _cardsTaken = new List<Card>();
        PlayerCardPositions = new List<Vector3>();
        _cardObjects = new List<GameObject>();
    }

    public void LoadCards()
    {
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            Card current = CardsInHand[i];
            GameObject cardObject = new GameObject(string.Format("{0}", current.CardNumber));
            cardObject.AddComponent<ClickEvent>();
            cardObject.AddComponent<BoxCollider2D>();
            var boxCollider2d = cardObject.GetComponent<BoxCollider2D>();
            boxCollider2d.size = new Vector2(8f, 8f);
            SetCardTexture(current.CardNumber, cardObject);
            _cardObjects.Add(cardObject);
            cardObject.transform.localScale = new Vector3(16.5f, 16.75f, 0f);
            cardObject.transform.localPosition = new Vector3(PlayerCardPositions[i].x, _playerAreaOffsetY, 0);
        }
    }

    public void TakeRow(Row row) 
    {
        _cardsTaken.AddRange(row.CardList);
        row.CardList.Clear();
        Debug.Log($"{Name} heeft nu {_cardsTaken.Count} kaarten geraapt");
    }

    public void UpdateScore()
    {
        if(_cardsTaken.Count == 0) 
        {
            return;
        }
        foreach (var card in _cardsTaken)
        {
            Score += card.AmountTriangles;
        }
        _cardsTaken.Clear();
    }

    public void isDone()
    {
        foreach(var cardObject in _cardObjects)
        {
            GameObject.Destroy(cardObject);
        }
    }


    public void AddCardToHand(Card card)
    {
        CardsInHand.Add(card);
    }

    private void SetCardTexture(int number, GameObject cardObject)
    {
        Texture2D texture = cards.cards[number-1];
        cardObject.AddComponent<SpriteRenderer>();
        cardObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        cardObject.GetComponent<SpriteRenderer>().sortingLayerName = _layerName;
    }
}

