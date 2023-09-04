using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    Requerimiento 1: Mensajes del printf deben salir sin comillas
                     Incluir \n y \t como secuencias de escape
    Requerimiento 2: Agregar el % al PorFactor
                     Modificar el valor de una variable con ++,--,+=,-=,*=,/=.%=
    Requerimiento 3: Cada vez que se haga un match(Tipos.Identificador) verficar el
                     uso de la variable
                     Icremento(), Printf(), Factor()
                     Levantar una excepcion en scanf() cuando se capture un string
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            Imprime();
        }

        private void Imprime()
        {
            log.WriteLine("-----------------");
            log.WriteLine("V a r i a b l e s");
            log.WriteLine("-----------------");
            foreach (Variable v in lista)
            {
                log.WriteLine(v.getNombre() + " " + v.getTiposDatos() + " = " + v.getValor());
            }
            log.WriteLine("-----------------");
        }

        private bool Existe(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return true;
                }
            }
            return false;
        }
        private void Modifica(string nombre, float nuevoValor)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float GetValor(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getValor();
                }
            }
            throw new Error("La variable <" + nombre + "> no está declarada", log, linea, columna);
        }
        // Libreria -> #include<Identificador(.h)?>
        private void Libreria()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                match("h");
            }
            match(">");
        }
        //Librerias -> Libreria Librerias?
        private void Librerias()
        {
            Libreria();
            if (getContenido() == "#")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato ListaIdentificadores; Variables?
        private void Variables()
        {
            Variable.TiposDatos tipo = Variable.TiposDatos.Char;
            switch (getContenido())
            {
                case "int": tipo = Variable.TiposDatos.Int; break;
                case "float": tipo = Variable.TiposDatos.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TiposDatos tipo)
        {
            if (!Existe(getContenido()))
            {
                lista.Add(new Variable(getContenido(), tipo));
            }
            else
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> está duplicada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(tipo);
            }
        }
        //BloqueInstrucciones -> { ListaInstrucciones ? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "printf")
            {
                Printf();
            }
            else if (getContenido() == "scanf")
            {
                Scanf();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            // ...
            else
            {
                Asignacion();
            }
        }
        //Asignacion -> identificador = Expresion;
        private void Asignacion()
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            log.Write(getContenido() + " = ");
            string variable = getContenido();
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                match("=");
                Expresion();
            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                }
                else
                {
                    match("--");
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {

                if (getContenido() == "+=")
                {
                    match("+=");
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                }
                Expresion();
            }
            float resultado = stack.Pop();
            log.WriteLine(" = " + resultado);
            Modifica(variable,resultado);
            match(";");
        }
        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OperadorRelacional);
            Expresion();
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }

        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf()
        {
            match("printf");
            match("(");

            //guardamos el contenido de la cadena
            string cadena = getContenido();
            
            //Vamos a quitar las comillas cualquier cadena utilizando el metodo Trim
            cadena = cadena.Trim('"');
            //Podemos observar que este metodo Trim, es eficiente para quitar las comillas
            //Ya que consta de un arreglo de caracteres, y si encuentra una comilla, la quita

            //Ahora vamos a ver si la cadena tiene secuencias de escape ya sea \n o \t
            //Replace reemplaza el caracter que le indiquemos y Contains verifica si existe
            if (cadena.Contains("\\n") || cadena.Contains("\\t"))
            {
                cadena = cadena.Replace("\\t", "\t");
                cadena = cadena.Replace("\\n", "\n");
            }       
            //Ahora, como el getContenido lo guardamos con un string en "cadena"...
            //Lo vamos a mostrar en consola con un Console.Write pero con (cadena) porque ahi se guardo el contenido
            Console.Write(cadena); 
            match(Tipos.Cadena); //match de la cadena

            if (getContenido() == ",")
            {
                match(",");
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            match(")"); //match del parentesis
            match(";"); //match del punto y coma
        }

        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf()
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            string variable = getContenido();
            match(Tipos.Identificador);
            float captura = float.Parse(Console.ReadLine());
            Modifica(variable,captura);
            match(")");
            match(";");
        }
        //Main -> void main() BloqueInstrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones();
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                
                log.Write(" " + operador); //Agrega un espacio en blanco antes de escribir el operador
                float R2 = stack.Pop(); //Saca el segundo operando
                float R1 = stack.Pop(); //Saca el primer operando
                if (operador == "+") //Realiza la operación
                    stack.Push(R1+R2); //Guarda el resultado en la pila cuando es una suma
                else
                    stack.Push(R1-R2); //Guarda el resultado en la pila en caso de ser una resta
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (operador == "*")
                    stack.Push(R1*R2);
                else
                    stack.Push(R1/R2);
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }

                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}