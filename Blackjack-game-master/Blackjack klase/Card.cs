namespace Blackjack
{
   
    public class Card
    {
        private string id;
       
        public string ID
        {
            get { return id; }set { id = value; }
        }
        
        private int card_value;
      
        public int Value
        {
            get
            {
                return card_value;
            }
            set { card_value = value; }
        }
       
        public bool IDNumber
        {
            get
            {
                if (ID == "J" || ID == "Q" || ID == "K" || ID == "A") return true;
                else return false;
            }
        }

       
        public Card()
        { id = "";card_value = 0; }
        public Card(string id_karte)
        {
            id = id_karte;
          
            try
            {
                card_value = int.Parse(id_karte);
            }
            catch 
            {
                if (id_karte == "J" || id_karte == "Q" || id_karte == "K")
                {
                    card_value = 10;
                }
                else card_value = 11;
            }
        } 
    }
}
