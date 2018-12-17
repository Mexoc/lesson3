using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BrandNewShip
{
    class Star : BaseObject
    {
        public Star(Point pos, Point dir, Size size) : base(pos, dir, size)
        {            
        }

        public override void Draw()
        {
            Game.buffer.Graphics.DrawLine(Pens.White, Pos.X - Size.Width, Pos.Y + Size.Height, Pos.X + Size.Width, Pos.Y - Size.Height);
            Game.buffer.Graphics.DrawLine(Pens.White, Pos.X + Size.Width, Pos.Y + Size.Height, Pos.X - Size.Width, Pos.Y - Size.Height);
        }

        public override void Update()
        {           
            Pos.X = Pos.X + Dir.X;
            if (Pos.X < 0) Pos.X = Game.Width;
            if (Pos.X > Game.Width) Dir.X = -Dir.X;
        }
    }
}

