using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject inputFieldPrefab;
    public GameObject inputFields;
    [Header("Popup's")]
    public PopupEmptyPlayerNameController popupEmptyPlayerName;
    public PopupToManyPlayersController popupToManyPlayers;
    public PopupNotEnoughPlayersController popupNotEnoughPlayers;

    public static List<Player> players = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddInputField()
    {
        if (inputFields.transform.childCount < 10)
        {
            var newInputField = GameObject.Instantiate<GameObject>(inputFieldPrefab);
            newInputField.transform.SetParent(inputFields.transform);
            newInputField.transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            popupToManyPlayers.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        for (int i = 0; i < inputFields.transform.childCount; i++)
        {
            var inputField = inputFields.transform.GetChild(i);
            if (inputField.transform.Find("Text").GetComponent<Text>().text == "" || inputField.transform.Find("Text").GetComponent<Text>().text == string.Empty)
            {
                popupEmptyPlayerName.gameObject.SetActive(true);
                popupEmptyPlayerName.SetTitle(i);
                return;
            }
            players.Add(new Player(inputField.transform.Find("Text").GetComponent<Text>().text));
        }
        if (players.Count < 2)
        {
            popupNotEnoughPlayers.gameObject.SetActive(true);
            return;
        }
        SceneManager.LoadScene(1);
    }
}
