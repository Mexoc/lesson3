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
        private static Ship _ship = new Ship(new Point(10, 400), new Point(5, 5), new Size(10, 10));
        private static Timer _timer = new Timer { Interval = 100 };
        private static HealthPack[] _healthPack;
        private static int n = 3;

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
        }

        //рисуем форму и закрашиваем её черным
        public static void DrawForm()
        {
            buffer = context.Allocate(g, new Rectangle(0, 0, Width, Height));
            buffer.Graphics.Clear(Color.Black);
        }

        //загружаем астероиды, звезды и пулю
        public static void Load()
        {            
            _objs = new BaseObject[200];     
            _healthPack = new HealthPack[10];
            for (var i = 0; i < _objs.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _objs[i] = new Star(new Point(70,70), new Point(-r, r), new Size(3, 3));                
                _objs[i] = new Star(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Height, 1000)), new Point(-r, r), new Size(2, 2));                
            }
            LoadAsteroids(n);           
            for (var i = 0; i < _healthPack.Length; i++)
            {
                _healthPack[i] = new HealthPack(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Width, 1000)), new Point(-3, 0), new Size(10, 10));
            }
        }

        public static void LoadAsteroids(int m)
        {
            for (var i = 0; i < m; i++)
            {
                int r = rnd.Next(5, 50);
                Asteroids.Add(new Asteroid(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Height, 1000)), new Point(-r / 5, r), new Size(r, r)));
            }          
        }

        public static void Draw()
        {
            buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            foreach (Asteroid a in Asteroids)
            {
                a?.Draw();
            }
            foreach (HealthPack hp in _healthPack)
            {
                hp?.Draw();
            }
            if (Asteroids.Count == 0)
            {
                foreach (var a in AsteroidsNew) a?.Draw();
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
            foreach(var b in _bullets) b?.Update();
            foreach (var a in Asteroids) a?.Update(); 
            if (Asteroids.Count == 0)
            {
                foreach (var a in AsteroidsNew) a?.Update();
            }
            for (var i = _bullets.Count - 1; i >= 0; i--)
            {
                for (var j = Asteroids.Count - 1; j >= 0; j--)
                {
                    if (_bullets.Count != 0 && _bullets[i].Collision(Asteroids[j]))
                    {
                        System.Media.SystemSounds.Hand.Play();
                        _ship.IncScore(5);
                        AsteroidsNew.Add(Asteroids[j]);
                        Asteroids.Remove(Asteroids[j]);
                        _bullets.Remove(_bullets[i]);
                        continue;
                    }
                    if (Asteroids.Count != 0 && _ship.Collision(Asteroids[j]))
                    {
                        _ship.EnergyLow(rnd.Next(1, 10));
                        System.Media.SystemSounds.Asterisk.Play();
                        if (_ship.Energy <= 0) _ship.Die();
                    }
                }      
            }
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
            buffer.Graphics.DrawString("Game Over", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Bold), Brushes.White, 200, 100);
            buffer.Render();
        }
        

        //таймер изменения состояний
        public static void Timer_Tick(object sender, EventArgs e)
        {        
            DrawForm();           
            Draw();
            Update();   
            buffer.Render();
            buffer.Dispose();
        }        
    }
}
