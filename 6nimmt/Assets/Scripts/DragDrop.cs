using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{
    public GameObject Canvas;
    public bool isDraggable = true;

    private bool isDragging = false;
    private GameObject startParent;
    private Vector2 startPosition;
    private GameObject dropZone;
    private bool isOverDropZone;

    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void BeginDrag() 
    {
        if(!isDraggable) return;
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        
    }

    public void EndDrag() 
    {
        if(!isDraggable) return;
        isDragging = false;
        if(isOverDropZone) 
        {
            if(dropZone.transform.childCount == 1) 
            {
                Transform currentSelected = dropZone.transform.GetChild(0);
                currentSelected.position = startPosition;
                currentSelected.SetParent(startParent.transform, false);
            }
            transform.SetParent(dropZone.transform, false);
        }
        else 
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    void Update()
    {
        if(isDragging) 
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }
}
