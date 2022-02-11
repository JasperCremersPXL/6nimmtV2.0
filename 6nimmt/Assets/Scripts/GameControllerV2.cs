using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameControllerV2 : MonoBehaviour
    {
        public Text activePlayerText;
        public Text activePlayerScore;
        public Text endScoreText;
        public Cards cards;
        public List<Player> playerList;
        public List<Row> rows;
        public GameObject playerArea;
        public GameObject playArea;
        public GameObject playArea1;
        public GameObject playArea2;
        public GameObject playArea3;
        public GameObject players;
        public PassingCanvasController passingCanvas;
        public Canvas playAgainCanvas;

        private List<Card> _dealtCards;
        private List<Card> _deck;
        private Dictionary<int, Player> _roundPlayedCards;
        private int turnCount;
        private Player currentPlayer;
        private int currentPlayerIndex;
        private bool _isLayoutReady;
        private bool _isHandDealt;
        private bool _isPassing = false;

        private void Start()
        {
            
            //playerList = MainMenuController.players;
            playerList = new List<Player>();
            _dealtCards = new List<Card>();
            //playerList = new List<Player>();
            rows = new List<Row>();
            //playerList.Add(new Player("Test1"));
            //playerList.Add(new Player("Test2"));
            //playerList.Add(new Player("Test3"));
            _deck = Deck.CreateDeck();
            _roundPlayedCards = new Dictionary<int, Player>();
            _isLayoutReady = false;
            _isHandDealt = false;
        }

        private void Update()
        {
            if(playerList.Count == 0) 
            {
                playerList = MainMenuController.players;
                activePlayerText.text = playerList[0].Name;
                activePlayerScore.text = $"Score: {playerList[0].Score}";
            } 
            else 
            { 
                if (!_isLayoutReady)
                {
                    if (playerArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x != 0)
                    {
                        // get x localPosition of playerarea
                        GetPlayerAreaStartPositions();

                        GetPlayAreaStartPositions();
                        // stop if
                        _isLayoutReady = true;
                    }
                }
                else if (_isLayoutReady && !_isHandDealt)
                {
                    ResetRound();
                    currentPlayerIndex = 0;
                    currentPlayer = playerList[currentPlayerIndex];
                    currentPlayer.LoadCards();
                    // stop if
                    _isHandDealt = true;
                }
                else if (_isPassing && ClickUtil.PrevGameObject != null)
                {
                    
                    Card selectedCard = _deck[Convert.ToInt32(ClickUtil.PrevGameObject.name) - 1];
                    _roundPlayedCards.Add(selectedCard.CardNumber, currentPlayer);
                    currentPlayer.CardsInHand.Remove(selectedCard);
                    if (currentPlayerIndex == playerList.Count - 1)
                    {
                        currentPlayerIndex = 0;
                        turnCount++;
                        AddPlayedCardsToRows();
                        //Kaarten aan rijen toevoegen
                        if (turnCount == 10)
                        {
                            ResetRound();
                            if(GameOver())
                            {
                                EndGame();
                                return;
                            }
                        }
                        _roundPlayedCards.Clear();
                    }
                    else
                    {
                        currentPlayerIndex++;
                    }
                    currentPlayer.isDone();

                    foreach(var row in rows)
                    {
                        row.ClearUI();
                    }
                    currentPlayer = playerList[currentPlayerIndex];
                    activePlayerText.text = currentPlayer.Name;
                    activePlayerScore.text = $"Score: {currentPlayer.Score}";
                    ClickUtil.PrevGameObject = null;
                    passingCanvas.gameObject.SetActive(true);
                    passingCanvas.SetPlayerName(currentPlayer.Name);
                }
                _isPassing = false;
            }
        }

        private void AddPlayedCardsToRows()
        {
            foreach (KeyValuePair<int, Player> playedCard in _roundPlayedCards.OrderBy(key => key.Key))
            {
                int lowestDiff = 110;
                int lowestDiffIndex = -1;
                for (int i = 0; i < rows.Count; i++)
                {
                    int currentDiff = rows[i].GetDifference(playedCard.Key);
                    if (currentDiff < lowestDiff)
                    {
                        lowestDiff = currentDiff;
                        lowestDiffIndex = i;
                    }
                }
                if (lowestDiffIndex != -1)
                {
                    if (rows[lowestDiffIndex].IsFull)
                    {
                        playedCard.Value.TakeRow(rows[lowestDiffIndex]);
                    }
                    rows[lowestDiffIndex].AddCardToCardList(_deck[playedCard.Key - 1]);
                    rows[lowestDiffIndex].LoadCards();
                } else {
                    int lowestScore = 150;
                    int lowestScoreIndex = -1;
                    for(int i = 0; i < rows.Count; i++) 
                    {
                        int rowScore = rows[i].GetRowScore();
                        Debug.Log($"Row {i+1} has score {rowScore}");
                        if(rowScore < lowestScore) 
                        {
                            lowestScore = rowScore;
                            lowestScoreIndex = i;
                        }
                    }
                    Debug.Log($"player {playedCard.Value.Name} had to take row {lowestScoreIndex+1}");
                    playedCard.Value.TakeRow(rows[lowestScoreIndex]);
                    rows[lowestScoreIndex].AddCardToCardList(_deck[playedCard.Key - 1]);
                    rows[lowestScoreIndex].LoadCards();
                }
            }
        }

        public void NextPlayerTurn()
        {
            passingCanvas.gameObject.SetActive(false);
            currentPlayer.LoadCards();
            foreach(var row in rows)
            {
                row.LoadCards();
            }
        }

        public void OnNextButtonPressed()
        {
            _isPassing = true;
        }

        private void ResetRound()
        {
            UpdatePlayerScores();
            _dealtCards.Clear();
            foreach(var row in rows) 
            {
                row.ResetRow();
            }

            if (!GameOver())
            {
                DealPlayerCards();
                DealRowCards();
                turnCount = 0;
                foreach (var row in rows)
                {
                    row.LoadCards();
                }
            }
        }

        private void DealRowCards()
        {
            System.Random random = new System.Random();
            int randomNumber;
            Card tempCard;

            for (int i = 0; i < rows.Count; i++)
            {
                randomNumber = random.Next(_deck.Count);
                tempCard = _deck[randomNumber];

                while (_dealtCards.Contains(tempCard))
                {
                    randomNumber = random.Next(_deck.Count);
                    tempCard = _deck[randomNumber];
                }
                rows[i].AddCardToCardList(tempCard);
                _dealtCards.Add(tempCard);
            }
        }

        private void EndGame()
        {
            _isPassing = false;
            _roundPlayedCards.Clear();
            currentPlayer.isDone();

            foreach (var row in rows)
            {
                row.ClearUI();
            }

            playAgainCanvas.gameObject.SetActive(true);
            playerList.OrderByDescending(k => k.Score);
            //playAgainCanvas.gameObject.AddComponent<Text>();
            //var text = playAgainCanvas.gameObject.GetComponent<Text>();
            StringBuilder builder = new StringBuilder();
            foreach (var player in playerList)
            {
                builder.Append($"Player: {player.Name} - Score: {player.Score}\n");
                
            }
            endScoreText.text = builder.ToString();
        }

        public void PlayAgain()
        {
            playAgainCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene(0);
        }

        public void UpdatePlayerScores()
        {
            foreach (var player in playerList)
            {
                player.UpdateScore();
            }
        }

        public bool GameOver()
        {
            foreach (var player in playerList)
            {
                if (player.Score > 5)
                {
                    return true;
                }
            }
            return false;
        }

        private void DealPlayerCards()
        {
            System.Random random = new System.Random();
            int randomNumber;
            Card tempCard;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < playerList.Count; j++)
                {
                    randomNumber = random.Next(_deck.Count);
                    tempCard = _deck[randomNumber];

                    while (_dealtCards.Contains(tempCard))
                    {
                        randomNumber = random.Next(_deck.Count);
                        tempCard = _deck[randomNumber];
                    }
                    playerList[j].AddCardToHand(tempCard);
                    _dealtCards.Add(tempCard);
                }
            }
        }

        private void GetPlayerAreaStartPositions()
        {
            List<Vector3> startPositionsPlayerArea = new List<Vector3>();
            foreach (Transform image in playerArea.transform)
            {
                RectTransform rt = image.GetComponent<RectTransform>();
                startPositionsPlayerArea.Add(rt.localPosition);
            }

            foreach (var player in playerList)
            {
                player.PlayerCardPositions = startPositionsPlayerArea;
                player.cards = cards;
            }
        }

        private void GetPlayAreaStartPositions()
        {
            List<Vector3> startPositionsPlayArea1 = new List<Vector3>();
            List<Vector3> startPositionsPlayArea2 = new List<Vector3>();
            List<Vector3> startPositionsPlayArea3 = new List<Vector3>();
            List<Vector3> startPositionsPlayArea4 = new List<Vector3>();
            for (int i = 0; i < 5; i++)
            {
                RectTransform rt = playArea.transform.GetChild(i).GetComponent<RectTransform>();
                startPositionsPlayArea1.Add(rt.localPosition);
                RectTransform rt1 = playArea1.transform.GetChild(i).GetComponent<RectTransform>();
                startPositionsPlayArea2.Add(rt1.localPosition);
                RectTransform rt2 = playArea2.transform.GetChild(i).GetComponent<RectTransform>();
                startPositionsPlayArea3.Add(rt2.localPosition);
                RectTransform rt3 = playArea3.transform.GetChild(i).GetComponent<RectTransform>();
                startPositionsPlayArea4.Add(rt3.localPosition);
            }

            Row row1 = new Row(-220, startPositionsPlayArea1);
            Row row2 = new Row(30, startPositionsPlayArea2);
            Row row3 = new Row(280, startPositionsPlayArea3);
            Row row4 = new Row(530, startPositionsPlayArea4);

            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);

            foreach (var row in rows)
            {
                row.cards = cards;
            }
        }
    }
}
