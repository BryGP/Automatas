using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    Requerimiento 4: Implementar la ejecucion del Else
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
            Main(true);
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
            return 0;
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
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "printf")
            {
                Printf(ejecuta);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            // ...
            else
            {
                Asignacion(ejecuta);
            }
        }
        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta)
        {
            float resultado = 0;
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }

            log.Write(getContenido() + " = ");
            string variable = getContenido(); //Guardamos el contenido de la variable en un string 
            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");
                Expresion();
                resultado = stack.Pop(); //Sacamos el valor del stack
            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    //Para modificar los valores, vamos a utilizar la sig linea
                    //La cual va constar de asignar el signo despues del contenido de la variable
                    resultado = GetValor(variable) + 1;
                }
                else
                {
                    match("--");
                    resultado = GetValor(variable) - 1;
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {

                if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion();
                    resultado = GetValor(variable) + stack.Pop(); //En este caso vamos a sacar el valor del stack y lo vamos a sumar con el valor de la variable

                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion();
                    resultado = GetValor(variable) - stack.Pop();
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion();
                    resultado = GetValor(variable) * stack.Pop();
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion();
                    resultado = GetValor(variable) / stack.Pop();
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion();
                    resultado = GetValor(variable) % stack.Pop();
                }
            }

            else
            {
                throw new Error("de sintaxis, se esperaba un operador de asignación", log, linea, columna);
            }
            log.WriteLine(" = " + resultado);

            //Agregamos el if para los incrementos
            if (ejecuta)
            {
                Modifica(variable, resultado);
            }
            match(";");
        }
        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }

        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion(ejecuta);
            Condicion();
            match(";");
            Incremento(ejecuta);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
        }
        //Incremento -> Identificador ++ | --
        private void Incremento(bool ejecuta)
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);
            //Verificamos el uso de la variable
            stack.Push(float.Parse(getContenido()));

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
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido(); //Guardamos el operador relacional en una variable
            match(Tipos.OperadorRelacional);
            Expresion();
            //Agregamos el operador relacional al stack
            float R1 = stack.Pop();
            float R2 = stack.Pop();
            //Añadimos el switch para los operadores relacionales
            switch (operador)
            {
                case "==": return R2 == R1;
                case ">": return R2 > R1;
                case ">=": return R2 >= R1;
                case "<": return R2 < R1;
                case "<=": return R2 <= R1;
                default: return R2 != R1;
            }
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If(bool ejecuta)
        {
            match("if");
            match("(");
            bool evaluacion = Condicion() && ejecuta; //Agregamos el && ejecuta para que no se ejecute el if si no se cumple la condicion
            Console.WriteLine(evaluacion); //Evaluacion se encarga de evaluar la condicion y saber si es verdadera o falsa
            match(")");

            //Vamos a crear un bool para el if
            bool valorIf = evaluacion;
            if (getContenido() == "{")
            {
                BloqueInstrucciones(valorIf);
            }
            else
            {
                Instruccion(valorIf);
            }

            //Agregamos el bool del else
            bool valorElse = !evaluacion;
            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    /*Para hacer el else, vamos a necesitar efectuar
                    la instruccion evaluar, pero al contrario, que seria "!" */
                    BloqueInstrucciones(valorElse);
                }
                else
                {
                    Instruccion(valorElse);
                }
            }
        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta)
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
            if (ejecuta)
            {
                Console.Write(cadena);
            }
            match(Tipos.Cadena); //match de la cadena

            if (getContenido() == ",")
            {
                match(",");
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                Console.Write(GetValor(getContenido())); //Agregamos el valor de la variable al stack
                match(Tipos.Identificador);
                //Verificamos el uso de la variable
                stack.Push(float.Parse(getContenido()));
            }
            match(")"); //match del parentesis
            match(";"); //match del punto y coma
        }

        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            string variable = getContenido();

            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);

            if (ejecuta)
            {
                string captura = Console.ReadLine();
                //Agregamos la excepcion del scanf cuando se capture un string                
                if (float.TryParse(captura, out float resultado))
                {
                    // Si se puede hacer la conversión, llamamos a Modifica
                    Modifica(variable, resultado);
                }
                else
                {
                    throw new Exception("Se capturó una cadena...");
                }
            }
            match(")");
            match(";");
        }
        //Main -> void main() BloqueInstrucciones
        private void Main(bool ejecuta)
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(ejecuta);
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
                    stack.Push(R1 + R2); //Guarda el resultado en la pila cuando es una suma
                else
                    stack.Push(R1 - R2); //Guarda el resultado en la pila en caso de ser una resta
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
                    stack.Push(R1 * R2);
                if (operador == "/")
                    stack.Push(R1 / R2);

                //Agregamos el % al PorFactor
                if (operador == "%")
                    stack.Push(R1 % R2);
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                stack.Push(float.Parse(getContenido())); //Verficamos el uso de la variable
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                stack.Push(GetValor(getContenido())); //Agregamos el valor de la variable al stack
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