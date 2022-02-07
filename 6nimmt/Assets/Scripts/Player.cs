using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public List<Card> CardsInHand { get; set; }
        private List<Card> _cardsTaken;
        private List<Vector3> _playerCardPositions;

        public Player(string name) {
            Name = name;
            Score = 0;
            CardsInHand = new List<Card>();
            _cardsTaken = new List<Card>();
            _playerCardPositions = new List<Vector3>();
        }

        public void LoadCards() 
        {
            
        }
    }

