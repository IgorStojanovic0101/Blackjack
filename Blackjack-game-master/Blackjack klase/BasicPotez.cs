using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER.Blackjack_klase
{
    public class BSPotez
    {
        public BSPotez(string hand, int player, int diler)
        {
            this.HandType = hand;
            this.Player = player;
            this.Diler = diler;
        }
        public string HandType { get; set; }
        public int Player { get; set; }
        public int Diler { get; set; }

        public string potez;

        public string GetPotez()
        {
            return potez;
        }

        public void SetPotez(string value)
        {
            potez = value;
        }
    }
}
