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
            //MyImage image = new MyImage("coco.bmp");
            //image.ConvertToGris();
            //image.Rotation(27);
            //image.From_Image_To_File("COCOrot.bmp");
            MyImage fract = new MyImage(1000, 1000, 56);
            fract.From_Image_To_File("Fract.bmp");
            Console.ReadKey();
        }
    }
}
