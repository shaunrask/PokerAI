using System.Collections.Generic;

namespace PokerApi.Models {
    public class Player {
        public required string Name { get; set; }
        public int Chips { get; set; }
        public List<Card> Hand { get; set; } = [];
        public bool Folded { get; set; }
        public int CurrentBet { get; set; }
        public bool IsBot { get; set; }
    }
}
