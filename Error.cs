
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_2
{
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log, int linea, int columna) : base(mensaje + " en la linea "+linea+" columna "+columna)
        {
            log.WriteLine("Error: "+mensaje+" en la linea "+linea+" columna "+columna);
        }
    }
}