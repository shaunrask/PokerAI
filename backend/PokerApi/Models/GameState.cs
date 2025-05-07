using System.Collections.Generic;

namespace PokerApi.Models
{
    public class GameState
    {
        public int Pot { get; set; }
        public int CurrentBet { get; set; }
        public required string Stage { get; set; }
        public int TurnIndex { get; set; }
        public bool RoundComplete { get; set; }
        public required List<Player> Players { get; set; }
        public required List<Card> ShownCards { get; set; }
    }
}
