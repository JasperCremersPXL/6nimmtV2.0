using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class DragDrop : NetworkBehaviour
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
        if(!hasAuthority) 
        {
            isDraggable = false;
        }
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
        if(isOverDropZone && dropZone.transform.childCount < 1) 
        {
            transform.SetParent(dropZone.transform, false);
            isDraggable = false;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.PlayCard(gameObject);
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
