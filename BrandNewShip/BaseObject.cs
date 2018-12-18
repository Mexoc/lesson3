using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BrandNewShip
{
    abstract class BaseObject: ICollision
    {
        public Point Pos; //координаты начальные
        public Point Dir;   // направление движения       
        public Size Size; //размер объекта
        public delegate void Message();

        protected BaseObject(Point pos, Point dir, Size size)
        {
            Pos = pos;
            Dir = dir;
            Size = size;
        }

        public abstract void Draw();        

        virtual public void Update()
        {
            Pos.X = Pos.X + Dir.X;
            if (Pos.X < 0) Pos.X = Game.Width + Size.Width;            
        }

        public Rectangle rect => new Rectangle(Pos, Size);

        public bool Collision(ICollision o) => o.rect.IntersectsWith(this.rect);
    }
}
