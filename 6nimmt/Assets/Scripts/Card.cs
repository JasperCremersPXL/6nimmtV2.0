using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Card
{
    private int _cardNumber;
    public int CardNumber { get => _cardNumber; }
    private int _amountTriangles;
    public int AmountTriangles { get => _amountTriangles; }

    public Card(int number, int triangles) {
        _cardNumber = number;
        _amountTriangles = triangles;
    }
}
