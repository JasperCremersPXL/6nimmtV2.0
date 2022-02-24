using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        SortCards();
    }
    
    public void SortCards() {
        CardsPlayedThisRound = CardsPlayedThisRound.OrderBy(Card => Card.GetComponent<CardInfo>().CardNumber).ToList();

        bool sort = false;
        for(int i = 0; i < CardsPlayedThisRound.Count - 1; i++) {
            if (CardsPlayedThisRound[i].GetComponent<CardInfo>().CardNumber > CardsPlayedThisRound[i + 1].GetComponent<CardInfo>().CardNumber) {
                sort = true;
                break;
            }
        }
        Debug.Log("sort value");
        Debug.Log(sort);
        if (sort) CardsPlayedThisRound = CardsPlayedThisRound.OrderBy(Card => Card.GetComponent<CardInfo>().CardNumber).ToList();
        
        foreach(GameObject card in CardsPlayedThisRound) {
            Debug.Log(card.GetComponent<CardInfo>().CardNumber);
        }
    }
    public int GetCardNumber(GameObject card) 
    {
        int cardNumber = Random.Range(1,105);
        while(dealtCards.ContainsKey(cardNumber)) 
        {
            cardNumber = Random.Range(1,105);
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
