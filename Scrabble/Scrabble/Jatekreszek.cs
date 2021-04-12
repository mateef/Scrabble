using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Scrabble
{
    class Jatekreszek
    {

        //ADATTAGOK

        //AI
        int _aiBetuIndex = 0;
        string[] _aiBetuk = new string[_zsetonokSzama];
        int _aiScores;
        int _gepPasszokSzama;

        //PLAYER
        int _playerBetuIndex = 0;
        string[] _playerBetuk = new string[_zsetonokSzama];
        int _playerScores;
        int _elojatekosPasszokSzama;
        string _elojatekosnev;

        //TÁBLA
        const int _magassagSzelesseg = 15;
        const int _zsetonokSzama = 7;
        string[,] _tabla;
        bool[,] _lehelyezettE;

        public string Elojatekosnev { get => _elojatekosnev; set => _elojatekosnev = value; }


        //KONSTRUKTOR
        public Jatekreszek()
        {
            _tabla = new string[_magassagSzelesseg, _magassagSzelesseg];
            _lehelyezettE = new bool[_magassagSzelesseg, _magassagSzelesseg];

            _elojatekosPasszokSzama = 0;
            _gepPasszokSzama = 0;

            _playerScores = 0;
            _aiScores = 0;

            _aiBetuIndex = 0;
            _aiBetuk = new string[_zsetonokSzama * 2];

            _playerBetuIndex = 0;
            _playerBetuk = new string[_zsetonokSzama * 2];
        }


        //METÓDUSOK-------------------------------------------

        public void NevAdas()
        {

            Console.Write("\n\t\t\t\t\tTYPE YOUR NAME: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            string nev = Console.ReadLine();
            _elojatekosnev = nev;
            Console.ResetColor();
            do
            {
                Console.Write("\n\t\t\t\t\tPRESS <ENTER> TO START GAME");
            }
            while (Console.ReadKey().Key != ConsoleKey.Enter);
            Console.Clear();

        }
        public void Jatekfuttatas()
        {
            for (int i = 0; i < _zsetonokSzama; i++)
            {
                BetutHozzaado(Betukeszlet.RandomBetu(), true);
                BetutHozzaado(Betukeszlet.RandomBetu(), false);
            }
            Console.WriteLine();
            AIKiiro();
            PlayerKiiro();
            Console.WriteLine();

            while (_elojatekosPasszokSzama < 2 && _gepPasszokSzama < 2)
            {
                BetutHozzaado(Betukeszlet.RandomBetu(), true);
                Szo[] lehetsegesSzavak = Szotar.Lehelyezo(_aiBetuk);
                bool elhelyezettE = false;
                if (UresE() && lehetsegesSzavak[0] != null)
                {
                    Szo szo = lehetsegesSzavak[0];
                    int length = szo.Betutvisszaado().Length;
                    int maxKezdopont = _magassagSzelesseg - 1 - length;
                    int Fixkezdes = GameMaster.r.Next(15);
                    int veletlenKezdes = GameMaster.r.Next(maxKezdopont + 1);
                    int veletlenIrany = GameMaster.r.Next(2);
                    bool irany = veletlenIrany == 1 ? true : false;
                    if (irany)
                    {
                        VizFug(szo, Fixkezdes, veletlenKezdes, irany, true);
                        elhelyezettE = true;
                    }
                    else
                    {
                        VizFug(szo, veletlenKezdes, Fixkezdes, irany, true);
                        elhelyezettE = true;
                    }
                }
                else if (lehetsegesSzavak[0] != null)
                {
                    int i = 0;
                    while (i < lehetsegesSzavak.Length && !elhelyezettE && lehetsegesSzavak[i] != null)
                    {
                        string[] betuk = lehetsegesSzavak[i].Betutvisszaado();

                        int j = 0;
                        while (j < betuk.Length && !elhelyezettE)
                        {
                            string betu = betuk[j];
                            Koordinatak[] koordinatak = KoordinataVisszaado(betu);
                            int k = 0;
                            while (k < koordinatak.Length && !elhelyezettE)
                            {
                                int x = koordinatak[k].X();
                                int y = koordinatak[k].Y();
                                int startx = x - j;
                                int starty = y - j;
                                if (SikeresE(lehetsegesSzavak[i], x, starty, true))
                                {
                                    VizFug(lehetsegesSzavak[i], x, starty, true, true);
                                    elhelyezettE = true;
                                }
                                else if (SikeresE(lehetsegesSzavak[i], startx, y, false))
                                {
                                    VizFug(lehetsegesSzavak[i], startx, y, false, true);
                                    elhelyezettE = true;
                                }
                                k++;
                            }
                            j++;
                        }
                        i++;
                    }
                }
                if (!elhelyezettE)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Az AI passzolt!");
                    _gepPasszokSzama++;
                    Console.ResetColor();
                }
                if (_gepPasszokSzama < 2)
                {
                    TablatMegjelenito();
                    PontokKiirasa();
                    BetutHozzaado(Betukeszlet.RandomBetu(), false);
                    AIKiiro();
                    PlayerKiiro();

                    string valasztas;
                    bool probalkozikE = true;
                    while (probalkozikE)
                    {
                        Console.WriteLine("Kérem adja meg az adatokat az alábbi sorrendben: (betűk ','-vel elválasztva);x;y;Vízszintes? (true/false)");
                        Console.WriteLine("Passzolni a 'passz' szó beírásával lehet!");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        valasztas = Console.ReadLine();
                        Console.ResetColor();
                        if (valasztas != "passz")
                        {
                            string[] splittedValasztas = valasztas.Split(';');
                            int startY = 0;
                            int startX = 0;
                            bool vizszintes = true;
                            bool helyes = true;
                            Szo szo = null;
                            try
                            {
                                startX = int.Parse(splittedValasztas[1]) - 1;
                                startY = int.Parse(splittedValasztas[2]) - 1;
                                vizszintes = bool.Parse(splittedValasztas[3]);

                                string[] letters = splittedValasztas[0].Split(',');

                                if (letters.Length < 2)
                                {
                                    helyes = false;
                                }
                                else
                                {
                                    szo = new Szo(letters);
                                }
                            }
                            catch (Exception)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Nem megfelelő formátum,kérem próbálja újra!");
                                Console.ResetColor();
                                helyes = false;
                            }
                            if (helyes)
                            {
                                if (!Szotar.BetuE(szo))
                                {
                                    Szotar.BetutHozzaad(szo);
                                }
                                if (szo.BetuCheck(_playerBetuk))
                                {
                                    if (UresE())
                                    {
                                        if ((vizszintes && startY + szo.MeretetVisszaado() >= _magassagSzelesseg) || startX + szo.MeretetVisszaado() > _magassagSzelesseg)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("HIBA! Nem helyes a lehelyezés, próbálja újra!");
                                            Console.ResetColor();
                                        }
                                        else
                                        {
                                            VizFug(szo, startX, startY, vizszintes, false);
                                            probalkozikE = false;
                                        }
                                    }
                                    else
                                    {
                                        if (SikeresE(szo, startX, startY, vizszintes))
                                        {
                                            VizFug(szo, startX, startY, vizszintes, false);
                                            probalkozikE = false;
                                        }
                                        else
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("HIBA! Nem helyes a lehelyezés, próbálja újra!");
                                            Console.ResetColor();
                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("HIBA! Nem lehelyezhető az adott betűkből!");
                                    Console.ResetColor();
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Passzoltál!");
                            probalkozikE = false;
                            _elojatekosPasszokSzama++;
                        }
                    }
                }
            }

            TablatMegjelenito();
            PontokKiirasa();
            AIKiiro();
            PlayerKiiro();
            EredmenyKiiro();
        }

        //BETUOPERATOROK
        private void BetutHozzaado(string betu, bool aiE)
        {
            if (aiE)
            {
                if (_aiBetuIndex == _aiBetuk.Length)
                {
                    Ketszerezo(aiE);
                }
                _aiBetuk[_aiBetuIndex] = betu;
                _aiBetuIndex++;
            }
            else
            {

                if (_playerBetuIndex == _playerBetuk.Length)
                {
                    Ketszerezo(aiE);
                }
                _playerBetuk[_playerBetuIndex] = betu;
                _playerBetuIndex++;
            }
        }
        private void BetutEltavolito(string betu, bool aiE)
        {
            bool vanE = false;
            int i = 0;
            if (aiE)
            {
                while (i < _aiBetuk.Length && !vanE)
                {
                    if (_aiBetuk[i] == betu)
                    {
                        vanE = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                if (vanE)
                {
                    if (i == _aiBetuIndex - 1)
                    {
                        _aiBetuk[i] = string.Empty;
                    }
                    else
                    {
                        _aiBetuk[i] = _aiBetuk[_aiBetuIndex - 1];
                    }
                    _aiBetuIndex--;
                }
            }
            else
            {
                while (i < _playerBetuk.Length && !vanE)
                {
                    if (_playerBetuk[i] == betu)
                    {
                        vanE = true;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (vanE)
                {
                    if (i == _playerBetuIndex - 1)
                    {
                        _playerBetuk[i] = string.Empty;
                    }
                    else
                    {
                        _playerBetuk[i] = _playerBetuk[_playerBetuIndex - 1];
                    }
                    _playerBetuIndex--;
                }
            }
        }
        private void Ketszerezo(bool aiE)
        {
            if (aiE)
            {
                string[] ujtomb = new string[_aiBetuk.Length * 2];

                for (int i = 0; i < _aiBetuIndex; i++)
                {
                    ujtomb[i] = _aiBetuk[i];
                }
                _aiBetuk = ujtomb;
            }
            else
            {
                string[] ujtomb = new string[_playerBetuk.Length * 2];
                for (int i = 0; i < _playerBetuIndex; i++)
                {
                    ujtomb[i] = _playerBetuk[i];
                }
                _playerBetuk = ujtomb;
            }
        }

        //MEGJELENITO ES KIIRO

        private void PlayerKiiro()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(Elojatekosnev + " betűi: ");


            string kiiras = string.Empty;

            for (int i = 0; i < _playerBetuIndex; i++)
            {
                kiiras = kiiras + _playerBetuk[i] + ",";
            }

            kiiras = kiiras.Remove(kiiras.Length - 1);

            Console.WriteLine(kiiras);

            Console.ResetColor();

            Console.WriteLine();
        }
        private void AIKiiro()
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;

            Console.Write("AI betűi: ");


            string kiiras = string.Empty;

            for (int i = 0; i < _aiBetuIndex; i++)
            {
                kiiras = kiiras + _aiBetuk[i] + ",";
            }

            kiiras = kiiras.Remove(kiiras.Length - 1);

            Console.WriteLine(kiiras);

            Console.ResetColor();

            Console.WriteLine();
        }
        private void TablatMegjelenito()
        {
            Console.WriteLine();
            for (int i = 0; i < _tabla.GetLength(0); i++)
            {
                Console.Write("°");
                for (int j = 0; j < _tabla.GetLength(1); j++)
                {
                    if (_tabla[i, j] == null)
                    {
                        int realkoordinataI = i + 1;
                        int realkoordinataJ = j + 1;
                        string kiiras = realkoordinataI + ":" + realkoordinataJ;
                        if (kiiras.Length < 5)
                        {
                            int szukseges = 5 - kiiras.Length;
                            for (int k = 0; k < szukseges; k++)
                            {

                                kiiras = kiiras + " ";
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(kiiras);
                        Console.ResetColor();
                    }
                    else
                    {
                        if (_lehelyezettE[i, j])
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            if (_tabla[i, j].Length > 1)
                            {
                                Console.Write(_tabla[i, j] + "   ");
                            }
                            else
                            {
                                Console.Write(_tabla[i, j] + "    ");
                            }
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            if (_tabla[i, j].Length > 1)
                            {
                                Console.Write(_tabla[i, j] + "   ");
                            }
                            else
                            {
                                Console.Write(_tabla[i, j] + "    ");
                            }
                            Console.ResetColor();
                        }
                    }
                    Console.Write("°");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        private void PontokKiirasa()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("AI pontok száma: " + _aiScores);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(_elojatekosnev + " pontjai: " + _playerScores);
            Console.ResetColor();
            Console.WriteLine();
        }
        private void EredmenyKiiro()
        {

            Console.WriteLine();

            if (_aiScores > _playerScores)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Vesztettél!");
                Console.ResetColor();
            }
            else if (_playerScores > _aiScores)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(_elojatekosnev + " Nyert!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Döntetlen!");
                Console.ResetColor();
            }
        }
        private Koordinatak[] KoordinataVisszaado(string k)
        {
            int pointer = 0;
            Koordinatak[] koordinatak = new Koordinatak[_magassagSzelesseg * _magassagSzelesseg];

            for (int i = 0; i < _magassagSzelesseg; i++)
            {
                for (int j = 0; j < _magassagSzelesseg; j++)
                {
                    if (_tabla[i, j] == k)
                    {
                        koordinatak[pointer] = new Koordinatak(i, j);
                        pointer++;
                    }
                }

            }
            Koordinatak[] ujtomb = new Koordinatak[pointer];
            for (int i = 0; i < pointer; i++)
            {
                ujtomb[i] = koordinatak[i];
            }
            return ujtomb;
        }


        //LOGIKAI VIZSGALAT
        private bool UresE()
        {

            bool ures = true;

            int i = 0;

            while (i < _magassagSzelesseg && ures)
            {

                int j = 0;

                while (j < _magassagSzelesseg && ures)
                {

                    if (_tabla[i, j] != null)
                    {
                        ures = false;
                    }
                    else
                    {
                        j++;
                    }

                }

                i++;
            }

            return ures;
        }
        private bool VizFug(Szo szo, int startX, int startY, bool vizszintes, bool aiE)
        {

            string[] betuk = szo.Betutvisszaado();

            int points = 0;

            if (vizszintes)
            {

                for (int i = 0; i < betuk.Length; i++)
                {

                    _tabla[startX, startY + i] = betuk[i];

                    _lehelyezettE[startX, startY + i] = !aiE;

                    BetutEltavolito(betuk[i], aiE);
                    points = points + Betukeszlet.BetukPontjai(betuk[i]);
                }
            }
            else
            {
                for (int i = 0; i < betuk.Length; i++)
                {

                    _tabla[startX + i, startY] = betuk[i];

                    _lehelyezettE[startX + i, startY] = !aiE;


                    BetutEltavolito(betuk[i], aiE);


                    points = points + Betukeszlet.BetukPontjai(betuk[i]);
                }
            }

            if (aiE)
            {
                _aiScores = _aiScores + points;
            }
            else
            {
                _playerScores = _playerScores + points;
            }

            return true;

        }
        private bool SikeresE(Szo szo, int startX, int startY, bool vizszintes)
        {

            bool sikeres = true;

            if (startX < 0 || startY < 0)
            {
                sikeres = false;
            }

            if (vizszintes && sikeres)
            {
                if (startY + szo.MeretetVisszaado() - 1 < _magassagSzelesseg)
                {
                    int pointer = 0;
                    int szamlalo = 0;

                    while (pointer < szo.MeretetVisszaado() && sikeres && szamlalo < szo.MeretetVisszaado())
                    {

                        if (_tabla[startX, startY + pointer] == null || _tabla[startX, startY + pointer] == string.Empty)
                        {
                            pointer++;
                        }
                        else if (_tabla[startX, startY + pointer] == szo.Indexvisszaado(pointer))
                        {
                            szamlalo++;
                            pointer++;
                        }
                        else
                        {
                            Console.WriteLine("Nem egyezik a betű");
                            sikeres = false;
                        }
                    }
                    if (szamlalo >= szo.MeretetVisszaado())
                    {
                        sikeres = false;
                    }
                    else if (szamlalo == 0)
                    {
                        sikeres = false;
                    }
                }
                else
                {
                    sikeres = false;
                }
            }
            else
            {
                if (startX + szo.MeretetVisszaado() - 1 < _magassagSzelesseg)
                {
                    int pointer = 0;
                    int szamlalo = 0;

                    while (pointer < szo.MeretetVisszaado() && sikeres && szamlalo < szo.MeretetVisszaado())
                    {
                        if (_tabla[startX + pointer, startY] == null || _tabla[startX + pointer, startY] == string.Empty)
                        {
                            pointer++;
                        }
                        else if (_tabla[startX + pointer, startY] == szo.Indexvisszaado(pointer))
                        {
                            szamlalo++;
                            pointer++;
                        }
                        else
                        {
                            Console.WriteLine("Nem egyező szó");
                            sikeres = false;
                        }
                    }

                    if (szamlalo >= szo.MeretetVisszaado())
                    {
                        sikeres = false;
                    }
                }
                else
                {
                    sikeres = false;
                }
            }

            if (sikeres)
            {
                string[,] tabla = new string[_magassagSzelesseg, _magassagSzelesseg];
                for (int i = 0; i < _magassagSzelesseg; i++)
                {
                    for (int j = 0; j < _magassagSzelesseg; j++)
                    {
                        tabla[i, j] = _tabla[i, j];
                    }
                }

                if (vizszintes)
                {
                    string[] betuk = szo.Betutvisszaado();
                    for (int i = 0; i < betuk.Length; i++)
                    {
                        tabla[startX, startY + i] = betuk[i];
                    }
                }
                else
                {
                    string[] betuk = szo.Betutvisszaado();
                    for (int i = 0; i < betuk.Length; i++)
                    {
                        tabla[startX + i, startY] = betuk[i];
                    }
                }
                sikeres = SikeresKiiro(tabla);
            }
            return sikeres;
        }
        private bool SikeresKiiro(string[,] tabla)
        {
            bool sikeres = true;
            int i = 0;
            while (i < _magassagSzelesseg && sikeres)
            {
                int start = -1;
                int vege = -1;
                int j = 0;

                while (j < _magassagSzelesseg && sikeres)
                {
                    if (tabla[i, j] != null)
                    {
                        if (start == -1)
                        {
                            start = j;
                        }
                    }
                    else
                    {
                        if (start != -1)
                        {
                            vege = j - 1;
                        }
                    }
                    if (start != -1 && vege != -1)
                    {
                        if (start != vege)
                        {
                            int pointer = 0;
                            string[] betuk = new string[vege - start + 1];
                            for (int k = start; k <= vege; k++)
                            {
                                betuk[pointer] = tabla[i, k];
                                pointer++;
                            }
                            Szo betu = new Szo(betuk);
                            if (!Szotar.BetuE(betu))
                            {
                                sikeres = false;
                            }
                        }
                        start = -1;
                        vege = -1;
                    }
                    j++;
                }
                i++;
            }
            int l = 0;
            while (l < _magassagSzelesseg && sikeres)
            {
                int start = -1;
                int vege = -1;
                int m = 0;
                while (m < _magassagSzelesseg && sikeres)
                {
                    if (tabla[m, l] != null)
                    {
                        if (start == -1)
                        {
                            start = m;
                        }
                    }
                    else
                    {
                        if (start != -1)
                        {
                            vege = m - 1;
                        }
                    }
                    if (start != -1 && vege != -1)
                    {
                        if (start != vege)
                        {
                            int szamlalo = 0;

                            string[] betuk = new string[vege - start + 1];

                            for (int k = start; k <= vege; k++)
                            {
                                betuk[szamlalo] = tabla[k, l];
                                szamlalo++;
                            }
                            Szo w = new Szo(betuk);
                            if (!Szotar.BetuE(w))
                            {
                                sikeres = false;
                            }
                        }
                        start = -1;
                        vege = -1;
                    }
                    m++;
                }
                l++;
            }
            return sikeres;
        }
    }
}
