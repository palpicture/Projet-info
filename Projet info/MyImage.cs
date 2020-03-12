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
        //Attributs
        string type;
        int taille;
        int large;
        int haut;
        int offset;
        int couleur;
        Pixel[,] image;
        byte[] header;

        //Constructeur
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

        //Métohde pour passer d'un tableau de byte en entier avec une boucle for sur tous les bytes du tableau
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

        //Méthode pour passer d'un entier en un tableau de bytes avec une boucle for sur le nombre d'octets sur lequel on veut faire la conversion
        private byte[] Convertir_Int_To_Endian(int val, int nboctet)
        {
            byte[] res = new byte[nboctet];
            int reste = val % 256;
            res[0] = (byte)reste;
            val -= reste;
            for (int i = nboctet - 1; i > 0; i--)
            {
                int temp = val / ((int)Math.Pow(256, i));
                res[i] = (byte)temp;
                val -=  ((int)Math.Pow(256, i) * temp);
            }
            return res;
        }

        //Méthode pour accéder aux pixels de l'image sans le header
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

        //Méthode pour passer d'une image à un fichier en recréant le header puis l'image et en le mettant dans un fichier 
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
                for(int x =0;x<large;x++)
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

        public void Agrandissement(int fact)
        {
            Console.WriteLine();
            Pixel[,] image1 = new Pixel[haut*fact, large*fact];
            for (int j = 0; j < this.haut; j++)
            {
                for (int i = 0; i < this.large; i++)
                {
                    for (int ii = 0;ii<fact;ii++)
                    {
                        for (int jj = 0; jj < fact; jj++)
                        {
                            image1[j*fact + jj, i*fact + ii] = image[j, i];
                        }
                    }      
                }
            }
            taille = (taille - 54) * fact * fact + 54;
            image = image1;
            haut *= fact;
            large *= fact;
            byte[] temp = Convertir_Int_To_Endian(large,4);
            for(int i = 0; i < 4; i++) { header[18+i] = temp[i]; }
            temp = Convertir_Int_To_Endian(haut, 4);
            for (int i = 0; i < 4; i++) { header[22 + i] = temp[i]; }
            temp = Convertir_Int_To_Endian(taille+54, 4);
            for (int i = 0; i < 4; i++) { header[2 + i] = temp[i]; }
        }

        public void Reduction(int fact)
        {
            Console.WriteLine();
            Pixel[,] image1 = new Pixel[haut /fact, large /fact];
            for (int j = 0; j < this.haut/fact; j++)
            {
                for (int i = 0; i < this.large/fact; i++)
                {
                    for (int ii = 0; ii < fact; ii++)
                    {
                            image1[j, i] = image[j*fact, i*fact];
                    }

                }
            }
            taille = (taille - 54) / (fact * fact) + 54;
            image = image1;
            haut /= fact;
            large /= fact;
            byte[] temp = Convertir_Int_To_Endian(large, 4);
            for (int i = 0; i < 4; i++) { header[18 + i] = temp[i]; }
            temp = Convertir_Int_To_Endian(haut, 4);
            for (int i = 0; i < 4; i++) { header[22 + i] = temp[i]; }
            temp = Convertir_Int_To_Endian(taille + 54, 4);
            for (int i = 0; i < 4; i++) { header[2 + i] = temp[i]; }
        }

        public void Miroir(char x)
        {
            Pixel[,] image1=new Pixel[image.GetLength(0), image.GetLength(1)];
            if (x == 'H')
            {
                for (int j = 0; j < this.haut; j++)
                {
                    for (int i = 0; i < this.large; i++)
                    {
                        image1[j, i] = image[j, this.large - 1 - i];
                    }
                }
                image = image1;
            }



            else if (x == 'V')
            {
                for (int i = 0; i < this.large; i++)
                {
                    for (int j = 0; j < this.haut; j++)
                    {
                        image1[j, i] = image[this.haut -1 - j, i];
                    }
                }
                image = image1;
            }



            else
            {
                Console.WriteLine("Vous n'avez pas choisi V ou H");
            }
        }

        public void Rotationbis(int angle)
        {
            Pixel[,] image1 = new Pixel[image.GetLength(0), image.GetLength(1)];
            for (int i = 0; i < this.haut; i++)
            {
                for (int j = 0; j < this.large; j++)
                {
                    int tempj = Convert.ToInt32(i * Math.Cos(angle) - j * Math.Sin(angle));
                    int tempi = Convert.ToInt32(i * Math.Sin(angle) + j * Math.Cos(angle));
                    if (tempi < large && tempj < haut && 0 < tempi && 0 < tempj) { image1[i, j] = image[tempj, tempi]; }
                    else { image1[i, j] = new Pixel((byte)0, (byte)0, (byte)0); }
                }
            }
            image = image1;
        }

        public void Convolution(int[,] matrice)
        {
            Pixel[,] temp = new Pixel[haut, large];
            if (matrice.GetLength(0) == 3 && matrice.GetLength(1) == 3)
            {
                for (int i = 0; i < large; i++)
                {
                    for (int j = 0; j < haut; j++)
                    {
                        temp[j, i] = Operation(matrice, i, j);
                    }
                }
                image = temp;
            }
            else { Console.WriteLine("erreur"); }
        }

        public void Flou()
        {
            int[,] matrice = new int[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            Convolution(matrice);
        }

        public void DetecBord()
        {
            int[,] matrice = new int[3, 3] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            Convolution(matrice);
        }

        public void RenfocementBords()
        {
            int[,] matrice = new int[3, 3] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
            Convolution(matrice);
        }

        public void Repoussage()
        {
            int[,] matrice = new int[3, 3] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            Convolution(matrice);
        }

        private Pixel Operation(int[,] matrice, int i, int j)
        {
            byte r;
            byte g;
            byte b;
            if (i == 0)
            {
                if (j == 0)
                {
                    r = (byte)((image[j, i].Red * matrice[1, 1] + image[j, i + 1].Red * matrice[1, 2] + image[j + 1, i].Red * matrice[2, 1] + image[j + 1, i + 1].Red * matrice[2, 2]) / 4);
                    g = (byte)((image[j, i].Green * matrice[1, 1] + image[j, i + 1].Green * matrice[1, 2] + image[j + 1, i].Green * matrice[2, 1] + image[j + 1, i + 1].Green * matrice[2, 2]) / 4);
                    b = (byte)((image[j, i].Blue * matrice[1, 1] + image[j, i + 1].Blue * matrice[1, 2] + image[j + 1, i].Blue * matrice[2, 1] + image[j + 1, i + 1].Blue * matrice[2, 2]) / 4);
                }
                else if (j == haut - 1)
                {
                    r = (byte)((image[j - 1, i].Red * matrice[0, 1] + image[j - 1, i + 1].Red * matrice[0, 2] + image[j, i].Red * matrice[1, 1] + image[j, i + 1].Red * matrice[1, 2]) / 4);
                    g = (byte)((image[j - 1, i].Green * matrice[0, 1] + image[j - 1, i + 1].Green * matrice[0, 2] + image[j, i].Green * matrice[1, 1] + image[j, i + 1].Green * matrice[1, 2]) / 4);
                    b = (byte)((image[j - 1, i].Blue * matrice[0, 1] + image[j - 1, i + 1].Blue * matrice[0, 2] + image[j, i].Blue * matrice[1, 1] + image[j, i + 1].Blue * matrice[1, 2]) / 4);
                }
                else
                {
                    r = (byte)((image[j - 1, i].Red * matrice[0, 1] + image[j - 1, i + 1].Red * matrice[0, 2] + image[j, i].Red * matrice[1, 1] + image[j, i + 1].Red * matrice[1, 2] + image[j + 1, i].Red * matrice[2, 1] + image[j + 1, i + 1].Red * matrice[2, 2]) / 6);
                    g = (byte)((image[j - 1, i].Green * matrice[0, 1] + image[j - 1, i + 1].Green * matrice[0, 2] + image[j, i].Green * matrice[1, 1] + image[j, i + 1].Green * matrice[1, 2] + image[j + 1, i].Green * matrice[2, 1] + image[j + 1, i + 1].Green * matrice[2, 2]) / 6);
                    b = (byte)((image[j - 1, i].Blue * matrice[0, 1] + image[j - 1, i + 1].Blue * matrice[0, 2] + image[j, i].Blue * matrice[1, 1] + image[j, i + 1].Blue * matrice[1, 2] + image[j + 1, i].Blue * matrice[2, 1] + image[j + 1, i + 1].Blue * matrice[2, 2]) / 6);
                }
            }
            else if (i == large - 1)
            {
                if (j == 0)
                {
                    r = (byte)((image[j, i - 1].Red * matrice[1, 0] + image[j, i].Red * matrice[1, 1] + image[j + 1, i - 1].Red * matrice[2, 0] + image[j + 1, i].Red * matrice[2, 1]) / 4);
                    g = (byte)((image[j, i - 1].Green * matrice[1, 0] + image[j, i].Green * matrice[1, 1] + image[j + 1, i - 1].Green * matrice[2, 0] + image[j + 1, i].Green * matrice[2, 1]) / 4);
                    b = (byte)((image[j, i - 1].Blue * matrice[1, 0] + image[j, i].Blue * matrice[1, 1] + image[j + 1, i - 1].Blue * matrice[2, 0] + image[j + 1, i].Blue * matrice[2, 1]) / 4);

                }
                else if (j == haut - 1)
                {
                    r = (byte)((image[j - 1, i - 1].Red * matrice[0, 0] + image[j - 1, i].Red * matrice[0, 1] + image[j, i - 1].Red * matrice[1, 0] + image[j, i].Red * matrice[1, 1]) / 4);
                    g = (byte)((image[j - 1, i - 1].Green * matrice[0, 0] + image[j - 1, i].Green * matrice[0, 1] + image[j, i - 1].Green * matrice[1, 0] + image[j, i].Green * matrice[1, 1]) / 4);
                    b = (byte)((image[j - 1, i - 1].Blue * matrice[0, 0] + image[j - 1, i].Blue * matrice[0, 1] + image[j, i - 1].Blue * matrice[1, 0] + image[j, i].Blue * matrice[1, 1]) / 4);
                }
                else
                {
                    r = (byte)((image[j - 1, i - 1].Red * matrice[0, 0] + image[j - 1, i].Red * matrice[0, 1] + image[j, i - 1].Red * matrice[1, 0] + image[j, i].Red * matrice[1, 1] + image[j + 1, i - 1].Red * matrice[2, 0] + image[j + 1, i].Red * matrice[2, 1]) / 6);
                    g = (byte)((image[j - 1, i - 1].Green * matrice[0, 0] + image[j - 1, i].Green * matrice[0, 1] + image[j, i - 1].Green * matrice[1, 0] + image[j, i].Green * matrice[1, 1] + image[j + 1, i - 1].Green * matrice[2, 0] + image[j + 1, i].Green * matrice[2, 1]) / 6);
                    b = (byte)((image[j - 1, i - 1].Blue * matrice[0, 0] + image[j - 1, i].Blue * matrice[0, 1] + image[j, i - 1].Blue * matrice[1, 0] + image[j, i].Blue * matrice[1, 1] + image[j + 1, i - 1].Blue * matrice[2, 0] + image[j + 1, i].Blue * matrice[2, 1]) / 6);

                }
            }
            else if (j == 0)
            {
                r = (byte)((image[j, i - 1].Red * matrice[1, 0] + image[j, i].Red * matrice[1, 1] + image[j, i + 1].Red * matrice[1, 2] + image[j + 1, i - 1].Red * matrice[2, 0] + image[j + 1, i].Red * matrice[2, 1] + image[j + 1, i + 1].Red * matrice[2, 2]) / 6);
                g = (byte)((image[j, i - 1].Green * matrice[1, 0] + image[j, i].Green * matrice[1, 1] + image[j, i + 1].Green * matrice[1, 2] + image[j + 1, i - 1].Green * matrice[2, 0] + image[j + 1, i].Green * matrice[2, 1] + image[j + 1, i + 1].Green * matrice[2, 2]) / 6);
                b = (byte)((image[j, i - 1].Blue * matrice[1, 0] + image[j, i].Blue * matrice[1, 1] + image[j, i + 1].Blue * matrice[1, 2] + image[j + 1, i - 1].Blue * matrice[2, 0] + image[j + 1, i].Blue * matrice[2, 1] + image[j + 1, i + 1].Blue * matrice[2, 2]) / 6);

            }
            else if (j == haut - 1)
            {
                r = (byte)((image[j - 1, i - 1].Red * matrice[0, 0] + image[j - 1, i].Red * matrice[0, 1] + image[j - 1, i + 1].Red * matrice[0, 2] + image[j, i - 1].Red * matrice[1, 0] + image[j, i].Red * matrice[1, 1] + image[j, i + 1].Red * matrice[1, 2]) / 6);
                g = (byte)((image[j - 1, i - 1].Green * matrice[0, 0] + image[j - 1, i].Green * matrice[0, 1] + image[j - 1, i + 1].Green * matrice[0, 2] + image[j, i - 1].Green * matrice[1, 0] + image[j, i].Green * matrice[1, 1] + image[j, i + 1].Green * matrice[1, 2]) / 6);
                b = (byte)((image[j - 1, i - 1].Blue * matrice[0, 0] + image[j - 1, i].Blue * matrice[0, 1] + image[j - 1, i + 1].Blue * matrice[0, 2] + image[j, i - 1].Blue * matrice[1, 0] + image[j, i].Blue * matrice[1, 1] + image[j, i + 1].Blue * matrice[1, 2]) / 6);
            }
            else
            {
                r = (byte)((image[j - 1, i - 1].Red * matrice[0, 0] + image[j - 1, i].Red * matrice[0, 1] + image[j - 1, i + 1].Red * matrice[0, 2] + image[j, i - 1].Red * matrice[1, 0] + image[j, i].Red * matrice[1, 1] + image[j, i + 1].Red * matrice[1, 2] + image[j + 1, i - 1].Red * matrice[2, 0] + image[j + 1, i].Red * matrice[2, 1] + image[j + 1, i + 1].Red * matrice[2, 2]) / 9);
                g = (byte)((image[j - 1, i - 1].Green * matrice[0, 0] + image[j - 1, i].Green * matrice[0, 1] + image[j - 1, i + 1].Green * matrice[0, 2] + image[j, i - 1].Green * matrice[1, 0] + image[j, i].Green * matrice[1, 1] + image[j, i + 1].Green * matrice[1, 2] + image[j + 1, i - 1].Green * matrice[2, 0] + image[j + 1, i].Green * matrice[2, 1] + image[j + 1, i + 1].Green * matrice[2, 2]) / 9);
                b = (byte)((image[j - 1, i - 1].Blue * matrice[0, 0] + image[j - 1, i].Blue * matrice[0, 1] + image[j - 1, i + 1].Blue * matrice[0, 2] + image[j, i - 1].Blue * matrice[1, 0] + image[j, i].Blue * matrice[1, 1] + image[j, i + 1].Blue * matrice[1, 2] + image[j + 1, i - 1].Blue * matrice[2, 0] + image[j + 1, i].Blue * matrice[2, 1] + image[j + 1, i + 1].Blue * matrice[2, 2]) / 9);
            }
            Pixel res = new Pixel(r, g, b);
            return res;
        }
    }
}
