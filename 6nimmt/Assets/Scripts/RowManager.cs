using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : NetworkBehaviour
{
    public readonly SyncList<GameObject> CardsInRow = new SyncList<GameObject>();

    public bool IsFull
    {
        get { return CardsInRow.Count == 5; }
    }

    public void AddCardToRow(GameObject card) 
    {
        Debug.Log("test");
        CardsInRow.Add(card);
    }

    public int GetDifference(int playedCard)
    {
        return CardsInRow[CardsInRow.Count - 1].GetComponent<CardInfo>().CardNumber > playedCard ? 999999999 : playedCard - CardsInRow[CardsInRow.Count - 1].GetComponent<CardInfo>().CardNumber;
    }
}
