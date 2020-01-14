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
using System.Drawing.Drawing2D;
using Blackjack;

namespace OTTER
{
    #region SpriteCostume
    public class SpriteCostume
    {
        private Bitmap rotated;

        public Bitmap RotatedCostume
        {
            get { return rotated; }
            set { rotated = value; }
        }
        private int angle;

        public int CostumeAngle
        {
            get { return angle; }
            set { angle = value; }
        }

        public SpriteCostume(Bitmap bitmap, int degrees)
        {
            RotatedCostume = bitmap;
            CostumeAngle = degrees;
        }
    }
    #endregion

  
    public class Sprite
    {
       
        #region rotation
        
        public enum RotationStylesType 
        { 
          
            LeftRight, 
         
            DontRotate, 
          
            AllAround };
       
        public enum DirectionsType { up = 0, right = 90, left = 270, down = 180 };
        #endregion

        #region properties

        //private
        private int width, heigth;
        private int Direction; 

          public int X
        {
            get { return x; }
            set
            {
                this.x = value;
            }
        }
             public int Y
        {
            get { return y; }
            set
            {
          
                    this.y = value;
            }
        }

     
        public int Width
        {
            get { return width; }
            set
            {
                if (value <= 0)
                    width = 100;
                else
                    width = value;
            }
        }

      
        public int Heigth
        {
            get { return heigth; }
            set
            {
                if (value <= 0)
                    heigth = 100;
                else
                    heigth = value;
            }
        }

      
        public string RotationStyle;
        public bool Show; //show
        public List<Bitmap> Costumes = new List<Bitmap>();
       
        public Bitmap CurrentCostume;
        public int CostumeIndex;
        public string CostumeName;
        public int Size;
        public int SpriteIndex;

        //potrebno radi crtanja
        private Bitmap Kopija; 
        private int h2, w2;

          public string Name;

        #endregion

        #region constructors

        private int originalWidth, originalHeigth;
        private List<SpriteCostume> Rotations;

                public Sprite(string spriteImage, int posX, int posY)
        {
            CurrentCostume = new Bitmap(spriteImage);
            Kopija = new Bitmap(spriteImage);

            h2 = Kopija.Height / 2;
            w2 = Kopija.Width / 2;

            CostumeName = spriteImage;
            X = posX;
            Y = posY;
            Width = CurrentCostume.Width;
            Heigth = CurrentCostume.Height;
            Show = true;
            SpriteIndex = -1;
            Costumes.Add(new Bitmap(spriteImage));
            CostumeIndex = 0;
            RotationStyle = RotationStylesType.DontRotate.ToString();
            Direction = 0;
            Size = 100;
            originalWidth = width;
            originalHeigth = heigth;
            
          
            int numOfRotations = 36;

            if (numOfRotations > 360)
                numOfRotations %= 360;
            if (numOfRotations < 1)
                numOfRotations = 1;
            Rotations = new List<SpriteCostume>();

            Bitmap original = new Bitmap(spriteImage);

            int ang = 360 / numOfRotations;
            for (int i = 0; i < numOfRotations; i++)
            {
                PointF offset = new PointF(this.Width / 2, this.Heigth / 2);
                Bitmap b = RotateImage(original, offset, ang * i);
                SpriteCostume s = new SpriteCostume(b, i * ang);
                Rotations.Add(s);
            }

        }

        public Sprite(string spriteImage, int posX, int posY,int sirina,int visina,string name)//moj novi kontruktor,razlika je samo sta postavimo sami sirinu i visinu,te prima ime spritea
        {
            CurrentCostume = new Bitmap(spriteImage);
            Kopija = new Bitmap(spriteImage);

            h2 = Kopija.Height / 2;
            w2 = Kopija.Width / 2;

            CostumeName = spriteImage;
            X = posX;
            Y = posY;
            Width = sirina;
            Heigth = visina;
            Show = true;
            SpriteIndex = -1;
            Costumes.Add(new Bitmap(spriteImage));
            CostumeIndex = 0;
            RotationStyle = RotationStylesType.DontRotate.ToString();
            Direction = 0;
            Size = 100;
            originalWidth = width;
            originalHeigth = heigth;
            
           
            if (name.ToLower().Trim() == "card" || name.ToLower().Trim() == "karta")
            {
                Name = "card";
            }
            else Name = "pozadina";

        
            int numOfRotations = 36;

            if (numOfRotations > 360)
                numOfRotations %= 360;
            if (numOfRotations < 1)
                numOfRotations = 1;
            Rotations = new List<SpriteCostume>();

            Bitmap original = new Bitmap(spriteImage);

            int ang = 360 / numOfRotations;
            for (int i = 0; i < numOfRotations; i++)
            {
                PointF offset = new PointF(this.Width / 2, this.Heigth / 2);
                Bitmap b = RotateImage(original, offset, ang * i);
                SpriteCostume s = new SpriteCostume(b, i * ang);
                Rotations.Add(s);
            }
        }       

