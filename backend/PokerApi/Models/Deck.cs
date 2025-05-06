using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerApi.Models {
    public class Deck {
        public Stack<Card> cards;
        private string[] ranks = 
        {"2","3","4","5","6","7","8","9","T","J","Q","K","A"};
        private string[] suits = {"c","s","h","d"};

        public Deck() {
            cards = new Stack<Card>();
            foreach(string rank in ranks) {
                foreach(string suit in suits) {
                    cards.Push(new Card(rank,suit));
                }
            }
            Shuffle();
        }

        public Card Draw() => cards.Pop();

        public void Shuffle() {
            var list = cards.ToList();
            cards.Clear();

            // Fisher–Yates shuffle
            var rand = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                // pick a random index from 0..i
                int j = rand.Next(i + 1);

                // swap list[i] and list[j]
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }

            foreach (var card in list)
            {
                cards.Push(card);
            }
        }
    }
}