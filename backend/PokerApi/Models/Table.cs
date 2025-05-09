using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerApi.Models
{
    public class Table
    {
        public List<Player> Players { get; }
        private Deck Deck { get; set; }
        public int Pot { get; private set; }

        // ① Made the setter public so tests like `table.CurrentBet = 0;` compile:
        public int CurrentBet { get; set; }

        public string Stage { get; private set; } = "preflop";

        // ② Made the setter public so tests like `table.TurnIndex = 0;` compile:
        public int TurnIndex { get; set; }

        public List<Card> ShownCards { get; }

        private readonly int SmallBlind = 10;
        private readonly int BigBlind   = 20;

        public Table(IEnumerable<Player> players)
        {
            // fixed initializer
            Players    = players.ToList();
            Deck       = new Deck();
            ShownCards = new List<Card>();
            Reset();
        }

        // ③ Changed return type so tests can chain .Reset().HandlePlayerAction(...)
        public Table Reset()
        {
            Deck       = new Deck();
            Pot        = 0;
            CurrentBet = 0;
            Stage      = "preflop";
            TurnIndex  = 0;
            ShownCards.Clear();

            foreach (var p in Players)
            {
                p.Hand.Clear();
                p.Folded     = false;
                p.CurrentBet = 0;
            }

            foreach (var p in Players)
            {
                p.Hand.Add(Deck.Draw());
                p.Hand.Add(Deck.Draw());
            }

            var sbPlayer = Players[0];
            var bbPlayer = Players[1];

            sbPlayer.Chips      -= SmallBlind;
            sbPlayer.CurrentBet  = SmallBlind;
            Pot                 += SmallBlind;

            bbPlayer.Chips      -= BigBlind;
            bbPlayer.CurrentBet  = BigBlind;
            Pot                 += BigBlind;

            CurrentBet = BigBlind;
            TurnIndex  = 2 % Players.Count;

            return this;
        }

        public GameState HandlePlayerAction(string action, int amount)
        {
            var player = Players[TurnIndex];

            if (action == "fold")
            {
                player.Folded = true;
            }
            else if (action == "check")
            {
                if (CurrentBet != player.CurrentBet)
                    throw new InvalidOperationException("Cannot check when there's a bet.");
            }
            else if (action == "call")
            {
                int toCall = CurrentBet - player.CurrentBet;
                if (toCall > player.Chips)
                    throw new InvalidOperationException("Not enough chips to call.");

                player.Chips      -= toCall;
                player.CurrentBet += toCall;
                Pot               += toCall;
            }
            else if (action == "raise")
            {
                if (amount <= CurrentBet)
                    throw new InvalidOperationException("Raise must be above current bet.");
                int raiseAmount = amount - player.CurrentBet;
                if (raiseAmount > player.Chips)
                    throw new InvalidOperationException("Not enough chips to raise.");

                player.Chips      -= raiseAmount;
                player.CurrentBet += raiseAmount;
                CurrentBet         = player.CurrentBet;
                Pot               += raiseAmount;
            }
            else
            {
                throw new ArgumentException($"Unknown action '{action}'");
            }

            do
            {
                TurnIndex = (TurnIndex + 1) % Players.Count;
            } while (Players[TurnIndex].Folded || Players[TurnIndex].Chips == 0);

            if (IsBettingRoundOver())
                AdvanceStage();

            return GetState();
        }

        public GameState MakeBotMove()
        {
            var player = Players[TurnIndex];
            if (player.IsBot)
            {
                if (CurrentBet == player.CurrentBet)
                    return HandlePlayerAction("check", 0);
                else
                    return HandlePlayerAction("call",  0);
            }
            return GetState();
        }

        public GameState AdvanceStage()
        {
            foreach (var p in Players)
                p.CurrentBet = 0;
            CurrentBet = 0;

            if (Stage == "preflop")
            {
                Stage = "flop";
                Deck.Draw(); // burn
                ShownCards.Add(Deck.Draw());
                ShownCards.Add(Deck.Draw());
                ShownCards.Add(Deck.Draw());
            }
            else if (Stage == "flop")
            {
                Stage = "turn";
                Deck.Draw(); // burn
                ShownCards.Add(Deck.Draw());
            }
            else if (Stage == "turn")
            {
                Stage = "river";
                Deck.Draw(); // burn
                ShownCards.Add(Deck.Draw());
            }
            else if (Stage == "river")
            {
                Stage = "showdown";
            }

            TurnIndex = 0;
            while (Players[TurnIndex].Folded || Players[TurnIndex].Chips == 0)
                TurnIndex = (TurnIndex + 1) % Players.Count;

            return GetState();
        }

        public bool IsBettingRoundOver()
        {
            var active = Players.Where(p => !p.Folded && p.Chips > 0).ToList();
            if (active.Count <= 1) return true;
            return active.All(p => p.CurrentBet == CurrentBet);
        }

        public GameState GetState()
        {
            return new GameState
            {
                Pot           = Pot,
                CurrentBet    = CurrentBet,
                Stage         = Stage,
                TurnIndex     = TurnIndex,
                RoundComplete = (Stage == "showdown"),
                ShownCards    = new List<Card>(ShownCards),
                Players       = new List<Player>(Players)
            };
        }
    }
}