        #endregion

        #region methods

      
        public int GetDirection()
        {
            return Direction;
        }

       
        public int GetHeading()
        {
            return Direction;
        }

       
        public void SetDirection(int direction)
        {
            SetHeading(direction);
        }

      
        public int CenterX()
        {
            return X + Width / 2;
        }

      
        public int CenterY()
        {
            return Y + Heigth / 2;
        }        

     
        public void GotoXY(int posX, int posY)
        {
            this.X = posX;
            this.Y = posY;
        }

     
        public void SetX(int posX)
        {
            this.X = posX;
        }

        public void SetY(int posY)
        {
            this.Y = posY;
        }

          public void Goto_Sprite(Sprite sprite)
        {
         
            int rx, ry;
            rx = this.CenterX() - sprite.CenterX();
            ry = this.CenterY() - sprite.CenterY();

            this.X -= rx;
            this.Y -= ry;
        }

       
        public void Goto_MousePoint(Point mouse)
        {
            X = mouse.X - Width / 2;
            Y = mouse.Y - Heigth / 2;
        }

       
        public void ChangeX(int n)
        {
            if (CheckEdgeX(X + n))
                X += n;
            
        }

       
        public void ChangeY(int n)
        {
            if (CheckEdgeY(Y + n))
                Y += n;           
        }

        private bool CheckEdgeX(int posX)
        {
            if (posX + Width >= 700 || posX <= 0)
                return false;
            else
                return true;
        }

        private bool CheckEdgeY(int posY)
        {
            if (posY + Heigth >= 500 || posY <= 0)
                return false;
            else
                return true;
        }        
        

       
        public void SetHeading(DirectionsType heading)
        {
            int d = (int)heading;
            SetHeading(d);
        }

             public void MoveSimple(int steps)
        {
            if (this.Direction == (int)DirectionsType.right)
                this.X += steps;
            else if (this.Direction == (int)DirectionsType.left)
                this.X -= steps;
            else if (this.Direction == (int)DirectionsType.up)
                this.Y -= steps;
            else
                this.Y += steps;
        }

      
        public void SetHeading(int newDirectionAngle)
        {
            if (Direction == newDirectionAngle)
                return;
            if (newDirectionAngle < 0)
            {
                while (newDirectionAngle < 0)
                    newDirectionAngle += 360;
            }

            newDirectionAngle %= 360;
            Direction = newDirectionAngle;
            if (CostumeIndex > 0)
                return;
            if (RotationStyle == RotationStylesType.LeftRight.ToString())
            {
                return;
            }
            else if (RotationStyle == RotationStylesType.DontRotate.ToString())
            {
                return;
            }
           
            int min = 0;
            int max = 0;
            bool tocanKut = false;
            for (int i = 0; i < Rotations.Count; i++)
            {
                if (Rotations[i].CostumeAngle == newDirectionAngle)
                {
                    this.CurrentCostume = Rotations[i].RotatedCostume;
                    tocanKut = true;
                    break;
                }
                else if (Rotations[i].CostumeAngle < newDirectionAngle)
                    min = i;
                else
                {
                    max = i;
                    break;
                }
            }

           
            if (!tocanKut)
            {
                int r1 = Math.Abs(newDirectionAngle - Rotations[min].CostumeAngle);
                int r2 = Math.Abs(newDirectionAngle - Rotations[max].CostumeAngle);
                if (r1 <= r2)
                {
                    this.CurrentCostume = Rotations[min].RotatedCostume;
                }
                else
                    this.CurrentCostume = Rotations[max].RotatedCostume;
            }
        }           

