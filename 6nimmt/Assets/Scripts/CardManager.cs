using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Dictionary<int, GameObject> dealtCards = new Dictionary<int, GameObject>();
    public List<GameObject> CardsPlayedThisRound = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void PlayCard(GameObject card) 
    {
        CardsPlayedThisRound.Add(card);
    }

    public int GetCardNumber(GameObject card) 
    {
        int cardNumber = Random.Range(1,25);
        while(dealtCards.ContainsKey(cardNumber)) 
        {
            cardNumber = Random.Range(1,25);
        }
        dealtCards.Add(cardNumber, card);
        card.GetComponent<CardInfo>().CardNumber = cardNumber;
        return cardNumber;
    }
}
