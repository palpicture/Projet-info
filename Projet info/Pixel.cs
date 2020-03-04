using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_info
{
    class Pixel
    {
        byte[] tab;

        public Pixel (byte r, byte g, byte b)
        {
            this.tab = new byte[3] { r, g, b };
        }

        public byte Red { get { return this.tab[0]; } set { this.tab[0]=value; } }
        public byte Green { get { return this.tab[1]; } set { this.tab[1] = value; } }
        public byte Blue { get { return this.tab[2]; } set { this.tab[2] = value; } }

        public void ConvertToGris()
        {
            byte gris = (byte)(0.299 * Red + 0.587 * Green + 0.114 * Blue);
            Red=Green=Blue=gris;
        }
        
    }
}
