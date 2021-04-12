using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class Betukeszlet
    {
        //ADATTAGOK (statikus)
        static int _index;
        static int[] _szamoltbetuk;
        static string[] _betuk;
        static int[] _scores;

        //METÓDUSOK
        public static string RandomBetu()
        {

            int index = 0;

            string[] randombetuk = new string[_betuk.Length];

            for (int i = 0; i < _szamoltbetuk.Length; i++)
            {
                if (_szamoltbetuk[i] > 0)
                {
                    randombetuk[index] = _betuk[i];
                    index++;
                }
            }

            string randombetu = "";

            if (index > 0)
            {

                int randomindex = GameMaster.r.Next(index);

                randombetu = randombetuk[randomindex];

                int betuindex = IndexVisszaado(randombetu);

                _szamoltbetuk[betuindex] = _szamoltbetuk[betuindex] - 1;

            }

            return randombetu;

        }
        private static int IndexVisszaado(string betu)
        {

            bool van = false;

            int pointer = 0;

            while (pointer < _betuk.Length && !van)
            {

                if (_betuk[pointer] == betu)
                {
                    van = true;
                }
                else
                {
                    pointer++;
                }

            }

            int index = -1;

            if (van)
            {
                index = pointer;
            }

            return index;

        }
        public static void BetuHozzaado(string betu, int szam, int betuertek)
        {
            _szamoltbetuk[_index] = szam;
            _betuk[_index] = betu;
            _scores[_index] = betuertek;
            _index++;
        }
        public static bool BetuE(string szo)
        {

            bool van = false;

            int szamlalo = 0;

            while (szamlalo < _betuk.Length && !van)
            {
                if (_betuk[szamlalo] == szo)
                {
                    van = true;
                }
                else
                {
                    szamlalo++;
                }
            }

            return van;

        }
        public static int BetukPontjai(string s)
        {

            bool van = false;

            int szamlalo = 0;

            while (szamlalo < _betuk.Length && !van)
            {
                if (_betuk[szamlalo] == s)
                {
                    van = true;
                }
                else
                {
                    szamlalo++;
                }
            }

            if (van)
            {
                return _scores[szamlalo];
            }
            else
            {
                return 0;
            }

        }
        //--------------------------------
        //TALON INICIALIZALASA
        public static void BetukeszletBetolto(int meret)
        {
            _index = 0;
            _szamoltbetuk = new int[meret];
            _betuk = new string[meret];
            _scores = new int[meret];
        }



    }
}
