using System.Collections.Generic;
using System.Linq;
using Xunit;
using PokerApi.Models;

namespace PokerApi.Tests
{
    public class TableTests
    {
        private Table MakeTable(int numPlayers = 3)
        {
            // Initialize players with required Name and starting Chips
            var players = Enumerable.Range(1, numPlayers)
                                    .Select(i => new Player { Name = $"P{i}", Chips = 1000 })
                                    .ToList();
            return new Table(players);
        }

        [Fact]
        public void Reset_DealsTwoCardsAndPostsBlinds()
        {
            var table = MakeTable();
            var state = table.Reset();

            // each player has two cards
            Assert.All(state.Players, p => Assert.Equal(2, p.Hand.Count));

            // blinds posted correctly
            Assert.Equal(10, state.Players[0].CurrentBet);
            Assert.Equal(20, state.Players[1].CurrentBet);

            // pot equals sum of blinds
            Assert.Equal(30, state.Pot);

            // action starts after big blind
            Assert.Equal(2, state.TurnIndex);
        }

        [Fact]
        public void Fold_RemovesPlayerFromActionAndAdvancesTurn()
        {
            var table = MakeTable(3);
            int initialTurn = table.TurnIndex;

            var state = table.HandlePlayerAction("fold", 0);

            // folded flag set on that player
            Assert.True(table.Players[initialTurn].Folded);

            // turn moved to a different player
            Assert.NotEqual(initialTurn, state.TurnIndex);
        }

        [Fact]
        public void Call_DeductsChipsAndIncreasesPot()
        {
            var table       = MakeTable(2);
            int callerIndex = table.TurnIndex;
            var caller      = table.Players[callerIndex];

            int startingChips = caller.Chips;        // 990 after small blind
            int currentBet    = table.CurrentBet;    // 20
            int startingPot   = table.Pot;           // 10 + 20 = 30

            int toCall = currentBet - caller.CurrentBet; // 20–10 = 10

            var state = table.HandlePlayerAction("call", 0);

            // Stack should drop by exactly the amount owed
            Assert.Equal(startingChips - toCall, caller.Chips);   // 990–10 = 980

            // Pot should increase by the same amount
            Assert.Equal(startingPot + toCall, state.Pot);        // 30+10 = 40
        }
    }
}
