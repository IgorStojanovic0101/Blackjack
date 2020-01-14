using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blackjack;

namespace OTTER
{
    public class SpriteList<T> : List<T>
    {        
        public new void Add(T item)
        {
            base.Add(item);
            Change = true;
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            Change = true;
        }

        public SpriteList()
        {
            Change = false;
        }

        private bool change;

        public bool Change
        {
            get { return change; }
            set { change = value; }
        }
    }

   
    public class Game
    {

       
        public static void WaitMS(int ms)
        {
            Thread.Sleep(ms);
        }

       
        public static void AddSprite(Sprite s)
        {            
            s.SpriteIndex = BGL.spriteCount;
            BGL.spriteCount++;
            BGL.allSprites.Add(s);
        }

       
        public static void StartScriptAndWait(Func<int> scriptName)
        {
            Task t = Task.Factory.StartNew(scriptName);
            t.Wait();
        }

      
        public static void StartScript(Func<int> scriptName)
        {
            Task t;
            //t = Task.Factory.StartNew(scriptName, TaskCreationOptions.LongRunning);
            t = Task.Factory.StartNew(scriptName);
        }                        
       
    } //game
}
