using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static Dictionary<int, GameObject> dealtCards = new Dictionary<int, GameObject>();
    public static List<GameObject> CardsPlayedThisRound = new List<GameObject>();
    public static List<GameObject> Rows;

    public void ResetRound()
    {
        dealtCards.Clear();
        foreach(var row in Rows) {
            row.GetComponent<RowManager>().CardsInRow = new List<GameObject>();
        }
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
