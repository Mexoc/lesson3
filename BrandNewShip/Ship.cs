using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BrandNewShip
{
    class Ship: BaseObject
    {
        static private int _energy = 100;
        static private int _score = 0;
        public int Score => _score; 
        public int Energy => _energy;        
        
        public static event Message MessageDie;

        public void EnergyLow(int n)
        {
            _energy -= n;
        }

        public void IncScore(int n)
        {
            _score += n;
        }

        public void FullEnergy()
        {
            _energy -= _energy - 100;
        }

        public void EnergyUp(int n)
        {
            _energy += n;
        }

        public Ship(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }

        public override void Draw()
        {
            Game.buffer.Graphics.FillEllipse(Brushes.Red, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
        }

        public void Up()
        {
            if (Pos.Y > 0) Pos.Y = Pos.Y - Dir.Y;
        }

        public void Down()
        {
            if (Pos.Y < Game.Height) Pos.Y = Pos.Y + Dir.Y;
        }

        public void Die()
        {
            MessageDie?.Invoke();     
        }
    }
}
