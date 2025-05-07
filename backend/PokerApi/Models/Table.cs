using System.Collections.Generic;

namespace PokerApi.Models {
        public class Table {
        public List<Player> Players { get; }
        private Deck Deck { get; set; } = new Deck();
        public int Pot { get; private set; }
        public int CurrentBet { get; private set; }
        public string Stage { get; private set; } = "preflop";
        public int TurnIndex { get; private set; }
        public List<Card> ShownCards { get; } = [];

        public Table(IEnumerable<Player> players)
        {
            Players = [.. players];
            Reset();
        }

        public GameState Reset()
        {
            Deck = new Deck();
            Pot = 0;
            CurrentBet = 0;
            Stage = "preflop";
            TurnIndex = 0;
            ShownCards.Clear();
            foreach (var p in Players)
            {
                p.Hand.Clear();
                p.Folded = false;
                p.CurrentBet = 0;
                // deal two cards
                p.Hand.Add(Deck.Draw());
                p.Hand.Add(Deck.Draw());
            }
            // TODO: post blinds, set TurnIndex, etc.
            return GetState();
        }

        public GameState HandlePlayerAction(string action, int amount)
        {
            // TODO: implement fold, call, bet/raise logic
            return GetState();
        }

        public GameState MakeBotMove()
        {
            // TODO: single bot decision
            return GetState();
        }

        public GameState AdvanceStage()
        {
            // TODO: deal flop/turn/river and reset bets
            return GetState();
        }

        public GameState GetState()
        {
            return new GameState
            {
                Pot = Pot,
                CurrentBet = CurrentBet,
                Stage = Stage,
                TurnIndex = TurnIndex,
                RoundComplete = false,           // TODO: compute
                ShownCards = new List<Card>(ShownCards),
                Players = new List<Player>(Players)
            };
        }
    }
}