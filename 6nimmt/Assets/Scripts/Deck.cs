
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Deck
{
    public static List<Card> CreateDeck() {
        List<Card> _tempDeck = new List<Card>();
        Card _tempCard;
        for(int i = 1; i < 105; i++) {
            if (i==55) {                
                _tempCard = CreateCard(i, 7);
                _tempDeck.Add(_tempCard);
            }
            else if (i % 11 == 0) {
                _tempCard = CreateCard(i, 5);
                _tempDeck.Add(_tempCard);
            }
            else if (i % 10 == 0) {
                _tempCard = CreateCard(i, 3);
                _tempDeck.Add(_tempCard);
            }
            else if (i % 5 == 0) {
                _tempCard = CreateCard(i, 2);
                _tempDeck.Add(_tempCard);
            }
            else {
                _tempCard = CreateCard(i, 1);
                _tempDeck.Add(_tempCard);
            }
        }
        return _tempDeck;
    }

    private static Card CreateCard(int number, int triangles) {
        return new Card(number, triangles);
    }
}