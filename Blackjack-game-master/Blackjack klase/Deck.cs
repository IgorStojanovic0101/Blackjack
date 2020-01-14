using System;
using System.Collections.Generic;

namespace Blackjack
{
   
    public class Deck
    {
        private Random r;

        private List<Card> cards;
     
        public List<Card> Cards
        {
            get { return cards; }set { cards = value; }
        }

        public int CardsNumber
        {
            get { return Cards.Count; }
        }

        public void Shuffle()
        {
            Random r = new Random();
            for (int i = 1; i <= 100; i++)
            {
                for (int index = 0; index < CardsNumber; index++)
                {
                    int k = r.Next(0,CardsNumber-1);
                    Card pomocna = cards[index];
                    cards[index] = cards[k];
                    cards[k] = pomocna;
                }
            }
        }


        public Card ReturnCard()
        {
            int index = r.Next(0, CardsNumber - 1);
            Card karta = new Card(Cards[index].ID);
            cards.RemoveAt(index);
            return karta;
        }

        public Deck()
        {
            Cards = new List<Card>();r = new Random();
        }
        public Deck(int broj)
        {
            r = new Random();
            cards = new List<Card>();
            for (int i = 1; i <= broj; i++)
            {
                AddDeck();
            }
           
        }

        
        private void AddDeck()
        {
            for (int i = 1; i <= 4; i++)
            {
                for (int l = 2; l <= 10; l++)
                {
                    Card karta = new Card(l.ToString());
                    Cards.Add(karta);
                }
              
                Card a = new Card("A");Card k = new Card("K");Card q = new Card("Q");Card j = new Card("J");
                Cards.Add(a);Cards.Add(k);Cards.Add(q);Cards.Add(j);
            }
        }
}
}
