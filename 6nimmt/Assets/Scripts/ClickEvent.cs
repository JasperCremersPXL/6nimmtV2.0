using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEvent : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Object was clicked!");
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        if (ClickUtil.PrevGameObject != null && gameObject != ClickUtil.PrevGameObject)
        {
            ClickUtil.PrevGameObject.GetComponent<SpriteRenderer>().color = Color.white;
            Debug.Log("If statement reached!");
        }
        ClickUtil.PrevGameObject = gameObject;
    }
}
