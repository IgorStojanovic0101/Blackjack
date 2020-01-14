using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OTTER
{
   
    public class Sensing
    {
       
        public Point Mouse;

      
        public bool MouseDown;

        
        private string key;

        private bool keyPressedTest;

       
        public bool KeyPressedTest
        {
            get { return keyPressedTest; }
            set { keyPressedTest = value; }
        }

       
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

       
        public Sensing()
        {
            MouseDown = false;
            keyPressedTest = false;
            Mouse = new Point(0, 0);
        }

      
        public bool KeyPressed(string keyName)
        {
            if (KeyPressedTest && Key == keyName)
            {
                Game.WaitMS(20);
                return true;
            }
            else
                return false;
        }

       
        public bool KeyPressed(Keys key)
        {
            if (KeyPressedTest && Key == key.ToString())
            {
                Game.WaitMS(20);
                return true;
            }
            else
                return false;
        }

       
        public bool KeyPressed()
        {
            return KeyPressedTest;
        }

       
        public void KeyUp()
        {
            keyPressedTest = false;
            key = "";
        }

    } 
    //sensing
}
