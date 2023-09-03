using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Sintaxis_2
{
    public class Lexico : Token, IDisposable
    {
        const int F = -1;
        const int E = -2;

        int[,] TRAND =
        { 
          // 0   1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25
          // WS  L  D  .  =  :  ;  &  |  >  <  !  +  -  *  /  %  "  ' EOF ?  # lmd {  } EOL
            { 0, 1, 2,26, 5, 7, 9,10,11,13,14,15,17,18,20,33,20,22,24, F,25,27,26,31,32, 0}, //  0
            { F, 1, 1, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F,31, F, F}, //  1
            { F, F, 2, 3, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  2
            { E, E, 4, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F}, //  3
            { F, F, 4, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  4
            { F, F, F, F, 6, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  5
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  6
            { F, F, F, F, 8, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  7
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  8
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, //  9
            { F, F, F, F, F, F, F,12, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 10
            { F, F, F, F, F, F, F, F,12, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 11
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 12
            { F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 13
            { F, F, F, F,16, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 14
            { F, F, F, F, 6, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 15
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 16
            { F, F, F, F,19, F, F, F, F, F, F, F,19, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 17
            { F, F, F, F,19, F, F, F, F, F, F, F, F,19, F, F, F, F, F, F, F, F, F, F, F, F}, // 18
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 19
            { F, F, F, F,21, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 20
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 21
            {22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,23,22, E,22,22,22,22,22, F}, // 22
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 23
            {24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,24,23, E,24,24,24,24,24, F}, // 24
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 25
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 26
            { F, F,28, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 27
            { F, F,29, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 28
            { F, F,30, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 29
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 30
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 31
            { F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}, // 32
            { F, F, F, F,21, F, F, F, F, F, F, F, F, F,35,34, F, F, F, F, F, F, F, F, F, F}, // 33
            {34,34,34,34,34,34,34,34,34,34,34,34,34,34,34,34,34,34,34, F,34,34,34,34,34, 0}, // 34
            {35,35,35,35,35,35,35,35,35,35,35,35,35,35,36,35,35,35,35, E,35,35,35,35,35,35}, // 35
            {35,35,35,35,35,35,35,35,35,35,35,35,35,35,36, 0,35,35,35, E,35,35,35,35,35,35}, // 36
          // WS  L  D  .  =  :  ;  &  |  >  <  !  +  -  *  /  %  "  ' EOF ?  # lmd {  }  \n
          // 0   1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25
        };
        private StreamReader archivo;
        protected StreamWriter log;

        protected int linea;
        protected int columna;
        public Lexico()
        {
            linea = columna = 1;
            log = new StreamWriter("prueba.log");
            log.WriteLine("Autor: Guillermo Fernandez Romero");
            log.WriteLine("Fecha: 3-Mayo-2023 15:09");
            log.AutoFlush = true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.txt no existe", log, linea, columna);
            }
        }
        public Lexico(string nombre)
        {
            linea = columna = 1;
            log = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".log");
            log.WriteLine("Autor: Guillermo Fernandez Romero");
            log.WriteLine("Fecha: 3-Mayo-2023 15:09");
            log.AutoFlush = true;
            if (Path.GetExtension(nombre) != ".cpp")
            {
                throw new Error("El archivo " + nombre + " no tiene extension CPP", log, linea, columna);
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
        }
        private int Columna(char t)
        {
            // WS  L  D  .  =  :  ;  &  |  >  <  !  +  -  *  /  %  "  ' EOF ?  # lmd
          // WS  L  D  .  =  :  ;  &  |  >  <  !  +  -  *  /  %  "  ' EOF ?  # lmd {  }  \n
          // 0   1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25
            if (FinArchivo())
                return 19;
            else if (t == '\n')
                return 25;
            else if (char.IsWhiteSpace(t))
                return 0;
            else if (char.IsLetter(t))
                return 1;
            else if (char.IsDigit(t))
                return 2;
            else if (t == '.')
                return 3;
            else if (t == '=')
                return 4;
            else if (t == ':')
                return 5;
            else if (t == ';')
                return 6;
            else if (t == '&')
                return 7;
            else if (t == '|')
                return 8;
            else if (t == '>')
                return 9;
            else if (t == '<')
                return 10;
            else if (t == '!')
                return 11;
            else if (t == '+')
                return 12;
            else if (t == '-')
                return 13;
            else if (t == '*')
                return 14;
            else if (t == '/')
                return 15;
            else if (t == '%')
                return 16;
            else if (t == '"')
                return 17;
            else if (t == '\'')
                return 18;
            else if (t == '?')
                return 20;
            else if (t == '#')
                return 21;
            else if (t == '{')
                return 23;
            else if (t == '}')
                return 24;
            return 22;
        }
        private void Clasifica(int Estado)
        {
            switch (Estado)
            {
                case 1:
                    setClasificacion(Tipos.Identificador);
                    break;
                case 2:
                    setClasificacion(Tipos.Numero);
                    break;
                case 5:
                    setClasificacion(Tipos.Asignacion);
                    break;
                case 6:
                case 13:
                case 14:
                    setClasificacion(Tipos.OperadorRelacional);
                    break;
                case 7:
                case 10:
                case 11:
                case 26:
                case 27:
                    setClasificacion(Tipos.Caracter);
                    break;
                case 8:
                    setClasificacion(Tipos.Inicializacion);
                    break;
                case 9:
                    setClasificacion(Tipos.FinSentencia);
                    break;
                case 12:
                case 15:
                    setClasificacion(Tipos.OperadorLogico);
                    break;
                case 17:
                case 18:
                    setClasificacion(Tipos.OperadorTermino);
                    break;
                case 19:
                    setClasificacion(Tipos.IncrementoTermino);
                    break;
                case 20:
                case 33:
                    setClasificacion(Tipos.OperadorFactor);
                    break;
                case 21:
                    setClasificacion(Tipos.IncrementoFactor);
                    break;
                case 22:
                case 24:
                    setClasificacion(Tipos.Cadena);
                    break;
                case 25:
                    setClasificacion(Tipos.Ternario);
                    break;
                case 31:
                    setClasificacion(Tipos.Inicio);
                    break;
                case 32:
                    setClasificacion(Tipos.Fin);
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
            if (getClasificacion() == Tipos.Identificador)
            {
                switch (getContenido())
                {
                    case "char":
                    case "int":
                    case "float": setClasificacion(Tipos.TipoDato); break;

                    case "private":
                    case "public":
                    case "protected": setClasificacion(Tipos.Zona); break;

                    
                    case "else": 
                    case "switch": 
                    case "if": setClasificacion(Tipos.Condicion); break;

                    case "do":
                    case "while":
                    case "for": setClasificacion(Tipos.Ciclo); break;

                }
            }
            if (!FinArchivo())
            {
                // log.WriteLine(getContenido() + " | " + getClasificacion());
            }
            if (Estado == E)
            {
                if (getClasificacion() == Tipos.Numero)
                {
                    throw new Error("lexico, se espera un digito", log, linea, columna);
                }
                else
                {
                    throw new Error("lexico, se espera cerrar cadena", log, linea, columna);
                }
            }
        }
        public bool FinArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}