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
}
