using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Scrabble
{
    class GameMaster
    {
        //RANDOM (statikus)
        public static Random r;
        public static string scrabbleLogo;

        //START
        public static void Kezdes()
        {
            r = new Random();
            BetuBetolto();
            SzotarBetolto();
            Jatekreszek jatek = new Jatekreszek();
            LogoLetrehozas();
            Logo();
            jatek.NevAdas();
            jatek.Jatekfuttatas();
            Szotelmento();
        }

        //LOAD_SAVE
        public static void BetuBetolto()
        {
            string[] sor = File.ReadAllLines("betukeszlet.txt");

            Betukeszlet.BetukeszletBetolto(sor.Length);

            for (int i = 0; i < sor.Length; i++)
            {

                string[] splitted = sor[i].Split('-');

                Betukeszlet.BetuHozzaado(splitted[0], int.Parse(splitted[1]), int.Parse(splitted[2]));
            }
        }
        public static void SzotarBetolto()
        {
            string[] sorok = File.ReadAllLines("szotar.txt");

            Szotar.SzotarBetolto(sorok.Length);

            for (int i = 0; i < sorok.Length; i++)
            {

                string[] betuk = sorok[i].Split(',');

                Szo betu = new Szo(betuk);

                Szotar.BetutHozzaad(betu);

            }
        }
        public static void Szotelmento()
        {

            Szo[] szavak = Szotar.BetuketMasolo();

            string[] irnivalo = new string[szavak.Length];

            for (int i = 0; i < szavak.Length; i++)
            {

                string[] betuk = szavak[i].Betutvisszaado();

                string sor = string.Empty;

                for (int j = 0; j < betuk.Length; j++)
                {
                    sor = sor + betuk[j] + "-";
                }

                sor = sor.Remove(sor.Length - 1);

                irnivalo[i] = sor;
            }

            File.WriteAllLines("szotar.txt", irnivalo);

        }
        public static void LogoLetrehozas()
        {
            StreamReader sr = new StreamReader("scrabblelogo.txt");
            while (!sr.EndOfStream)
                scrabbleLogo += sr.ReadLine() + "\n";

            sr.Close();
        }
        public static void Logo()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(scrabbleLogo);
            Console.ResetColor();
        }
    }
}