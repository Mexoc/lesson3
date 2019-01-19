using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace BrandNewShip
{
    static class Program
    {
        static void Main()
        {
            Form form = new Form();
            form.Width = 900;
            form.Height = 900;
            Game.Init(form);
            Application.Run(form);
        }
    }
}
