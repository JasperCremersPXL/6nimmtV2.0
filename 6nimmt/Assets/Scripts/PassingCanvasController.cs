using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassingCanvasController : MonoBehaviour
{
    public Text playerName;

    public void RemoveCanvas()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }
}
