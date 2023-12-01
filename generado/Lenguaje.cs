using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Generado
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {
        }
        public Lenguaje(string nombre) : base(nombre)
        {
        }
        public void Programa()
        {
            Librerias();
            Main();
        }
        public void Librerias()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            match(".");
            match("h");
            match(">");

            // Aqui comienza el epsilon
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
                match("include");
                Main();
            }
            // Aqui comienza el or
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
            }
            else if (getContenido() == "include")
            {
                match("include");
            }
            else if (getContenido() == "math")
            {
                match("math");
            }
            else
            {
                Main();
            }
            // Aqui comienza el or
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
            }
            else
            {
                match("include");
            }
            match("#");
        }
        public void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            match("{");
            match("}");
        }
    }
}
