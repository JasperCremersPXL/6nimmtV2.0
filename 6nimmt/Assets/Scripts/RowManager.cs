using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public List<GameObject> CardsInRow = new List<GameObject>();

    public void AddCardToRow(GameObject card) 
    {
        CardsInRow.Add(card);
    }

    public int GetDifference(int playedCard)
    {
        return CardsInRow[CardsInRow.Count - 1].GetComponent<CardInfo>().CardNumber > playedCard ? 999999999 : playedCard - CardsInRow[CardsInRow.Count - 1].GetComponent<CardInfo>().CardNumber;
    }
}