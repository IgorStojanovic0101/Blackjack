using OTTER;
namespace Blackjack
{
    public class Dealer:Person
    {
        
        public Dealer():base() 
        {
            _bust = false;
        }

        private bool _bust;
       
        public bool Bust
        {
            get
            {
                if (HandValue <= 21) return true;
                else return false;
            }
            set
            { _bust = value; }
        }
    }
}
