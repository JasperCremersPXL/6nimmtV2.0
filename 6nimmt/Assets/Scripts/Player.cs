using Assets.Scripts.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    public CardGameObject cardGameObject;
    public Cards cards;
    public string Name { get; set; }
    public int Score { get; set; }
    public List<Card> CardsInHand { get; set; }
    public List<Vector3> PlayerCardPositions { get; set; }
    private List<Card> _cardsTaken;
    private int _playerAreaOffsetY = -570;
    private string _layerName = "Foreground";
    private List<CardGameObject> _cardObjects;


    public Player(string name)
    {
        Name = name;
        Score = 0;
        CardsInHand = new List<Card>();
        _cardsTaken = new List<Card>();
        PlayerCardPositions = new List<Vector3>();
        _cardObjects = new List<CardGameObject>();
    }

    public void LoadCards()
    {
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            Card current = CardsInHand[i];
            CardGameObject cardObject = GameObject.Instantiate<CardGameObject>(cardGameObject);
            cardObject.name = string.Format("{0}", current.CardNumber);

            var boxCollider2d = cardObject.GetComponent<BoxCollider2D>();
            boxCollider2d.size = new Vector2(8f, 8f);
            SetCardTexture(current.CardNumber-1, cardObject);
            _cardObjects.Add(cardObject);
            cardObject.transform.localScale = new Vector3(16.5f, 16.75f, 0f);
        //     // TODO: animation
        //     Debug.Log("new animation");
        //     AnimationsController.AddAnimation(new LocalPositionAnimation(cardObject, new Vector3(15,15,15), new Vector3(PlayerCardPositions[i].x, _playerAreaOffsetY, 0)));
            cardObject.transform.localPosition = new Vector3(PlayerCardPositions[i].x, _playerAreaOffsetY, 0);
        }
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

    private void SetCardTexture(int number, CardGameObject cardObject)
    {
        Texture2D texture = cards.cards[number];
        cardObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        cardObject.GetComponent<SpriteRenderer>().sortingLayerName = _layerName;
    }
}

