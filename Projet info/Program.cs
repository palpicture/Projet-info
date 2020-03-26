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
            //image.Rotation1(27);
            //image.From_Image_To_File("COCOrot.bmp");
            MyImage fract = new MyImage(4000, 4000);
            fract.From_Image_To_File("fract.bmp");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
