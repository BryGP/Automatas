using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Generador
{
    public class Lexico : Token, IDisposable
    {
        const int F = -1;
        const int E = -2;

        int[,] TRAND =
        { 
            // WS,L,-,>,\,;,?,|,(,),Lam
            {0,1,2,10,4,10,10,10,10,10,10},
            {F,1,F,F,F,F,F,F,F,F,F},
            {F,F,F,3,F,F,F,F,F,F,F},
            {F,F,F,F,F,F,F,F,F,F,F},
            {F,F,F,F,F,5,6,7,8,9,F},
            {F,F,F,F,F,F,F,F,F,F,F},
            {F,F,F,F,F,F,F,F,F,F,F},
            {F,F,F,F,F,F,F,F,F,F,F},
            {F,F,F,F,F,F,F,F,F,F,F},
            {F,F,F,F,F,F,F,F,F,F,F},
            {F,F,F,F,F,F,F,F,F,F,F},
        };
        protected StreamReader archivo;
        protected StreamWriter log;
        protected StreamWriter generado;

        protected int linea;
        protected int columna;
        protected int caracter;
        public Lexico()
        {
            linea = columna = caracter = 1;
            log      = new StreamWriter("..//generado//Lenguaje.log");
            generado = new StreamWriter("..//generado//Lenguaje.cs");
            log.AutoFlush = true;
            generado.AutoFlush = true;
            if (File.Exists("prueba.txt"))
            {
                archivo = new StreamReader("prueba.txt");
            }
            else
            {
                throw new Error("El archivo prueba.gram no existe", log, linea, columna);
            }
        }
        public Lexico(string nombre)
        {
            linea = columna = caracter = 1;
            log      = new StreamWriter("..//generado//Lenguaje.log");
            generado = new StreamWriter("..//generado//Lenguaje.cs");
            log.AutoFlush = true;
            generado.AutoFlush = true;
            if (Path.GetExtension(nombre) != ".gram")
            {
                throw new Error("El archivo " + nombre + " no tiene extension GRAM", log, linea, columna);
            }
            if (File.Exists(nombre))
            {
                archivo = new StreamReader(nombre);
            }
            else
            {
                throw new Error("El archivo " + nombre + " no existe", log, linea, columna);
            }
        }
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            generado.Close();
        }
        private int Columna(char t)
        {
            if (char.IsWhiteSpace(t))  return 0;
            else if (char.IsLetter(t)) return 1;
            else if (t == '-')         return 2;
            else if (t == '>')         return 3;
            else if (t == '\\')        return 4;
            else if (t == ';')         return 5;
            else if (t == '?')         return 6;
            else if (t == '|')         return 7;
            else if (t == '(')         return 8;
            else if (t == ')')         return 9; 
            return 10;
        }
        private void Clasifica(int Estado)
        {
            switch (Estado)
            {
                case 1:
                case 2:
                case 4:
                case 10:
                    setClasificacion(Tipos.ST);
                    break;
                case 3:
                    setClasificacion(Tipos.Flechita);
                    break;
                case 5:
                    setClasificacion(Tipos.FinProduccion);
                    break;
                case 6:
                    setClasificacion(Tipos.Epsilon);
                    break;
                case 7:
                    setClasificacion(Tipos.Or);
                    break;
                case 8:
                    setClasificacion(Tipos.PIzq);
                    break;
                case 9:
                    setClasificacion(Tipos.PDer);
                    break;
            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";

            int Estado = 0;   // Estado de inicio

            while (Estado >= 0)
            {
                c = (char)archivo.Peek();
                Estado = TRAND[Estado, Columna(c)];
                Clasifica(Estado);
                if (Estado >= 0)
                {
                    archivo.Read();
                    caracter++;
                    columna++;
                    if (Estado > 0)
                    {
                        buffer += c;
                    }
                    else
                    {
                        buffer = "";
                    }
                    if (c == '\n')
                    {
                        linea++;
                        columna = 1;
                    }
                }
            }
            setContenido(buffer);
            if (getClasificacion() == Tipos.ST && char.IsUpper(getContenido()[0]))
            {
                setClasificacion(Tipos.SNT);
            }
            if (!FinArchivo())
            {
                log.WriteLine(getContenido() + " | " + getClasificacion());
            }
            if (Estado == E)
            {
                //
            }

        }
        public bool FinArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}