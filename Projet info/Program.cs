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
            for (int i = 0; i < image.header.Length; i++) { Console.Write(image.header[i]+" "); }
            Console.WriteLine();
            image.Rotation1(27);
            for (int i = 0; i < image.header.Length; i++) { Console.Write(image.header[i]+" "); }
            Console.WriteLine();
            image.From_Image_To_File("COCOrot.bmp");
            //MyImage fract = new MyImage(4000, 4000);
            //fract.From_Image_To_File("fract.bmp");
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
