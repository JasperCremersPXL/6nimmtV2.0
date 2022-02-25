using UnityEngine;
using Mirror;

public class DragDrop : NetworkBehaviour
{
    public GameObject Canvas;
    public bool IsDraggable = true;

    private bool _isDragging = false;
    private GameObject _startParent;
    private Vector2 _startPosition;
    private GameObject _dropZone;
    private bool _isOverDropZone;

    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        if(!hasAuthority) 
        {
            IsDraggable = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        _isOverDropZone = true;
        _dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        _isOverDropZone = false;
        _dropZone = null;
    }

    public void BeginDrag() 
    {
        if(!IsDraggable) return;
        _isDragging = true;
        _startParent = transform.parent.gameObject;
        _startPosition = transform.position;
        
    }

    public void EndDrag() 
    {
        if(!IsDraggable) return;
        _isDragging = false;
        if(_isOverDropZone && _dropZone.transform.childCount < 1) 
        {
            transform.SetParent(_dropZone.transform, false);
            IsDraggable = false;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.PlayCard(gameObject);
        }
        else 
        {
            transform.position = _startPosition;
            transform.SetParent(_startParent.transform, false);
        }
    }

    void Update()
    {
        if(_isDragging) 
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }
}
