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
        private static Bullet _bullet;
        private static Asteroid[] _asteroids;
        private static Ship _ship = new Ship(new Point(10, 400), new Point(5, 5), new Size(10, 10));
        private static Timer _timer = new Timer { Interval = 100 };
        private static HealthPack[] _healthPack;

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
            if (e.KeyCode == Keys.ControlKey) _bullet = new Bullet(new Point(_ship.rect.X + 10, _ship.rect.Y + 4), new Point(8, 0), new Size(4, 1));
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
            _asteroids = new Asteroid[150];
            _healthPack = new HealthPack[10];
            for (var i = 0; i < _objs.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _objs[i] = new Star(new Point(70,70), new Point(-r, r), new Size(3, 3));                
                _objs[i] = new Star(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Height, 1000)), new Point(-r, r), new Size(3, 3));                
            }
            for (var i = 0; i < _asteroids.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _asteroids[i] = new Asteroid(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Height, 1000)), new Point(-r / 5, r), new Size(r, r)); 
            }
            for (var i = 0; i < _healthPack.Length; i++)
            {
                _healthPack[i] = new HealthPack(new Point(rnd.Next(Game.Width, 1000), rnd.Next(Game.Width, 1000)), new Point(-3, 0), new Size(10, 10));
            }
        }
        public static void Draw()
        {
            buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            foreach (Asteroid a in _asteroids)
            {
                a?.Draw();
            }
            foreach (HealthPack hp in _healthPack)
            {
                hp?.Draw();
            }
            _bullet?.Draw();
            _ship?.Draw();
            if (_ship != null)
                buffer.Graphics.DrawString("Здоровье:" + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
                buffer.Graphics.DrawString("Очки за астероиды:" + _ship.Score, SystemFonts.DefaultFont, Brushes.White, 0, 12);
            buffer.Render();
        }

        public static void Update()
        {
            foreach (BaseObject obj in _objs) obj.Update();
            _bullet?.Update();
            for (var i = 0; i < _asteroids.Length; i++)
            {
                if (_asteroids[i] == null) continue;
                _asteroids[i].Update();
                if (_bullet != null && _bullet.Collision(_asteroids[i]))
                {
                    System.Media.SystemSounds.Hand.Play();
                    _ship.IncScore(5);
                    _asteroids[i] = null;
                    _bullet = null;
                    continue;
                }
                if (!_ship.Collision(_asteroids[i])) continue;
                var rnd = new Random();
                _ship?.EnergyLow(rnd.Next(1, 10));
                System.Media.SystemSounds.Asterisk.Play();
                if (_ship.Energy <= 0) _ship?.Die();
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
