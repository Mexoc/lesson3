using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BrandNewShip
{
    class HealthPack: Asteroid
    {        
        public HealthPack(Point pos, Point dir, Size size) : base(pos, dir, size)
        {           
        }

        public override void Draw()
        {
            Game.buffer.Graphics.FillEllipse(Brushes.Green, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
            Pos.X = Pos.X + Dir.X;
            if (Pos.X < -Size.Width) Pos.X = Game.Width + Size.Width;            
        }
    }
}
