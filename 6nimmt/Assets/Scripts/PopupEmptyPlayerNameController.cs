using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupEmptyPlayerNameController : MonoBehaviour
{
    public Text title;
    public GameObject inputFields;
    public PopupNotEnoughPlayersController popupNotEnoughPlayers;

    public void SetTitle(int index)
    {
        this.title.text = $"Je hebt naam {index + 1} niet ingevuld. Wat wil je doen?";
    }

    public void OnAdvanceButtonPressed()
    {
        for (int i = 0; i < inputFields.transform.childCount; i++)
        {
            var inputField = inputFields.transform.GetChild(i);
            if (inputField.transform.Find("Text").GetComponent<Text>().text == "" || inputField.transform.Find("Text").GetComponent<Text>().text == string.Empty)
            {
                continue;
            }
            else
            {
                MainMenuController.players.Add(new Player(inputField.transform.Find("Text").GetComponent<Text>().text));
            }
        }
        if (MainMenuController.players.Count < 2)
        {
            gameObject.SetActive(false);
            popupNotEnoughPlayers.gameObject.SetActive(true);
            return;
        }
        SceneManager.LoadScene(0);
    }

    public void OnAgainButtonPressed()
    {
        gameObject.SetActive(false);
    }
}
