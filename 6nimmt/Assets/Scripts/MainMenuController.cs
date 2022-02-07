using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject inputFieldPrefab;
    public GameObject inputFields;

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
        }
    }

    public void StartGame()
    {
        for (int i = 0; i < inputFields.transform.childCount; i++)
        {
            var inputField = inputFields.transform.GetChild(i);
            players.Add(new Player(inputField.transform.Find("Text").GetComponent<Text>().text));
        }
        SceneManager.LoadScene(0);
    }
}
