using System;
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

            Assert.All(state.Players, p => Assert.Equal(2, p.Hand.Count));
            Assert.Equal(10, state.Players[0].CurrentBet);
            Assert.Equal(20, state.Players[1].CurrentBet);
            Assert.Equal(30, state.Pot);
            Assert.Equal(2, state.TurnIndex);
        }

        [Fact]
        public void Fold_RemovesPlayerFromActionAndAdvancesTurn()
        {
            var table = MakeTable(3).Reset();
            int initialTurn = table.TurnIndex;

            var state = table.HandlePlayerAction("fold", 0);

            Assert.True(table.Players[initialTurn].Folded);
            Assert.NotEqual(initialTurn, state.TurnIndex);
        }

        [Fact]
        public void Call_DeductsChipsAndIncreasesPot()
        {
            var table = MakeTable(2);
            var afterReset = table.Reset();
            int callerIndex = afterReset.TurnIndex;
            var caller = table.Players[callerIndex];

            int startingChips = caller.Chips;
            int currentBet    = afterReset.CurrentBet;
            int startingPot   = afterReset.Pot;

            int toCall = currentBet - caller.CurrentBet;
            var state = table.HandlePlayerAction("call", 0);

            Assert.Equal(startingChips - toCall, caller.Chips);
            Assert.Equal(startingPot + toCall, state.Pot);
        }

        [Fact]
        public void Raise_IncreasesCurrentBetAndPot()
        {
            var table = MakeTable(2).Reset();
            var raiser = table.Players[table.TurnIndex];

            int startingChips = raiser.Chips;
            int raiseTo       = table.CurrentBet + 30;
            int toRaise       = raiseTo - raiser.CurrentBet;
            int startingPot   = table.Pot;

            var state = table.HandlePlayerAction("raise", raiseTo);

            Assert.Equal(raiseTo, raiser.CurrentBet);
            Assert.Equal(startingChips - toRaise, raiser.Chips);
            Assert.Equal(startingPot + toRaise, state.Pot);
        }

        [Fact]
        public void Check_ThrowsWhenBetExists()
        {
            var table = MakeTable(2).Reset();
            Assert.Throws<InvalidOperationException>(() =>
                table.HandlePlayerAction("check", 0));
        }

        [Fact]
        public void AdvanceStage_DealsFlopTurnRiverAndShowdown()
        {
            var table = MakeTable(2).Reset();
            // everyone calls
            while (!table.IsBettingRoundOver())
                table.HandlePlayerAction("call", 0);

            var flop = table.GetState();
            Assert.Equal("flop", table.Stage);
            Assert.Equal(3, flop.ShownCards.Count);

            table.AdvanceStage();
            Assert.Equal("turn", table.Stage);
            Assert.Equal(4, table.GetState().ShownCards.Count);

            table.AdvanceStage();
            Assert.Equal("river", table.Stage);
            Assert.Equal(5, table.GetState().ShownCards.Count);

            table.AdvanceStage();
            Assert.Equal("showdown", table.Stage);
        }

        [Fact]
        public void IsBettingRoundOver_ReflectsActiveBets()
        {
            var table = MakeTable(3).Reset();
            Assert.False(table.IsBettingRoundOver());

            // everyone posts equal-current bets
            foreach (var p in table.Players)
                p.CurrentBet = table.CurrentBet;
            Assert.True(table.IsBettingRoundOver());
        }

        [Fact]
        public void Fold_AllButOne_AdvancesToFlopImmediately()
        {
            // 2-player game: if P0 folds, P1 wins preflop and we go to flop
            var table = MakeTable(2).Reset();
            // P0 is small blind → 10, P1 is big blind → 20
            var state = table.HandlePlayerAction("fold", 0);

            // Only one active player left ⇒ Stage should advance
            Assert.Equal("flop", table.Stage);
            // Pot stays as the blinds only
            Assert.Equal(30, state.Pot);
        }

        [Fact]
        public void RoundComplete_FlagIsFalseUntilShowdown()
        {
            var table = MakeTable(2).Reset();
            var initial = table.GetState();
            Assert.False(initial.RoundComplete);

            // Manually cycle through stages
            table.AdvanceStage(); // flop
            Assert.False(table.GetState().RoundComplete);
            table.AdvanceStage(); // turn
            Assert.False(table.GetState().RoundComplete);
            table.AdvanceStage(); // river
            Assert.False(table.GetState().RoundComplete);
            table.AdvanceStage(); // showdown
            Assert.True(table.GetState().RoundComplete);
        }
    }
}
