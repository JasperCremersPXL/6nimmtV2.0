using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Row
{
    public Cards cards;
    public List<Card> CardList { get; set; }
    public List<Vector3> PlayCardPositionsArea { get; set; }
    public int Offset { get; set; }
    private string _layerName = "Foreground";
    private List<GameObject> _cardObjects;
    public bool IsFull 
    {
        get { return CardList.Count == 5; }
    }


    public Row(int offset, List<Vector3> playCardPositionsArea)
    {
        CardList = new List<Card>();
        Offset = offset;
        PlayCardPositionsArea = playCardPositionsArea;
        _cardObjects = new List<GameObject>();

    }

    public int GetRowScore() 
    {
        int score = 0;
        foreach(var card in CardList)
        {
            score += card.AmountTriangles;
        }
        return score;
    }

    public void ResetRow() 
    {
        ClearUI();
        CardList.Clear();
    }

    public void LoadCards()
    {
        if(_cardObjects.Count != 0)
        {
            ClearUI();
        }
        for (int i = 0; i < CardList.Count; i++)
        {
            Card current = CardList[i];
            GameObject cardObject = new GameObject(string.Format("{0}", current.CardNumber));
            Debug.Log($"Row card number: {current.CardNumber}");
            SetCardTexture(current.CardNumber, cardObject);
            _cardObjects.Add(cardObject);
            cardObject.transform.localScale = new Vector3(16.5f, 16.75f, 0f);
            cardObject.transform.localPosition = new Vector3(PlayCardPositionsArea[i].x, Offset, 0);
        }
    }
    public void AddCardToCardList(Card card)
    {
            CardList.Add(card);
    }

    public void ClearUI() 
    {
        foreach (var cardObject in _cardObjects) 
        {
            GameObject.Destroy(cardObject);
        }
    }

    public int GetDifference(int playedCard)
    {
        return CardList[CardList.Count - 1].CardNumber > playedCard ? 110 : playedCard - CardList[CardList.Count - 1].CardNumber;
    }

    private void SetCardTexture(int number, GameObject cardObject)
    {
        Texture2D texture = cards.cards[number-1];
        cardObject.AddComponent<SpriteRenderer>();
        cardObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        cardObject.GetComponent<SpriteRenderer>().sortingLayerName = _layerName;
    }
    
}