        private static Bitmap RotateImage(Image image, PointF offset, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

          
            Bitmap rotatedBmp;
        
            {
                rotatedBmp = new Bitmap(image.Width, image.Height);

                rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            }

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            return rotatedBmp;
        }

     
        public void PointToMouse(Point mis)
        {
            
            int ax = this.CenterX();
            int ay = this.CenterY();

            int bx = mis.X;
            int by = mis.Y;

            double a, b, kutS, kutR;

            a = Math.Abs(bx - ax);
            b = Math.Abs(by - ay);

            kutR = Math.Atan(b / a);
            kutS = kutR * (180 / Math.PI);

            int kut = (int)kutS;

            if (ax < bx && ay > by) //I
                kut = 90 - kut;
            else if (ax > bx && ay < by) //III
                kut = 270 - kut;
            else if (ax > bx && ay > by) //II
                kut = 270 + kut;
            else if (ax < bx && ay < by) //IV
                kut = 90 + kut;
            else if (kut == 0 && ay == by)
            {
                if (ax <= bx)
                    kut = 90;
                else
                    kut = 270;
            }
            else if (kut == 90 && ax == bx)
            {
                if (ay < by)
                    kut = 180;
                else
                    kut = 0;
            }

            if (a == 0 && b == 0)
                kut = 0;
          

            SetHeading(kut);
        }

       
        public void PointToSprite(Sprite sprite)
        {
            Point p = new Point(sprite.CenterX(), sprite.CenterY());
            PointToMouse(p);
        }

    
        public void MoveSteps(int steps)
        {
            if (Direction == 0 || Direction == 90 || Direction == 180 || Direction == 270)
            {
                MoveSimple(steps);
                return;
            }

            double a, b, c, rad;
            c = Math.Abs(steps);

            int angle = Direction;

            rad = angle * (Math.PI / 180);

            a = c * Math.Sin(rad);
            b = c * Math.Cos(rad);

            a = Math.Abs(a);
            b = Math.Abs(b);

            if (steps < 0)
            {
                a = -a;
                b = -b;
            }

            if (angle > 0 && angle < 90)
            {
                x += (int)a;
                y -= (int)b;
            }
            else if (angle > 90 && angle < 180)
            {
                x += (int)a;
                y += (int)b;
            }
            else if (angle > 180 && angle < 270)
            {
                x -= (int)a;
                y += (int)b;
            }
            else
            {
                x -= (int)a;
                y -= (int)b;
            }


        }

     
        public void SetTransparentColor(int r, int g, int b)
        {
            this.CurrentCostume.MakeTransparent(Color.FromArgb(r, g, b));
        }

     

