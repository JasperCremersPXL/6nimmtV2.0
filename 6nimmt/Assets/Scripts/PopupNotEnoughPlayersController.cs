using UnityEngine;

public class PopupNotEnoughPlayersController : MonoBehaviour
{
    public void OnAgainButtonPressed()
    {
        gameObject.SetActive(false);
    }
}
