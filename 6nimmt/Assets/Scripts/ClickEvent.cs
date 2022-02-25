using UnityEngine;

public class ClickEvent : MonoBehaviour
{
    private void OnMouseDown()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        if (ClickUtil.PrevGameObject != null && gameObject != ClickUtil.PrevGameObject)
        {
            ClickUtil.PrevGameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        ClickUtil.PrevGameObject = gameObject;
    }
}
