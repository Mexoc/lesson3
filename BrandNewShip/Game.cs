using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BrandNewShip
{
    static class Game
    {
        public static BufferedGraphics buffer;
        public static BufferedGraphicsContext context;
        public static Graphics g;        
        private static int width;
        private static int height;
        private static Random rnd = new Random();
        private static BaseObject[] _objs;
        private static List<Bullet> _bullets = new List<Bullet>();
        private static List<Asteroid> Asteroids = new List<Asteroid>();
        private static List<Asteroid> AsteroidsNew = new List<Asteroid>();
        private static List<Asteroid> AsteroidsNew2 = new List<Asteroid>();
        private static List<Asteroid> AsteroidsNew3 = new List<Asteroid>();
        private static Ship _ship = new Ship(new Point(10, 400), new Point(5, 5), new Size(10, 10));
        private static Timer _timer = new Timer { Interval = 100 };
        private static HealthPack[] _healthPack;
        private static int n = 100;

        public static int Width
        {
            get { return width; }
            set { width = value; }
        }

        public static int Height
        {
            get { return height; }
            set { height = value; }
        }

        public static void Init(Form form)
        {
            context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            Load();
            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;
            if (Width > 984 || Height > 962 || Width < 132 || Height < 38) throw new ArgumentOutOfRangeException("Размер превышен");            
            form.KeyDown += Form_KeyDown;            
            _timer.Start();
            _timer.Tick += Timer_Tick;
            Ship.MessageDie += Finish;    
        }      
        
        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullets.Add(new Bullet(new Point(_ship.rect.X + 10, _ship.rect.Y + 4), new Point(25, 0), new Size(4, 1)));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
            if (e.KeyCode == Keys.Escape) Application.Exit();
        }
        
        public static void Load()
        {
            _objs = new BaseObject[200];
            _healthPack = new HealthPack[10];
            for (var i = 0; i < _objs.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _objs[i] = new Star(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Height, 1000)), new Point(-r, r), new Size(2, 2));
            }
            LoadAsteroids(Asteroids);
            LoadAsteroids(AsteroidsNew);
            LoadAsteroids(AsteroidsNew2);
            LoadAsteroids(AsteroidsNew3);          
            for (var i = 0; i < _healthPack.Length; i++)
            {
                _healthPack[i] = new HealthPack(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Width, 1000)), new Point(-3, 0), new Size(10, 10));
            }                     
        }
        
        public static void LoadAsteroids(List<Asteroid> Asteroids)
        {
            for (var i = 0; i < n; i++)
            {
                int r = rnd.Next(20, 50);
                Asteroids.Add(new Asteroid(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Height, 800)), new Point(- n / 5, n/2), new Size(r, r)));
            }
            n++;
        }

        //public static void UpdateAsteroids(List<Asteroid> Asteroids)
        //{
        //    foreach (var a in Asteroids)
        //    {
        //        a?.Update();
        //        if (_ship.Collision(a))
        //        {
        //            _ship.EnergyLow(rnd.Next(1, 10));
        //            Console.WriteLine("Столкновение с астероидом");
        //            System.Media.SystemSounds.Asterisk.Play();
        //            if (_ship.Energy <= 0) _ship.Die();
        //        }
        //    }
        //    foreach (var a in _bullets)
        //    {
        //        a.Update();
        //        for (var j = Asteroids.Count - 1; j >= 0; j--)
        //        {
        //            if (a.Collision(Asteroids[j]))
        //            {
        //                Console.WriteLine("Астероид сбит");
        //                int r = rnd.Next(5, 50);
        //                System.Media.SystemSounds.Hand.Play();
        //                _ship.IncScore(5);                        
        //                Asteroids.RemoveAt(j);
        //                j--;
        //                continue;
        //            }
        //        }
        //    }
        //}

        public static void UpdateAsteroids(List<Asteroid> Asteroids)
        {
            bool flag;
            foreach (var a in Asteroids)
            {
                a?.Update();
                if (_ship.Collision(a))
                {
                    _ship.EnergyLow(rnd.Next(1, 10));
                    Console.WriteLine("Столкновение с астероидом");
                    System.Media.SystemSounds.Asterisk.Play();
                    if (_ship.Energy <= 0) _ship.Die();
                }
            }
            for (var i = _bullets.Count - 1; i >= 0; i--)
            {                
                _bullets[i].Update();
                for (var j = Asteroids.Count - 1; j >= 0; j--)
                {
                    flag = false;
                    if (_bullets[i].Collision(Asteroids[j]))
                    {
                        flag = true;
                        Console.WriteLine("Астероид сбит");
                        int r = rnd.Next(5, 50);
                        System.Media.SystemSounds.Hand.Play();
                        _ship.IncScore(5);
                        Asteroids.RemoveAt(j);
                        _bullets.RemoveAt(i);
                        j--;
                        break;
                    }
                }
            }
        }

        public static void Draw()
        {
            buffer = context.Allocate(g, new Rectangle(0, 0, Width, Height));
            buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();            
            foreach (Asteroid a in Asteroids)
            {
                a?.Draw();
            }
           if (Asteroids.Count == 0)
                foreach (Asteroid a in AsteroidsNew)
                {
                    a?.Draw();
                }
            if (AsteroidsNew.Count == 0)
                foreach (Asteroid a in AsteroidsNew2)
                {
                    a?.Draw();
                }
            if (AsteroidsNew2.Count == 0)
                foreach (Asteroid a in AsteroidsNew3)
                {
                    a?.Draw();
                }
            
            foreach (HealthPack hp in _healthPack)
            {
                hp?.Draw();
            }                      
            foreach (var b in _bullets) b?.Draw();
            _ship?.Draw();
            if (_ship != null)
                buffer.Graphics.DrawString("Здоровье:" + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
                buffer.Graphics.DrawString("Очки за астероиды:" + _ship.Score, SystemFonts.DefaultFont, Brushes.White, 0, 12);
            buffer.Render();
        }

        public static void Update()
        {
            foreach (BaseObject obj in _objs) obj.Update();
            UpdateAsteroids(Asteroids);
            if (Asteroids.Count == 0)
            UpdateAsteroids(AsteroidsNew);
            if (AsteroidsNew.Count == 0)
                UpdateAsteroids(AsteroidsNew2);
            if (AsteroidsNew2.Count == 0)
                UpdateAsteroids(AsteroidsNew3);               
            
            for (var i = 0; i < _healthPack.Length; i++)
            {
                _healthPack[i].Update();                
                if (_ship.Collision(_healthPack[i]))
                {
                    _healthPack[i].Pos = new Point((rnd.Next(Game.Width, 1000)), rnd.Next(Game.Width, 1000));
                    _ship.EnergyUp(rnd.Next(20, 30));
                    if (_ship.Energy > 100) _ship.FullEnergy();               
                }
            }            
        }

        public static void Finish()
        {
            _timer.Stop();
            Console.WriteLine("Корабль уничтожен");
            buffer.Graphics.DrawString("Game Over", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold), Brushes.White, 200, 100);
            buffer.Render();
        }
        
        //таймер изменения состояний
        public static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();   
            buffer.Render();
            buffer.Dispose();
        }        
    }
}
