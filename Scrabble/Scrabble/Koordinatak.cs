using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class Koordinatak
    {
        //ADATTAGOK
        int _y;
        int _x;

        //Tulajdonságok
        public int X()
        {
            return _x;
        }
        public int Y()
        {
            return _y;
        }

        //KONSTRUKTOR
        public Koordinatak(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
