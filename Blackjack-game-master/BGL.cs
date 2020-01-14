using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Blackjack;
using StackExchange.Redis;
using Newtonsoft.Json;
using OTTER.Blackjack_klase;

namespace OTTER
{
   
    public partial class BGL : Form
    {
       
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();
        Form1 statistics = new Form1();

        public static bool START = true;

      
        public static int spriteCount = 0, soundCount = 0;

     
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>(); //LISTA SVIH LIKOVA U IGRI

      
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
      
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                foreach (Sprite sprite in allSprites)
                {
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
            OnemoguciKontrole();
            LoadSettings();
            BetChange += new BetChangeEvent(AddBet);

            
            ClearBet += new ClearBetEvent(ClearBet_Clicked);


            
            pictureBoxStol.Visible = false;
            Sprite slika = new Sprite("Resources\\blackjack-table.png", pictureBoxStol.Location.X, pictureBoxStol.Location.Y, pictureBoxStol.Width, pictureBoxStol.Height, "pozadina");
            Game.AddSprite(slika);
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

       
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
           
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
           
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
           
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
      
        #region Start of Game Methods

       
        #region my

       

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                OTTER.Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

       
        public BGL()
        {
            InitializeComponent();
        }

        public static void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

       
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

       
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        
        #region Stage

       
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

      
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

       
        #region sound methods

      
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

      
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

       
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

       
        #region file methods

       
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

      
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

       
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

       
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        public void hideMouse()
        {
            Cursor.Hide();
        }

       
        public void showMouse()
        {
            Cursor.Show();
        }

        public bool isMousePressed()
        {
            
            return sensing.MouseDown;
        }

        
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnBasicStrategy_Click(object sender, EventArgs e) 
        {
            FormStrategy form = new FormStrategy();
            form.Show();
        }

       
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
      
        public static Player user;
        public static Player player1;
        public static Player player2;
        public static Dealer dealer;
        public static Deck deck;

       
        private delegate void BetChangeEvent(int oklada);

       
        private delegate void ClearBetEvent();

        
        private event BetChangeEvent BetChange;

      
        private event ClearBetEvent ClearBet;

        private void SetupGame()
        {
          
            SetStageTitle("Blackjack game");
            setBackgroundColor(Color.DarkGreen);          
           
            setPictureLayout("stretch");
        }

        #region moje_metode
       
        private void SetGame()
        {
            BlackjackGame.ActiveGame = true;
            user.Active = false;
            BlackjackGame.PocetniNovac = 10000;
            lblPlayerMoney.Text = "Money: 10000";
            BlackjackGame.BrojSpilova = 8;
            BlackjackGame.DoubleActive = false;
            BlackjackGame.TrenutniIgrac = TrenutniIgrac.Player1;
            MessageBox.Show("Game starts.","Game",MessageBoxButtons.OK,MessageBoxIcon.Information);
            //GameTimer.Enabled = true;
            //GameTimer.Start();
            btnNovaIgra.Enabled = true;
            lblUserScore.Visible = true;
            lblPlayerMoney.Visible = true;lblPlayerMoney.Text = "0";

          
            user.SetNewCard(520, 373);
            player1.SetNewCard(815, 340); player2.SetNewCard(215, 242); dealer.SetNewCard(512, 42);
        }

        #region game_methods
       
        private void SetNextRound()
        {
            btnDeal.Enabled = false;
            user.Active = false;
            player2.Active = false; dealer.Active = false;
            OnemoguciKontrole();
           
            user.SetNewCard(520, 373);
            player1.SetNewCard(815, 330); player2.SetNewCard(215, 242); dealer.SetNewCard(512, 42);

           
            if (deck.CardsNumber / (52 * BlackjackGame.BrojSpilova) < 0.3)
            {
                deck = new Deck(BlackjackGame.BrojSpilova);
            }

            
            player1.AddNewCard(new Card(deck.ReturnCard().ID));
            player1.AddNewCard(new Card(deck.ReturnCard().ID));

           
            user.AddNewCard(new Card(deck.ReturnCard().ID));
            user.AddNewCard(new Card(deck.ReturnCard().ID));

           
            player2.AddNewCard(new Card(deck.ReturnCard().ID));
            player2.AddNewCard(new Card(deck.ReturnCard().ID));

            
            dealer.AddNewCard(new Card(deck.ReturnCard().ID));
            Sprite poledjina = new Sprite("sprites\\blue_back.png", dealer.LastCard_X, dealer.LastCard_Y, 65, 100, "card");
            Game.AddSprite(poledjina);

            lblDealerScore.Visible = true;lblUserScore.Visible = true;
            lblDealerScore.Text = dealer.HandValue.ToString();
            lblUserScore.Text = user.HandValue.ToString();

           
            player1.Active = true;
            lblActivePlayer.Text = "Active player: player 1";
        }
        
        
        private void Play(Player player, int dealer_card)
        {
            if (player1.Active) { lblActivePlayer.Text = "Active player: player 1"; }
            else lblActivePlayer.Text = "Active player: player 2";

            if (player.CardsNumber == 7)
            {
             
                if (player1.Active)
                { player1.Active = false; user.Active = true; OmoguciKontrole(); EnableBets(false); lblActivePlayer.Text = "Active player: user"; }
                else if (player2.Active)
                {
                    player2.Active = false; dealer.Active = true; lblActivePlayer.Text = "Active player: dealer";
                    MessageBox.Show("Dealer play.");
                    while (dealer.Active) { Play(); }
                }
            }
            else 
            {
                if (player.CardsNumber == 2)
                {
                    if (player.HandValue == 10 && !player.SoftHand && dealer_card <= 9)
                    {
                        Card nova_karta = deck.ReturnCard();
                        player.AddNewCard(nova_karta);
                       
                        if (player1.Active)
                        { player1.Active = false; user.Active = true; OmoguciKontrole(); EnableBets(false); lblActivePlayer.Text = "Active player: user"; }
                        else if (player2.Active)
                        {
                            player2.Active = false; dealer.Active = true; lblActivePlayer.Text = "Active player: dealer";
                            MessageBox.Show("Dealer play.");
                            while (dealer.Active) { Play(); }
                        }
                    }
                    else 
                    {
                        if (player.HandValue < 17 || (player.HandValue >= 17 && player.HandValue<=19 && player.SoftHand))//uzima opciju hit
                        {
                            Card nova_karta = deck.ReturnCard();
                            player.AddNewCard(nova_karta);
                        }
                        else
                        {
                            if (player1.Active)
                            { player1.Active = false; user.Active = true; OmoguciKontrole(); EnableBets(false); lblActivePlayer.Text = "Active player: user"; }
                            else if (player2.Active)
                            {
                                player2.Active = false; dealer.Active = true; lblActivePlayer.Text = "Active player: dealer";
                                MessageBox.Show("Dealer play.");
                                while (dealer.Active) { Play(); }
                            }
                        }
                    }
                }
                else
                {
                    
                    if (player.HandValue >= 17 && !player.SoftHand)
                    {
                        if (player1.Active)
                        { player1.Active = false; user.Active = true;OmoguciKontrole(); EnableBets(false); lblActivePlayer.Text = "Active player: user"; }
                        else if (player2.Active)
                        {
                            player2.Active = false; dealer.Active = true; lblActivePlayer.Text = "Active player: dealer";
                            MessageBox.Show("Dealer play.");
                            while (dealer.Active) { Play(); }
                        }
                    }
                    else
                    {
                        Card nova_karta = deck.ReturnCard();
                        player.AddNewCard(nova_karta);
                    }
                }
            }
        }


        private void Play()
        {
            var redis = RedisStore.RedisCache;

            var hashKey = "user";

            HashEntry[] redisBookHash;
            redisBookHash = new HashEntry[5];

            redisBookHash[0] = new HashEntry("Bet", user.Bet);

            redisBookHash[1] = new HashEntry("Money", user.Money);
            redisBookHash[2] = new HashEntry("BSErrrors", user.BasicErrors);



            // redis.HashSet(hashKey, redisBookHash);
            lblActivePlayer.Text = "Active player: dealer";
            dealer.Active = true;
            lblDealerScore.Visible = true; lblDealerScore.Text = "0";

            while (dealer.HandValue < 17)
            {
                Card karta = deck.ReturnCard();
                dealer.AddNewCard(karta);
                lblDealerScore.Text = dealer.HandValue.ToString();
                Wait(0.05);
            }
            dealer.Active = false; player1.Active = false; player2.Active = false; user.Active = false;
            lblActivePlayer.Text = "Active player: nobody";

            if ((user.HandValue <= 21 && user.HandValue > dealer.HandValue) || (user.HandValue <= 21 && dealer.HandValue > 21))
            {
                user.Money += 2 * user.Bet;
                redisBookHash[3] = new HashEntry("Status", "Win");
                redisBookHash[4] = new HashEntry("WinningBet", user.Bet * 2);
                MessageBox.Show("You won!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else if (user.HandValue > 21 || (dealer.HandValue <= 21 && dealer.HandValue > user.HandValue))
            {
                MessageBox.Show("You lost!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                redisBookHash[3] = new HashEntry("Status", "Lose");
                redisBookHash[4] = new HashEntry("WinningBet", user.Bet * 0);

                if (user.Money <= 0)
                {
                    MessageBox.Show("You don't have any money for play!");
                    user.Active = false; player1.Active = false; player2.Active = false; dealer.Active = false;
                    OnemoguciKontrole();
                }
            }
            else
            {
                MessageBox.Show("Draw !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information); user.Money += user.Bet;
                redisBookHash[3] = new HashEntry("Status", "Draw");
                redisBookHash[4] = new HashEntry("WinningBet", user.Bet * 1);
            }
            redis.HashSet(hashKey, redisBookHash);
            var allHash = redis.HashGetAll(hashKey);
            string[] arr = new string[5];
            //get all the items


            //Add items in the listview


            DataGridViewRow row = (DataGridViewRow)statistics.dataGridView1.Rows[0].Clone();

            row.Cells[0].Value = allHash[0].Value;
            row.Cells[1].Value = allHash[1].Value;
            row.Cells[2].Value = allHash[2].Value;
            row.Cells[3].Value = allHash[3].Value;
            row.Cells[4].Value = allHash[4].Value;
            statistics.dataGridView1.Rows.Add(row);
            //Add second item


            user.Bet = 0;
            lblNovac.Text = "0";
            lblPlayerMoney.Text = "Money: " + user.Money;

            btnDeal.Enabled = true;
            lblDealerScore.Visible = false; lblUserScore.Visible = false;
            pictureBoxChip100.Enabled = true; pictureBoxChip25.Enabled = true; pictureBoxChip5.Enabled = true; pictureBoxChip50.Enabled = true;
            ClearStage();
        }
        #endregion

        private void CardVisible(bool visible)
        {
            foreach (Sprite card in allSprites)
            {
                if (card.Name.ToLower().Trim() == "card" || card.Name.ToLower().Trim( )== "karta")
                {
                    card.SetVisible(visible);
                }
            }
        }

        private void EnableBets(bool enabled)
        {
            pictureBoxChip100.Enabled = enabled;
            pictureBoxChip25.Enabled = enabled;
            pictureBoxChip5.Enabled = enabled;
            pictureBoxChip50.Enabled = enabled;
        }

      
        private void ClearStage()
        {
            CardVisible(false);
            try
            {
               
                for (int i = 0; i < allSprites.Count; i++) 
                {
                    if (allSprites[i].Show == false || allSprites[i].Name.ToLower() == "card") { allSprites.RemoveAt(i); }
                }
            }
            catch { throw new Exception(); }

            try
            {
               
                user.Hand.Clear(); player1.Hand.Clear(); player2.Hand.Clear(); dealer.Hand.Clear();
            }
            catch { throw new Exception(); }

            spriteCount = allSprites.Count;
            lblUserScore.Visible = false;
            lblDealerScore.Visible = false;
            lblPlayerMoney.Text = "Money: "+user.Money.ToString();lblPlayerMoney.Visible = true;
            lblNovac.Text = "0";lblNovac.Visible = true;
            user.Bet = 0;
        }
        
        private void LoadSettings()
        {
            BlackjackGame.ActiveGame = false;
            BlackjackGame.DoubleActive = false;
            BlackjackGame.BrojSpilova = 8;
            BlackjackGame.PocetniNovac = 10000;lblNovac.Text = "0";lblPlayerMoney.Text = "Money: 0";
            BlackjackGame.TrenutniIgrac = TrenutniIgrac.Nobody;

            
            user = new Player(10000);
            player1 = new Player();player2 = new Player();dealer = new Dealer();
            deck = new Deck(8);

            lblUserScore.Visible = false;
            lblDealerScore.Visible = false;
            btnDeal.Enabled = false;
        }

       
        private void pictureBoxStol_MouseMove(object sender, MouseEventArgs e)
        {
            lblMisKoordinate.Text = e.X + ";" + e.Y;
        }
       

        private void OnemoguciKontrole()
        {
            btnHit.Enabled = false;btnStand.Enabled = false;btnDouble.Enabled = false;
            pictureBoxChip100.Enabled = false;pictureBoxChip25.Enabled = false;pictureBoxChip5.Enabled = false;pictureBoxChip50.Enabled = false;
        }

        private void OmoguciKontrole()
        {
            btnHit.Enabled = true; btnStand.Enabled = true; btnDouble.Enabled = true; 
            pictureBoxChip100.Enabled = true; pictureBoxChip25.Enabled = true; pictureBoxChip5.Enabled = true; pictureBoxChip50.Enabled = true;
            lblUserScore.Visible = true; lblDealerScore.Visible = true;
        }

        private void BetsAllowed(bool allowed)
        {
            pictureBoxChip100.Enabled = allowed;pictureBoxChip25.Enabled = allowed;pictureBoxChip5.Enabled = allowed;pictureBoxChip50.Enabled = allowed;
        }

       

       

        
        private void ClearBet_Clicked()
        {
            if (user.Bet > 0)
            {
                user.Money += user.Bet;
                user.Bet = 0;
                lblNovac.Text = "0";
                lblPlayerMoney.Text = "Money: " + user.Money.ToString();
                btnClearOklada.Enabled = false;
            }
        }
      

      
      
        private void AddBet(int bet)
        {
            if (bet > 0)
            {
                if (user.Money - bet < 0)
                {
                    MessageBox.Show("Bet is not allowed.", "Mistake", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else 
                {
                    user.Bet += bet;
                    lblNovac.Text = user.Bet.ToString();
                    user.Money -= bet;
                    lblPlayerMoney.Text = "Money: "+ user.Money.ToString();
                    btnClearOklada.Enabled = true;
                }
            }
        }

        private void pictureBoxChip5_Click(object sender, EventArgs e)
        {
            BetChange.Invoke(5);
        }

        private void pictureBoxChip25_Click(object sender, EventArgs e)
        {
            BetChange.Invoke(25);
        }

        private void pictureBoxChip50_Click(object sender, EventArgs e)
        {
            BetChange.Invoke(50);
        }

        private void ClearBet_Clicked(object sender, EventArgs e)
        {
            ClearBet.Invoke();
        }

        private void HitClick(object sender, EventArgs e)
        {
            if (BasicStategyCheck.Checked)
            {
                var redis = RedisStore.RedisCache;
                var type = user.SoftHand ? "Soft" : "Hard";
                var count = user.HandValue;
                var diler = dealer.HandValue;
                var userKey = type + ":" + count + ":" + diler;

                //output Name = taswar, Twitter = @taswarbhatti 
                var deserializeObject = JsonConvert.DeserializeObject<BSPotez>(redis.StringGet(userKey));
              
                if (deserializeObject == null)
                {
                    MessageBox.Show("Potez ne postoji u bazi");
                }
                else
                {
                    var msg = deserializeObject.GetPotez();
                    if (msg == "HIT")
                    {
                        Card card = new Card(deck.ReturnCard().ID);
                        user.AddNewCard(card);
                        lblUserScore.Text = user.HandValue.ToString();
                        btnDouble.Enabled = false;


                        if (user.HandValue >= 21)
                        {
                            StandClick(null, null);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Basic Strategy saying.. you should " + msg);
                        user.BasicErrors++;
                    }
                }
            }
            else
            {
                Card card = new Card(deck.ReturnCard().ID);
                user.AddNewCard(card);
                lblUserScore.Text = user.HandValue.ToString();
                btnDouble.Enabled = false;


                if (user.HandValue >= 21)
                {
                    StandClick(null, null);
                }
            }

        }

        private void StandClick(object sender, EventArgs e)
        {
            if (BasicStategyCheck.Checked)
            {


                var redis = RedisStore.RedisCache;
                var type = user.SoftHand ? "Soft" : "Hard";
                var count = user.HandValue;
                var diler = dealer.HandValue;
                var userKey = type + ":" + count + ":" + diler;

                //output Name = taswar, Twitter = @taswarbhatti 
                var deserializeObject = JsonConvert.DeserializeObject<BSPotez>(redis.StringGet(userKey));
             

                if (deserializeObject == null)
                {
                    MessageBox.Show("Potez ne postoji u bazi");
                }
                else
                {
                    var msg = deserializeObject.GetPotez();
                    if (msg == "STAND")
                    {
                        user.Active = false;
                        OnemoguciKontrole();
                        btnDouble.Enabled = false;
                        player2.Active = true;

                        lblActivePlayer.Text = "Active player: player 2";
                        while (player2.Active)
                        {

                            Play(player2, dealer.HandValue);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Basic Strategy saying.. you should " + msg);
                        user.BasicErrors++;
                    }
                }
            }
            else
            {
                user.Active = false;
                OnemoguciKontrole();
                btnDouble.Enabled = false;
                player2.Active = true;

                lblActivePlayer.Text = "Active player: player 2";
                while (player2.Active)
                {

                    Play(player2, dealer.HandValue);
                }
            }



        }

        private void btnDouble_Click(object sender, EventArgs e)
        {
            if (user.Money - user.Bet < 0)
            {
                MessageBox.Show("You don't have enough money for option DOUBLE!");
                return;
            }
            if (BasicStategyCheck.Checked)
            {

                var redis = RedisStore.RedisCache;
                var type = user.SoftHand ? "Soft" : "Hard";
                var count = user.HandValue;
                var diler = dealer.HandValue;
                var userKey = type + ":" + count + ":" + diler;

                var deserializeObject = JsonConvert.DeserializeObject<BSPotez>(redis.StringGet(userKey));
               

                if (deserializeObject==null)
                {
                    MessageBox.Show("Potez ne postoji u bazi");
                }
                else
                {
                     var msg = deserializeObject.GetPotez();
                    if (msg == "DOUBLE")
                    {
                        user.Money -= user.Bet;
                        user.Bet = 2 * user.Bet;
                        lblNovac.Text = user.Bet.ToString();

                        Card card = new Card(deck.ReturnCard().ID);
                        user.AddNewCard(card);
                        lblUserScore.Text = user.HandValue.ToString();
                        StandClick(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Basic Strategy saying.. you should " + msg);
                        user.BasicErrors++;
                    }
                }
            }
            else
            {
                user.Money -= user.Bet;
                user.Bet = 2 * user.Bet;
                lblNovac.Text = user.Bet.ToString();

                Card card = new Card(deck.ReturnCard().ID);
                user.AddNewCard(card);
                lblUserScore.Text = user.HandValue.ToString();
                StandClick(null, null);
            }

        }



        private void btnTestButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sprite count: "+allSprites.Count);
        }

        private void Deal_Click(object sender, EventArgs e)
        {
            if (user.Bet <= 0)
            {
                MessageBox.Show("Add bet.");
                return;
            }
            
            btnDeal.Enabled = false;
            btnClearOklada.Enabled = false;
            OnemoguciKontrole();
            MessageBox.Show("GAME STARTS");
            SetNextRound();
            EnableBets(false);
            while (player1.Active) { Play(player1,dealer.HandValue); }
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            foreach (Sprite sprite in allSprites)
            {
                sprite.Show = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            statistics.Visible = true;
        }

        private void pictureBoxChip100_Click(object sender, EventArgs e)
        {
            BetChange.Invoke(100);
        }
        #endregion

       
        private void UpdateGameTimer(object sender, EventArgs e)
        {
            if (BlackjackGame.ActiveGame)
            {
                lblMisKoordinate.Text = "Sprite count: "+allSprites.Count;
                if (!user.Active)
                {
                    
                }
                else
                {
                   
                    if (user.Money <= 0)
                    {
                        BlackjackGame.ActiveGame = false;
                        ClearStage();
                        lblPlayerMoney.Text = "Money: 0";lblNovac.Text = "0";
                        OnemoguciKontrole();
                        MessageBox.Show("Game is finished,you don't have any money!.", "GAME END");
                    }
                    lblUserScore.Visible = true;
                }
            }
            else { GameTimer.Enabled = false;OnemoguciKontrole(); btnNovaIgra.Enabled = true; }
        }
       

        private void btnWikipedia_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://en.wikipedia.org/wiki/Blackjack");
            }
            catch
            {
                MessageBox.Show("Trouble with internet connection.","Mistake",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void btnNovaIgra_Click(object sender, EventArgs e)
        {
            var redis = RedisStore.RedisCache;

            #region Hardcard

            //serialization of object
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 12; j++)
                {
                    var u = new BSPotez("Hard", i, j);
                    u.SetPotez("HIT");

                    var userKey = "Hard:" + i + ":" + j;
                    var serializedUser = JsonConvert.SerializeObject(u);
                    redis.StringSet(userKey, serializedUser);
                    // redis.KeyDelete(userKey, CommandFlags.FireAndForget);
                }
            }
            var u9 = new BSPotez("Hard", 9, 2);
            u9.SetPotez("HIT");

            var userKey9 = "Hard:" + 9 + ":" + 2;
            var serializedUser9 = JsonConvert.SerializeObject(u9);
            redis.StringSet(userKey9, serializedUser9);


            for (int i = 7; i < 12; i++)
            {
                var u9h = new BSPotez("Hard", 9, i);
                u9h.SetPotez("HIT");
                var userKey9h = "Hard:" + 9 + ":" + i;
                var serializedUser9h = JsonConvert.SerializeObject(u9h);
                redis.StringSet(userKey9h, serializedUser9h);
            }

            for (int i = 3; i < 7; i++)
            {
                var u9d = new BSPotez("Hard", 9, i);
                u9d.SetPotez("DOUBLE");

                var userKey9d = "Hard:" + 9 + ":" + i;
                var serializedUser9d = JsonConvert.SerializeObject(u9d);
                redis.StringSet(userKey9d, serializedUser9d);
            }
            //10 
            for (int i = 2; i < 10; i++)
            {
                var u10d = new BSPotez("Hard", 10, i);
                u10d.SetPotez("DOUBLE");

                var userKey10d = "Hard:" + 10 + ":" + i;
                var serializedUser10d = JsonConvert.SerializeObject(u10d);
                redis.StringSet(userKey10d, serializedUser10d);
            }
            var u10h = new BSPotez("Hard", 10, 10);
            u10h.SetPotez("HIT");

            var userKey10h = "Hard:" + 10 + ":" + 10;
            var serializedUser10h = JsonConvert.SerializeObject(u10h);
            redis.StringSet(userKey10h, serializedUser10h);
            var u10_11h = new BSPotez("Hard", 10, 11);
            u10_11h.SetPotez("HIT");

            var userKeyu10_11h = "Hard:" + 10 + ":" + 11;
            var serializedUseru10_11h = JsonConvert.SerializeObject(u10_11h);
            redis.StringSet(userKeyu10_11h, serializedUseru10_11h);
            //11///////////////////////////////////////////////

            for (int i = 2; i < 10; i++)
            {
                var u11d = new BSPotez("Hard", 11, i);
                u11d.SetPotez("DOUBLE");

                var userKeyu11d = "Hard:" + 11 + ":" + i;
                var serializedUser11d = JsonConvert.SerializeObject(u11d);
                redis.StringSet(userKeyu11d, serializedUser11d);
            }
            var u11h = new BSPotez("Hard", 11, 11);
            u11h.SetPotez("HIT");

            var userKeyu11h = "Hard:" + 11 + ":" + 11;
            var serializedUseru11h = JsonConvert.SerializeObject(u11h);
            redis.StringSet(userKeyu11h, serializedUseru11h);
            //////////////////////////////////////////////////////////

            for (int i = 12; i < 22; i++)
            {
                for (int j = 2; j < 7; j++)
                {
                    var s = new BSPotez("Hard", i, j);
                    s.SetPotez("STAND");
                    var userKeyuS = "Hard:" + i + ":" + j;
                    var serializedUserS = JsonConvert.SerializeObject(s);
                    redis.StringSet(userKeyuS, serializedUserS);
                }

            }
            for (int i = 12; i < 17; i++)
            {
                for (int j = 7; j < 12; j++)
                {
                    var s = new BSPotez("Hard", i, j);
                    s.SetPotez("HIT");
                    var userKeyuS = "Hard:" + i + ":" + j;
                    var serializedUserS = JsonConvert.SerializeObject(s);
                    redis.StringSet(userKeyuS, serializedUserS);
                }

            }

            for (int i = 17; i < 22; i++)
            {
                for (int j = 7; j < 12; j++)
                {
                    var s = new BSPotez("Hard", i, j);
                    s.SetPotez("STAND");
                    var userKeyuS = "Hard:" + i + ":" + j;
                    var serializedUserS = JsonConvert.SerializeObject(s);
                    redis.StringSet(userKeyuS, serializedUserS);
                }

            }
            for (int i = 12; i < 17; i++)
            {
                for (int j = 7; j < 12; j++)
                {
                    var h = new BSPotez("Hard", i, j);
                    h.SetPotez("HIT");
                    var userKeyuh = "Hard:" + i + ":" + j;
                    var serializedUserh = JsonConvert.SerializeObject(h);
                    redis.StringSet(userKeyuh, serializedUserh);
                }

            }
            for (int i = 2; i < 4; i++)
            {
                var h = new BSPotez("Hard", 12, i);
                h.SetPotez("HIT");
                var userKeyuh = "Hard:" + 12 + ":" + i;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);
            }

            #endregion Hardcard

            #region SoftCard
            for (int j = 3; j < 8; j++)
            {
                for (int i = 2; i < 12; i++)
                {
                    var h = new BSPotez("Soft", j, i);
                    h.SetPotez("HIT");
                    var userKeyuh = "Soft:" + j + ":" + i;
                    var serializedUserh = JsonConvert.SerializeObject(h);
                    redis.StringSet(userKeyuh, serializedUserh);
                }
            }
            //8
            for (int i = 2; i < 9; i++)
            {
                var h = new BSPotez("Soft", 8, i);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 8 + ":" + i;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);
            }
            for (int i = 9; i < 12; i++)
            {
                var h = new BSPotez("Soft", 8, i);
                h.SetPotez("HIT");
                var userKeyuh = "Soft:" + 8 + ":" + i;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);
            }
            //
            for (int j = 3; j < 9; j++)
            {
                for (int i = 5; i < 7; i++)
                {
                    var h = new BSPotez("Soft", j, i);
                    h.SetPotez("DOUBLE");
                    var userKeyuh = "Soft:" + j + ":" + i;
                    var serializedUserh = JsonConvert.SerializeObject(h);
                    redis.StringSet(userKeyuh, serializedUserh);
                }
            }
            for (int j = 5; j < 9; j++)
            {

                var h = new BSPotez("Soft", j, 4);
                h.SetPotez("DOUBLE");
                var userKeyuh = "Soft:" + j + ":" + 4;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 7; j < 9; j++)
            {

                var h = new BSPotez("Soft", j, 3);
                h.SetPotez("DOUBLE");
                var userKeyuh = "Soft:" + j + ":" + 3;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 2; j < 12; j++)
            {

                var h = new BSPotez("Soft", 9, j);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 9 + ":" + j;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 2; j < 12; j++)
            {

                var h = new BSPotez("Soft", 10, j);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 10 + ":" + j;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            //PART TWO

            for (int j = 13; j < 18; j++)
            {
                for (int i = 2; i < 12; i++)
                {
                    var h = new BSPotez("Soft", j, i);
                    h.SetPotez("HIT");
                    var userKeyuh = "Soft:" + j + ":" + i;
                    var serializedUserh = JsonConvert.SerializeObject(h);
                    redis.StringSet(userKeyuh, serializedUserh);
                }
            }
            //8
            for (int i = 2; i < 9; i++)
            {
                var h = new BSPotez("Soft", 18, i);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 18 + ":" + i;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);
            }
            for (int i = 9; i < 12; i++)
            {
                var h = new BSPotez("Soft", 18, i);
                h.SetPotez("HIT");
                var userKeyuh = "Soft:" + 18 + ":" + i;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);
            }
            //
            for (int j = 13; j < 19; j++)
            {
                for (int i = 5; i < 7; i++)
                {
                    var h = new BSPotez("Soft", j, i);
                    h.SetPotez("DOUBLE");
                    var userKeyuh = "Soft:" + j + ":" + i;
                    var serializedUserh = JsonConvert.SerializeObject(h);
                    redis.StringSet(userKeyuh, serializedUserh);
                }
            }
            for (int j = 15; j < 19; j++)
            {

                var h = new BSPotez("Soft", j, 4);
                h.SetPotez("DOUBLE");
                var userKeyuh = "Soft:" + j + ":" + 4;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 17; j < 19; j++)
            {

                var h = new BSPotez("Soft", j, 3);
                h.SetPotez("DOUBLE");
                var userKeyuh = "Soft:" + j + ":" + 3;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 2; j < 12; j++)
            {

                var h = new BSPotez("Soft", 19, j);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 19 + ":" + j;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 2; j < 12; j++)
            {

                var h = new BSPotez("Soft", 20, j);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 20 + ":" + j;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            for (int j = 2; j < 12; j++)
            {

                var h = new BSPotez("Soft", 21, j);
                h.SetPotez("STAND");
                var userKeyuh = "Soft:" + 21 + ":" + j;
                var serializedUserh = JsonConvert.SerializeObject(h);
                redis.StringSet(userKeyuh, serializedUserh);

            }
            #endregion

            lblPlayerMoney.Text = "10000"; lblPlayerMoney.Visible = true;
            lblNovac.Text = "0"; lblNovac.Visible = true;
            btnDeal.Enabled = true;
            OnemoguciKontrole();
            BetsAllowed(true);
            btnClearOklada.Enabled = true;

            btnNovaIgra.Enabled = false;

        }


    }
}
