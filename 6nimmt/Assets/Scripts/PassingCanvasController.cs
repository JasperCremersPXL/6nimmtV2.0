using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassingCanvasController : MonoBehaviour
{
    public Text playerName;
    public AnimationsController animationsController;
    public Image background;

    public void AddCanvas()
    {

        gameObject.SetActive(true);
        animationsController.AddAnimation(new PositionAnimation(
                background.gameObject,
                new Vector3(Screen.width, Screen.height / 2, 0),
                new Vector3(Screen.width / 2, Screen.height / 2, 0),
                1f
            ));
    }

    public void RemoveCanvas()
    {
        StartCoroutine(RemoveCanvasCoroutine());
    }

    IEnumerator RemoveCanvasCoroutine()
    {
        float animationDuration = 1f;
        animationsController.AddAnimation(new PositionAnimation(
                background.gameObject,
                gameObject.transform.position,
                new Vector3(-Screen.width / 2, Screen.height / 2, 0),
                animationDuration
            ));
        yield return new WaitForSeconds(animationDuration);
        gameObject.SetActive(false);
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }
}
