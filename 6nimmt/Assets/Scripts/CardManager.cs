using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static Dictionary<int, GameObject> dealtCards = new Dictionary<int, GameObject>();
    public List<GameObject> CardsPlayedThisRound = new List<GameObject>();
    public List<GameObject> Rows;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void InstantiateRows() 
    {
        Rows = new List<GameObject>();
        Rows.Add(GameObject.Find("Row1"));
        Rows.Add(GameObject.Find("Row2"));
        Rows.Add(GameObject.Find("Row3"));
        Rows.Add(GameObject.Find("Row4"));
    }

    public void PlayCard(GameObject card) 
    {
        CardsPlayedThisRound.Add(card);
    }

    public int GetCardNumber(GameObject card) 
    {
        int cardNumber = Random.Range(1,30);
        while(dealtCards.ContainsKey(cardNumber)) 
        {
            cardNumber = Random.Range(1,30);
        }
        dealtCards.Add(cardNumber, card);
        card.GetComponent<CardInfo>().CardNumber = cardNumber;
        return cardNumber;
    }

    public void AddCardToRow(GameObject card, int rowIndex)
    {
        Rows[rowIndex].GetComponent<RowManager>().AddCardToRow(card);
    }
}
