using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_info
{
    class Program
    {
        static void Main(string[] args)
        {
            MyImage image = new MyImage("coco.bmp");
            //image.ConvertToGris();
            image.Agrandissement(3);
            image.From_Image_To_File("COCOgrand.bmp");
            Console.ReadKey();
        }

        /// avoir fini de déboguer le pojet et avoir fini miroir, rotation changement de taille et ,=niveaux de gris
    }
}
