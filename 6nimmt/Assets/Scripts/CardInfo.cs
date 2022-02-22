using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    public int CardNumber;

    public static int CalculateCardTriangles(int i)
    {
        if (i == 55)
        {
            return 7;
        }
        else if (i % 11 == 0)
        {
            return 5;
        }
        else if (i % 10 == 0)
        {
            return 3;
        }
        else if (i % 5 == 0)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}
