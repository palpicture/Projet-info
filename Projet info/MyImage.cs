using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Projet_info
{
    class MyImage
    {
        string type;
        int taille;
        int large;
        int haut;
        int offset;
        int couleur;
        Pixel[,] image;
        byte[] header;

        public MyImage(string fichier)
        {
            byte[] image = File.ReadAllBytes(fichier);
            this.type = Encoding.ASCII.GetString(new byte[] { image[0], image[1] });
            this.taille = image.Count();
            this.large = Convertir_Endian_To_Int(new byte[] { image[18], image[19], image[20], image[21] });
            this.haut = Convertir_Endian_To_Int(new byte[] { image[22], image[23], image[24], image[25] });
            this.offset = Convertir_Endian_To_Int(new byte[] { image[14], image[15], image[16], image[17] });
            this.couleur = Convertir_Endian_To_Int(new byte[] { image[28], image[29] }) / 8;
            this.image = GoImage(image, large, haut, offset, taille);
            this.header = new byte[54];
            for(int i = 0; i < 54; i++) { header[i] = image[i]; }

        }

        private int Convertir_Endian_To_Int(byte[] tab)
        {
            int i = 1;
            int tot = 0;
            foreach (int a in tab)
            {
                tot += a * i;
                i *= 256;
            }
            return tot;
        }

        private byte[] Convertir_Int_To_Endian(int val, int nboctet)
        {
            int i = val;
            byte[] tab= new byte[nboctet];
            for (int a=0; a<nboctet; a++)
            {
                tab[a] = Convert.ToByte(i%256);
                i -= tab[a]*(int)(Math.Pow(256,a));
            }
            return tab;
        }


        private Pixel[,] GoImage(byte[] image, int large, int haut, int offset, int taille)
        {
            Pixel[,] tab = new Pixel[haut, large];
            int i = offset + 14;
            for (int y = 0;y<haut;y++)
            {
                for (int x=0;x<large;x++)
                {
                    tab[y, x] = new Pixel(image[i], image[i + 1], image[i + 2]);
                    i += 3;
                }
            }
            return tab;
        }

        public void From_Image_To_File (string file)
        {
            byte[] myfile = new byte[this.taille];
            for(int i =0;i<54;i++)
            {
                myfile[i] = header[i];
            }
            int index = 54;
            for (int y=0;y<haut;y++)
            {
                for(int x =0;x<=large-3;x++)
                {
                    myfile[index] = image[y,x].Red;
                    myfile[index+1] = image[y, x].Green;
                    myfile[index+2] = image[y, x].Blue;
                    index += 3;
                }
            }
            File.WriteAllBytes(file, myfile);
        } 

        public void ConvertToGris()
        {
            for (int j=0; j<this.haut; j++)
            {
                for (int i = 0;i<this.large;i++)
                {
                    image[j, i].ConvertToGris();
                }
            }
        }


    }
}
