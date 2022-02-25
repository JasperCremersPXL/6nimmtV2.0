using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public List<GameObject> CardsInRow = new List<GameObject>();

    public bool IsFull
    {
        get { return CardsInRow.Count == 5; }
    }

    public void AddCardToRow(GameObject card) 
    {
        CardsInRow.Add(card);
    }

    public void ClearRowAndAddCard(GameObject card)
    {
        CardsInRow.Clear();
        CardsInRow.Add(card);
    }

    public int GetDifference(int playedCard)
    {
        return CardsInRow[CardsInRow.Count - 1].GetComponent<CardInfo>().CardNumber > playedCard ? 150 : playedCard - CardsInRow[CardsInRow.Count - 1].GetComponent<CardInfo>().CardNumber;
    }

    public int GetRowScore()
    {
        int score = 0;
        foreach (var card in CardsInRow)
        {
            score += CardInfo.CalculateCardTriangles(card.GetComponent<CardInfo>().CardNumber);
        }
        return score;
    }
}
