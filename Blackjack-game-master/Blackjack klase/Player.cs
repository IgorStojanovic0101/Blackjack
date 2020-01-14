using OTTER;
namespace Blackjack
{
    public class Player:Person
    {

        public Player(int _money) : base()
        {
            basicErrrors = 0;
            money = _money;
            score = 0;
            bet = 0;
        }

        public Player() : base() { score = 0; money = 10000; }
        private double money;
        private int basicErrrors;
        public int BasicErrors
        {
            get { return basicErrrors; }
            set { basicErrrors = value; }

        }
        public double Money
        {
            get { return money; }
            set { money = value; }
        }

        private int cards_number;
        public int CardsNumber
        {
            get { return Hand.Count; }
        }

        private int score;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        private int bet;

        public int Bet
        {
            get { return bet; }
            set { bet = value; if (bet < 0) { bet = 0; } }
        }

        public string LastCard
        {
            get { return Hand[CardsNumber - 1].ID; }
        }

        public int LastCardValue
        {
            get { return Hand[CardsNumber - 1].Value; }
        }
    }
}
