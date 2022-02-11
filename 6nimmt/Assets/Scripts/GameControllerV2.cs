// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UI;

// namespace Assets.Scripts
// {
//     public class GameControllerV2 : MonoBehaviour
//     {
//         public Text activePlayerText;
//         public Cards cards;
//         public List<Player> playerList;
//         public List<Row> rows;
//         public GameObject playerArea;
//         public GameObject playArea;
//         public GameObject playArea1;
//         public GameObject playArea2;
//         public GameObject playArea3;
//         public GameObject players;
//         public PassingCanvasController passingCanvas;

//         private List<Card> _dealtCards;
//         private List<Card> _deck;
//         private List<Card> _roundPlayedCards;
//         private int turnCount;
//         private Player currentPlayer;
//         private int currentPlayerIndex;
//         private bool _isLayoutReady;
//         private bool _isHandDealt;

//         private void Start()
//         {
//             //playerList = MainMenuController.players;
//             _dealtCards = new List<Card>();
//             playerList = new List<Player>();
//             rows = new List<Row>();
//             playerList.Add(new Player("Test1"));
//             playerList.Add(new Player("Test2"));
//             _deck = Deck.CreateDeck();
//             _roundPlayedCards = new List<Card>();
//             _isLayoutReady = false;
//             _isHandDealt = false;
//             activePlayerText.text = playerList[0].Name;
//         }

//         private void Update()
//         {
//             if (!_isLayoutReady)
//             {
//                 if (playerArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x != 0)
//                 {
//                     // get x localPosition of playerarea
//                     GetPlayerAreaStartPositions();

//                     GetPlayAreaStartPositions();
//                     // stop if
//                     _isLayoutReady = true;
//                 }
//             }
//             else if (_isLayoutReady && !_isHandDealt)
//             {
//                 ResetRound();

//                 // stop if
//                 _isHandDealt = true;
//             }
//             else if (Input.GetKeyDown(KeyCode.N))
//             {
//                 if (currentPlayerIndex == playerList.Count)
//                 {
//                     currentPlayerIndex = 0;
//                     turnCount++;
//                     //Kaarten aan rijen toevoegen
//                     if (turnCount == 10)
//                     {
//                         ResetRound();
//                     }
//                 }
//                 else
//                 {
//                     currentPlayerIndex++;
//                     currentPlayer.isDone();
//                     currentPlayer = playerList[currentPlayerIndex % playerList.Count];
//                     currentPlayer.LoadCards();
//                     activePlayerText.text = currentPlayer.Name;
//                 }
//             }
//         }

//         public void OnNextButtonPressed()
//         {
            
            
//             if (currentPlayerIndex == playerList.Count)
//             {
//                 Debug.Log("Playing cards");
//                 currentPlayerIndex = 0;
//                 turnCount++;
//                 //Kaarten aan rijen toevoegen
//                 if (turnCount == 10)
//                 {
//                     ResetRound();
//                 }
//             }
//             else
//             {
//                 passingCanvas.gameObject.SetActive(true);
//                 currentPlayerIndex++;
//                 currentPlayer.isDone();
//                 currentPlayer = playerList[currentPlayerIndex % playerList.Count];
//                 currentPlayer.LoadCards();
//                 activePlayerText.text = currentPlayer.Name;
//                 passingCanvas.SetPlayerName(currentPlayer.Name);
//             }
//         }

//         private void ResetRound()
//         {
//             UpdatePlayerScores();
//             if (!GameOver())
//             {
//                 _dealtCards.Clear();
//                 DealPlayerCards();
//                 DealRowCards();
//                 turnCount = 0;
//                 currentPlayerIndex = 0;
//                 currentPlayer = playerList[currentPlayerIndex];
//                 currentPlayer.LoadCards();
//                 foreach(var row in rows)
//                 {
//                     row.LoadCards();
//                 }
//             }
//             else
//             {
//                 EndGame();
//             }
//         }

//         private void DealRowCards()
//         {
//             System.Random random = new System.Random();
//             int randomNumber;
//             Card tempCard;

