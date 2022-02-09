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


    public Row(int offset, List<Vector3> playCardPositionsArea)
    {
        CardList = new List<Card>();
        Offset = offset;
        PlayCardPositionsArea = playCardPositionsArea;
        _cardObjects = new List<GameObject>();

    }

    public void LoadCards()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            Card current = CardList[i];
            GameObject cardObject = new GameObject(string.Format("{0}", current.CardNumber));
            
            SetCardTexture(current.CardNumber - 1, cardObject);
            _cardObjects.Add(cardObject);
            cardObject.transform.localScale = new Vector3(16.5f, 16.75f, 0f);
            cardObject.transform.localPosition = new Vector3(PlayCardPositionsArea[i].x, Offset, 0);
        }
    }
    public void AddCardToCardList(Card card)
    {
        CardList.Add(card);
    }

    private void SetCardTexture(int number, GameObject cardObject)
    {
        Texture2D texture = cards.cards[number];
        cardObject.AddComponent<SpriteRenderer>();
        cardObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        cardObject.GetComponent<SpriteRenderer>().sortingLayerName = _layerName;
    }
    
}

