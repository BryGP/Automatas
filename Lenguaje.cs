using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Generador
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje() { }

        public Lenguaje(string nombre)
            : base(nombre) { }

        public void generaLenguaje()
        {
            generado.WriteLine("using System;");
            generado.WriteLine("using System.Collections.Generic;");
            generado.WriteLine("using System.Linq;");
            generado.WriteLine("using System.Reflection.PortableExecutable;");
            generado.WriteLine("using System.Threading.Tasks;");
            generado.WriteLine();
            generado.WriteLine("namespace Generado");
            generado.WriteLine("{");
            generado.WriteLine("    public class Lenguaje : Sintaxis");
            generado.WriteLine("    {");
            generado.WriteLine("        public Lenguaje()");
            generado.WriteLine("        {");
            generado.WriteLine("        }");
            generado.WriteLine("        public Lenguaje(string nombre) : base(nombre)");
            generado.WriteLine("        {");
            generado.WriteLine("        }");

            Producciones();

            generado.WriteLine("    }");
            generado.WriteLine("}");
        }
        // Produccion -> SNT Flechita ListaSimbolos FinProduccion
        private void Producciones()
        {
            generado.WriteLine("        public void " + getContenido() + ("()"));
            generado.WriteLine("        {");
            match(Tipos.SNT);
            match(Tipos.Flechita);
            listaSimbolos();
            match(Tipos.FinProduccion);
            generado.WriteLine("        }");

            if (getClasificacion() == Tipos.SNT)
            {
                Producciones();
            }
        }
        private void listaSimbolos()
        {
            if (esPalabraReservada(getContenido()))
            {
                generado.WriteLine("            match(Tipos." + getContenido() + ");");
                match(Tipos.SNT);
            }
            else if (getClasificacion() == Tipos.ST)
            {
                generado.WriteLine("            match(\"" + getContenido() + "\");");
                match(Tipos.ST);
            }
            else if (getClasificacion() == Tipos.SNT)
            {
                generado.WriteLine("            " + getContenido() + "();");
                match(Tipos.SNT);
            }
            else if (getClasificacion() == Tipos.Epsilon)
            {
                match(Tipos.Epsilon);
                match(Tipos.PIzq);
                LlamarEpsilon();
                ListaEpsilon();
                match(Tipos.PDer);
            }
            else if (getClasificacion() == Tipos.Or)
            {
                match(Tipos.Or);
                match(Tipos.PIzq);
                LlamarOrs();
                ListaOrs();
                match(Tipos.PDer);
            }
            if (getClasificacion() != Tipos.FinProduccion)
            {
                listaSimbolos();
            }
        }
        // Epsilon -> Epsilon PIzq SimbolosEpsilon PDer
        // Funcion para llamar a un epsilon desde listaSimbolos
        private void LlamarEpsilon()
        {
            string simbolo = getContenido();
            generado.WriteLine();
            generado.WriteLine("            // Aqui comienza el epsilon");
            if (esPalabraReservada(simbolo))
            {
                match(Tipos.SNT);
                generado.WriteLine("            if (getClasificacion() == Tipos." + simbolo + ")");
                generado.WriteLine("            {");
                generado.WriteLine("                match(Tipos." + simbolo + ");");
            }
            else if (getClasificacion() == Tipos.ST)
            {
                generado.WriteLine("            if (getContenido() == \"" + simbolo + "\")");
                match(Tipos.ST);
                generado.WriteLine("            {");
                generado.WriteLine("                match(\"" + simbolo + "\");");
            }
            else
            {
                throw new Error("de sintaxis, se espera: " + simbolo + "en la linea",log, linea, columna);
            }
            ListaEpsilon();
            generado.WriteLine("            }");
        }
        // SimbolosEpsilon -> ListaSimbolos | Epsilon
        // Generar los match de un epsilon
        private void ListaEpsilon()
        {
            if (esPalabraReservada(getContenido()))
            {
                generado.WriteLine("                match(Tipos." + getContenido() + ");");
                match(Tipos.SNT);
            }
            else if (getClasificacion() == Tipos.ST)
            {
                generado.WriteLine("                match(\"" + getContenido() + "\");");
                match(Tipos.ST);
            }
            else if (getClasificacion() == Tipos.SNT)
            {
                generado.WriteLine("                " + getContenido() + "();");
                match(Tipos.SNT);
            }
            if (getClasificacion() != Tipos.PDer)
            {
                ListaEpsilon();
            }
        }
        // Lista de ors -> |\( OrsList \)
        // Funcion para llamar a un or desde listaSimbolos
        private void LlamarOrs()
        {
            string simbolo = getContenido();
            generado.WriteLine("            // Aqui comienza el or");
            if (esPalabraReservada(simbolo))
            {
                match(Tipos.SNT);
                generado.WriteLine("            if (getClasificacion() == Tipos." + simbolo + ")");
                generado.WriteLine("            {");
                generado.WriteLine("                match(Tipos." + simbolo + ");");
                generado.WriteLine("            }");
            }
            else if (getClasificacion() == Tipos.ST)
            {
                generado.WriteLine("            if (getContenido() == \"" + simbolo + "\")");
                match(Tipos.ST);
                generado.WriteLine("            {");
                generado.WriteLine("                match(\"" + simbolo + "\");");
                generado.WriteLine("            }");
            }
            else
            {
                throw new Error("de sintaxis, se espera: " + simbolo + "en la linea",log, linea, columna);
            }
            if (getClasificacion() != Tipos.FinProduccion)
            {
                ListaOrs();
            }
        }
        // OrsList -> ListaSimbolos | OrsList
        // Generar los match de un or
        private void ListaOrs()
        {
            string simbolo = getContenido();
            if (esPalabraReservada(simbolo))
            {
                match(Tipos.SNT);
                if (getClasificacion() == Tipos.PDer)
                {
                    generado.WriteLine("        else");
                }
                else
                {
                    generado.WriteLine("            else if (getClasificacion() == Tipos." + simbolo + ")");
                }
                generado.WriteLine("            {");
                generado.WriteLine("                match(Tipos." + simbolo + ");");
                generado.WriteLine("            }");
            }
            else if (getClasificacion() == Tipos.ST)
            {
                match(Tipos.ST);
                if (getClasificacion() != Tipos.PDer)
                {
                    generado.WriteLine("            else if (getContenido() == \"" + simbolo + "\")");
                }
                else
                {
                    generado.WriteLine("            else");
                }
                generado.WriteLine("            {");
                generado.WriteLine("                match(\"" + simbolo + "\");");
                generado.WriteLine("            }");
            }
            else if (getClasificacion() == Tipos.SNT)
            {
                generado.WriteLine("            else");
                match(Tipos.SNT);
                generado.WriteLine("            {");
                generado.WriteLine("                " + simbolo + "();");
                generado.WriteLine("            }");

                if (getClasificacion() != Tipos.PDer)
                {
                    throw new Error("de sintaxis, <" + simbolo + "> se espera un parentesis \\)", log, linea, columna);
                    //ListaOrs();
                }
            }
            if (getClasificacion() != Tipos.PDer)
            {
                ListaOrs();
            }
        }
        private bool esPalabraReservada(string palabra)
        {
            switch (palabra)
            {
                case "Identificador":
                case "Numero":
                case "Asignacion":
                case "Inicializacion":
                case "OperadorRelacional":
                case "OperadorTermino":
                case "OperadorFactor":
                case "IncrementoTermino":
                case "IncrementoFactor":
                case "Cadena":
                case "Ternario":
                case "FinSentencia":
                case "OperadorLogico":
                case "Inicio":
                case "Fin":
                case "Caracter":
                case "TipoDato":
                case "Zona":
                case "Condicion":
                case "Ciclo":
                    return true;
            }
            return false;
        }
    }
}