//             for (int i = 0; i < rows.Count; i++)
//             {
//                 randomNumber = random.Next(_deck.Count);
//                 tempCard = _deck[randomNumber];

//                 while (_dealtCards.Contains(tempCard))
//                 {
//                     randomNumber = random.Next(_deck.Count);
//                     tempCard = _deck[randomNumber];
//                 }
//                 rows[i].AddCardToCardList(tempCard);
//                 _dealtCards.Add(tempCard);
//             }
//         }

//         private void EndGame()
//         {
//             throw new NotImplementedException();
//         }

//         public void UpdatePlayerScores()
//         {
//             foreach (var player in playerList)
//             {
//                 player.UpdateScore();
//             }
//         }

//         public bool GameOver()
//         {
//             foreach (var player in playerList)
//             {
//                 if (player.Score > 66)
//                 {
//                     return true;
//                 }
//             }
//             return false;
//         }

//         private void DoRound()
//         {
//             Player player = playerList[0];
//             for (int i = 0; i < 10; i++)
//             {
//                 DoTurn(player);
//             }
//         }

//         private void DoTurn(Player player)
//         {
//             throw new NotImplementedException();
//         }

//         private void DealPlayerCards()
//         {
//             System.Random random = new System.Random();
//             int randomNumber;
//             Card tempCard;
//             for (int i = 0; i < 10; i++)
//             {
//                 for (int j = 0; j < playerList.Count; j++)
//                 {
//                     randomNumber = random.Next(_deck.Count);
//                     tempCard = _deck[randomNumber];

//                     while (_dealtCards.Contains(tempCard))
//                     {
//                         randomNumber = random.Next(_deck.Count);
//                         tempCard = _deck[randomNumber];
//                     }
//                     playerList[j].AddCardToHand(tempCard);
//                     _dealtCards.Add(tempCard);
//                 }
//             }
//         }

//         private void GetPlayerAreaStartPositions()
//         {
//             List<Vector3> startPositionsPlayerArea = new List<Vector3>();
//             foreach (Transform image in playerArea.transform)
//             {
//                 RectTransform rt = image.GetComponent<RectTransform>();
//                 startPositionsPlayerArea.Add(rt.localPosition);
//             }

//             foreach (var player in playerList)
//             {
//                 player.PlayerCardPositions = startPositionsPlayerArea;
//                 player.cards = cards;
//             }
//         }

//         private void GetPlayAreaStartPositions()
//         {
//             List<Vector3> startPositionsPlayArea1 = new List<Vector3>();
//             List<Vector3> startPositionsPlayArea2 = new List<Vector3>();
//             List<Vector3> startPositionsPlayArea3 = new List<Vector3>();
//             List<Vector3> startPositionsPlayArea4 = new List<Vector3>();
//             for (int i = 0; i < 5; i++)
//             {
//                 RectTransform rt = playArea.transform.GetChild(i).GetComponent<RectTransform>();
//                 startPositionsPlayArea1.Add(rt.localPosition);
//                 RectTransform rt1 = playArea1.transform.GetChild(i).GetComponent<RectTransform>();
//                 startPositionsPlayArea2.Add(rt1.localPosition);
//                 RectTransform rt2 = playArea2.transform.GetChild(i).GetComponent<RectTransform>();
//                 startPositionsPlayArea3.Add(rt2.localPosition);
//                 RectTransform rt3 = playArea3.transform.GetChild(i).GetComponent<RectTransform>();
//                 startPositionsPlayArea4.Add(rt3.localPosition);
//             }

//             Row row1 = new Row(-220, startPositionsPlayArea1);
//             Row row2 = new Row(30, startPositionsPlayArea2);
//             Row row3 = new Row(280, startPositionsPlayArea3);
//             Row row4 = new Row(530, startPositionsPlayArea4);

//             rows.Add(row1);
//             rows.Add(row2);
//             rows.Add(row3);
//             rows.Add(row4);

//             foreach (var row in rows)
//             {
//                 row.cards = cards;
//             }
//         }
//     }
// }
