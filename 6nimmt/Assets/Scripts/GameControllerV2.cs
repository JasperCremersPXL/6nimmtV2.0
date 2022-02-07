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
        private List<Player> playerList;
        public GameObject playerArea;
        public GameObject players;
        private List<Card> _deck;
        private List<Card> _roundPlayedCards;

        private void Start()
        {
            playerList = MainMenuController.players;
            _deck = Deck.CreateDeck();
            _roundPlayedCards = new List<Card>();
            GetPlayerAreaStartPositions();
            GameLoop();

        }

        private void Update()
        {
            // turnPlayer // turnChanged if (turnChanged) turnPlayer.DoTurn(); turnChanged = false;
            if (!GameOver())
            {
                DealPlayerCards();
                DoRound();
                UpdatePlayerScores();
            }
        }

        private void GameLoop()
        {
            while(!GameOver())
            {
                DealPlayerCards();
                DoRound();
                UpdatePlayerScores();
            }
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
            player.
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
            }
        }

    }
}