        public void SetTransparentColor(Color color)
        {            
            this.CurrentCostume.MakeTransparent(color);
        }

      
        public void SetSize(int size)
        {
            Size = size;

            float sx = float.Parse(this.Width.ToString());
            float sy = float.Parse(this.Heigth.ToString());
            float nsx = ((sx / 100) * size);
            float nsy = ((sy / 100) * size);

            if (Size == 100)
            {
                nsx = originalWidth;
                nsy = originalHeigth;
            }

            this.Width = Convert.ToInt32(nsx);
            this.Heigth = Convert.ToInt32(nsy);
        }

     
        public void SetVisible(bool value)
        {
            this.Show = value;
        }

       
        private void FlipSprite(string fliptype)
        {
            if (fliptype.ToLower() == "none")
            {
                foreach (Bitmap b in Costumes)
                    b.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                return;
            }

            if (fliptype.ToLower() == "horizontal")
            {
                foreach (Bitmap b in Costumes)
                {
                    //lock(b)
                    b.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                return;
            }

            if (fliptype.ToLower() == "horizontalvertical")
            {
                foreach (Bitmap b in Costumes)
                    b.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                return;
            }

            if (fliptype.ToLower() == "vertical")
            {
                foreach (Bitmap b in Costumes)
                    b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return;
            }
        }

        private void ChangeImage(string fileName)
        {
            CurrentCostume = new Bitmap(fileName);
            CostumeName = fileName;
            Width = CurrentCostume.Width;
            Heigth = CurrentCostume.Height;
        }

        private void ChangeImage(string fileName, int w, int h)
        {
            CurrentCostume = new Bitmap(fileName);
            CostumeName = fileName;
            Width = w;
            Heigth = h;
        }

       
        public virtual void AddCostumes(params string[] files)
        {
            foreach (string f in files)
            {
                Costumes.Add(new Bitmap(f));
            }
        }

      
        public void NextCostume()
        {
            if (CostumeIndex + 1 == Costumes.Count)
            {
                CostumeIndex = 0;
                CurrentCostume = Costumes[0];
            }
            else
            {
                CostumeIndex++;
                CurrentCostume = Costumes[CostumeIndex];
            }
        }
        
    
        
        private bool TouchingRightEdge()
        {
            if (this.X + this.Width >= 700)
                return true;
            else
                return false;
        }

        private bool TouchingLeftEdge()
        {
            if (this.X <= 0)
                return true;
            else
                return false;
        }

        private bool TouchingBottomEdge()
        {
            if (this.Y + this.Heigth >= 500)
                return true;
            else
                return false;
        }

        private bool TouchingTopEdge()
        {
            if (this.Y <= 0)
                return true;
            else
                return false;
        }

      
        public bool TouchingEdge()
        {
            if (TouchingLeftEdge())
            {
                return true;
            }
            if (TouchingRightEdge())
            {
                return true;
            }
            if (TouchingBottomEdge())
            {
                return true;
            }
            if (TouchingTopEdge())
            {
                return true;
            }

            return false;
        }

       
        
        public bool TouchingEdge(out string edge)
        {
            if (TouchingLeftEdge())
            {
                edge = "left";
                return true;
            }
            if (TouchingRightEdge())
            {
                edge = "right";
                return true;
            }
            if (TouchingBottomEdge())
            {
                edge = "bottom";
                return true;
            }
            if (TouchingTopEdge())
            {
                edge = "top";
                return true;
            }

            edge = "";
            return false;
        }

      
        public bool TouchingSprite(Sprite b)
        {
            Sprite a = this;

            Rectangle A = new Rectangle(a.X, a.Y, a.Width, a.Heigth);
            Rectangle B = new Rectangle(b.X, b.Y, b.Width, b.Heigth);

            if (A.IntersectsWith(B))
                return true;
            else
                return false;
        }

        private bool TouchingSprite_ne(Sprite b, int offset)
        {
            Sprite a = this;

            double x1 = a.CenterX();//a.x + a.width / 2;
            double y1 = a.CenterY();//a.y + a.heigth / 2;
            double x2 = b.CenterX();//b.x + b.width / 2;
            double y2 = b.CenterY();//b.y + b.heigth / 2;

            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            int ma = a.Width <= a.Heigth ? a.Width : a.Heigth;
            int mb = b.Width <= b.Heigth ? b.Width : b.Heigth;

            double ra = ma / 2;
            double rb = mb / 2;

            if (d + offset > ra + rb)
                return false;
            else
                return true;
        }

       
        public bool TouchingMousePoint(Point m)
        {
            Sprite a = this;
            int offset = 1;

            double x1 = a.X + a.Width / 2;
            double y1 = a.Y + a.Heigth / 2;
            double x2 = m.X;
            double y2 = m.Y;

            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            int ma = a.Width < a.Heigth ? a.Width : a.Heigth;
            int mb = 0;

            double ra = ma / 2;
            double rb = mb / 2;

            if (d + offset > ra + rb)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Provjerava je li lik kliknut. Za provjeru je potrebno poslati koordinate strelice miša. 
        /// Koordinate (točka) miša se mogu dobiti iz "sensing".
        /// </summary>
        /// <param name="mousePoint">Koordinate strelice miša.</param>
        /// <returns></returns>
        public bool Clicked(Point mousePoint)
        {
            if (mousePoint.X > this.X && mousePoint.X < this.X + this.Width)
            {
                if (mousePoint.Y > this.Y && mousePoint.Y < this.Y + this.Heigth)
                    return true;
            }

            return false;
        }


        /* Control */
        private void Wait(double seconds)
        {
            int ms = (int)(seconds * 1000);
            Thread.Sleep(ms);
        }

        #endregion

        private int x;
        private int y;

       
       
    }
}
