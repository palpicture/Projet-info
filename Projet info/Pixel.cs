using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_info
{
    class Pixel
    {
        //Attributs
        byte[] tab;

        //Constructeur
        public Pixel (byte r, byte g, byte b)
        {
            this.tab = new byte[3] { r, g, b };
        }

        //Renvoie la valeur du rouge
        public byte Red { get { return this.tab[0]; } set { this.tab[0]=value; } }

        //Renvoie la valeur du vert
        public byte Green { get { return this.tab[1]; } set { this.tab[1] = value; } }

        //Renvoie la valeur du bleu
        public byte Blue { get { return this.tab[2]; } set { this.tab[2] = value; } }

        //Permet de convertir les couleurs en nuances de gris
        public void ConvertToGris()
        {
            byte gris = (byte)(0.299 * Red + 0.587 * Green + 0.114 * Blue);
            tab[0] = gris;
            tab[1] = gris;
            tab[2] = gris;
        }
        
    }
}
