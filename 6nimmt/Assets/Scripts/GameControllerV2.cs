using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameControllerV2 : MonoBehaviour
    {
        public Cards cards;
        public List<Player> playerList;
        public GameObject playerArea;
        public GameObject players;
        private List<Card> _deck;
        private List<Card> _roundPlayedCards;
        private int turnCount;
        private Player currentPlayer;
        private int currentPlayerIndex;
        private bool _isLayoutReady;
        private bool _isHandDealt;

        private void Start()
        {
            //playerList = MainMenuController.players;
            playerList = new List<Player>();
            playerList.Add(new Player("Test1"));
            playerList.Add(new Player("Test2"));
            _deck = Deck.CreateDeck();
            _roundPlayedCards = new List<Card>();
            _isLayoutReady = false;
            _isHandDealt = false;
            
        }

        private void Update()
        {
            if (!_isLayoutReady) {
                if (playerArea.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x != 0) {
                    // get x localPosition of playerarea
                    GetPlayerAreaStartPositions();
                    // stop if
                    _isLayoutReady = true;
                }
            }
            else if (_isLayoutReady && !_isHandDealt) {
                ResetRound();

                // stop if
                _isHandDealt = true;
            }
            else if(Input.GetKeyDown(KeyCode.N))
            {
                if(currentPlayerIndex == playerList.Count - 1)
                {
                    currentPlayerIndex = 0;
                    turnCount++;
                    //Kaarten aan rijen toevoegen
                    if(turnCount == 10) {
                        ResetRound();
                    }
                } else {
                    currentPlayerIndex++;
                    currentPlayer.isDone();
                    currentPlayer = playerList[currentPlayerIndex%playerList.Count];
                    currentPlayer.LoadCards();
                }
            } 
        }

        private void ResetRound()
        {
            UpdatePlayerScores();
            if(!GameOver())
            {
                DealPlayerCards();
                turnCount = 0;
                currentPlayerIndex = 0;
                currentPlayer = playerList[currentPlayerIndex];
                currentPlayer.LoadCards();
            }
            else 
            {
                EndGame();
            }
        }

        private void EndGame() 
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayerScores()
        {
            foreach(var player in playerList)
            {
                player.UpdateScore();
            }
        }

        public bool GameOver()
        {
            foreach (var player in playerList)
            {
                if (player.Score > 66)
                {
                    return true;
                }
            }
            return false;
        }

        private void DoRound()
        {
            Player player = playerList[0];
            for (int i = 0; i < 10; i++)
            {
                DoTurn(player);
            }
        }

        private void DoTurn(Player player)
        {
            throw new NotImplementedException();
        }

        private void DealPlayerCards()
        {
            System.Random random = new System.Random();
            int randomNumber;
            Card tempCard;
            List<Card> _dealtCards = new List<Card>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < playerList.Count; j++)
                {
                    randomNumber = random.Next(_deck.Count);
                    tempCard = _deck[randomNumber];

                    while(_dealtCards.Contains(tempCard))
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

            foreach(var player in playerList)
            {
                player.PlayerCardPositions = startPositionsPlayerArea;
                player.cards = cards;
            }
        }

    }
}
