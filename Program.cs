using System;

namespace Generador
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lenguaje L = new Lenguaje())
                {
                    L.generaLenguaje();
                    /*
                    while (!L.FinArchivo())
                    {
                        L.nextToken();
                    }
                    */
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }
        }
    }
}

