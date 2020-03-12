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
            image.Rot(27);
            image.From_Image_To_File("COCOrot.bmp");
            Console.ReadKey();
        }
    }
}
