using System.Collections.Generic;
using OTTER;
using System;

namespace Blackjack
{
  
    public abstract class Person
    {
      
        public Person()
        {
            hand = new List<Card>();
            last_card_x = 0;
            last_card_y = 0;
            active = false;
            soft_hand = false;
            r = new Random();
        }

        private List<Card> hand;

     
        public List<Card> Hand
        {
            get { return hand; }set { hand = value; }
        }

        private int last_card_x;
        private int last_card_y;
        private bool active;
        private bool soft_hand;
        private Random r;

      
        public void SetNewCard(int x,int y)
        {
            LastCard_X = x;LastCard_Y = y;
        }

       
        public int HandValue
        {
            get
            {
                int br_aseva = 0;
                int vrijednost = 0;
                for (int i = 0; i < Hand.Count; i++)
                {
                    if (Hand[i].ID == "A")
                    {
                        br_aseva = br_aseva + 1;
                    }
                    else
                    {
                        vrijednost = vrijednost + Hand[i].Value;
                    }
                }
                SoftHand = false;
               
                if (br_aseva > 0)
                {
                    if ((vrijednost + 11 + br_aseva - 1) <= 21)
                    {
                        vrijednost = vrijednost + 11 + br_aseva - 1;
                        SoftHand = true;
                    }
                    else
                    {
                        vrijednost = vrijednost + br_aseva;
                    }
                }
                return vrijednost;
            }
        }

       
        public bool SoftHand
        {
            get { return soft_hand; }set { soft_hand = value; }
        }

        
        public int LastCard_X
        {
            get
            {
                return last_card_x;
            }
            set
            {
                last_card_x = value;
            }
        }

        public int LastCard_Y
        {
            get
            {
                return last_card_y;
            }
            set
            {
                last_card_y = value;
            }
        }

       
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

      
        public void AddNewCard(Card nova_karta)
        {
           
            string putanja = "sprites\\cards2\\"+nova_karta.ID.ToString()+"\\"+r.Next(1,5).ToString()+".png";
            Hand.Add(nova_karta);
            Sprite card = new Sprite(putanja, LastCard_X, LastCard_Y, BlackjackGame.Sirina, BlackjackGame.Visina,"card");//napravimo novi lik(karta)
            Game.AddSprite(card);
            SetNewCard(LastCard_X + 15, LastCard_Y);
        }
    }
}
