using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    UNIDAD 1, PRIMERA ENTREGA DE PROYECTO
    Requerimiento 3: Cada vez que se haga un match(Tipos.Identificador) verficar el
                     uso de la variable
                     Icremento(), Printf(), Factor() y usar getValor y Modifica
                     Levantar una excepcion en scanf() cuando se capture un string
    Requerimiento 4: Implemenar la ejecución del ELSE

    UNIDAD 2, SEGUNDA ENTREGA DE PROYECTO
    Requerimiento 1: Implementar la ejecucion del while
    Requerimiento 2: Implementar la ejecicion del do - while
    Requerimiento 3: Implementar la ejecucion del for
    Requerimiento 4: Marcar errores semánticos y levantar excepciones y decir que tipo de dato es mayor
    Requerimiento 5: Desarrollar la función Castea

*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;

        Variable.TiposDatos tipoDatoExpresion;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
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
                log.WriteLine(v.getNombre() + " " + v.getTiposDato() + " = " + v.getValor());
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
        private float getValor(string nombre)
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
        private Variable.TiposDatos getTipo(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getTiposDato();
                }
            }
            return Variable.TiposDatos.Char;
        }
        private Variable.TiposDatos getTipo(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TiposDatos.Float;
            }
            else if (resultado < 256)
            {
                return Variable.TiposDatos.Char;
            }
            else if (resultado < 65536)
            {
                return Variable.TiposDatos.Int;
            }
            return Variable.TiposDatos.Float;
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
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            else
            {
                Asignacion(ejecuta);
            }
        }
        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta)
        {
            // Declaramos los identificadores para el tipo de dato de la expresión.
            // Tanto como destino y origen.
            Variable.TiposDatos tipoOrigen = tipoDatoExpresion;
            Variable.TiposDatos tipoDestino = getTipo(getContenido());
            // Declaramos una variable de tipo float para el resultado de la expresión.
            float resultado = 0;
            tipoDatoExpresion = Variable.TiposDatos.Char;
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
                resultado = stack.Pop();

                if (tipoDestino >= tipoOrigen)
                {
                    tipoDatoExpresion = tipoDestino;
                }
                else
                {
                    throw new Error("de semantica, no se puede asignar un <" + tipoOrigen + "> a un <" + tipoDestino + ">", log, linea, columna);
                }
            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    resultado = getValor(variable) + 1;
                }
                else
                {
                    match("--");
                    resultado = getValor(variable) - 1;
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {
                resultado = getValor(variable);
                if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion();
                    resultado += stack.Pop();
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion();
                    resultado -= stack.Pop();
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion();
                    resultado *= stack.Pop();
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion();
                    resultado /= stack.Pop();
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion();
                    resultado %= stack.Pop();
                }
            }
            log.WriteLine(" = " + resultado);
            if (ejecuta)
            {
                Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                Console.WriteLine(variable + " = " + tipoDatoVariable);
                Console.WriteLine(resultado + " = " + tipoDatoResultado);
                Console.WriteLine("\nExpresion = " + tipoDatoExpresion);

                //Variable.TiposDatos tipoDatoMayor = tipoDatoVariable;

                if (tipoDatoVariable >= tipoDatoResultado)
                {
                    Modifica(variable, resultado);
                }
                else
                {
                    throw new Error("de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                }
            }
            match(";");
        }

        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            bool condicion = Condicion();
            while (condicion && ejecuta)
            {
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                condicion = Condicion();
            }
            // Implementamos la ejecucion del do - while
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
            bool evaluacion = Condicion() && ejecuta;
            match(")");
            match(";");

            if (evaluacion)
            {
                Do(ejecuta);
            }
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion(ejecuta);

            int inicia = caracter;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();

            log.WriteLine("For: " + variable);
            // Inicia un bucle 'do-while' que continúa mientras la condición de control sea verdadera.
            do
            {
                ejecuta = Condicion() && ejecuta; // Pasamos la condicion a la variable "ejecuta".
                match(";");
                resultado = Incremento(ejecuta); // Incrementamos en la variable "resultado" el incremento.
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                if (ejecuta)
                {
                    Modifica(variable, resultado);
                    archivo.DiscardBufferedData(); // Esta funcion se utiliza para limpiar el buffer del archivo.
                    caracter = inicia - variable.Length - 1; // Esta funcion se utiliza para regresar el puntero del archivo a la posicion inicial del for.
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin); // Y esta es para que el archivo lea desde la posicion inicial del for.
                    nextToken(); // Con esto, se obtiene el token del identificador.
                    linea = lineaInicio; // Y vuelvo a la linea inicial del for.
                }
            }
            while (ejecuta);
        }
        //Incremento -> Identificador ++ | --
        private float Incremento(bool ejecuta)
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            string variable = getContenido(); // Vamos a tener el nombre de la variable.
            match(Tipos.Identificador);
            // Ahora, obtenemos el valor de la variable
            // Mediante la funcion getValor() y en una variable de tipo float
            // Para poder incrementar o decrementar el valor.
            float resultado = getValor(variable);
            // Mediante el operador ++ o --.
            if (getContenido() == "++")
            {
                match("++");
                resultado++;
            }
            else
            {
                match("--");
                resultado--;
            }
            return resultado;
        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float R1 = stack.Pop();
            float R2 = stack.Pop();

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
            bool evaluacion = Condicion() && ejecuta;
            Console.WriteLine(evaluacion);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
            }

        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta)
        {
            match("printf");
            match("(");
            if (ejecuta)
            {
                string cadena = getContenido().TrimStart('"');
                cadena = cadena.Remove(cadena.Length - 1);
                cadena = cadena.Replace(@"\n", "\n");
                Console.Write(cadena);
            }
            match(Tipos.Cadena);
            if (getContenido() == ",")
            {
                match(",");
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                Console.Write(getValor(getContenido()));
                match(Tipos.Identificador);
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
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
            if (ejecuta)
            {
                string captura = " " + Console.ReadLine();
                float resultado = float.Parse(captura);
                Modifica(variable, resultado);
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
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (operador == "+")
                    stack.Push(R1 + R2);
                else
                    stack.Push(R1 - R2);
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
                else if (operador == "/")
                    stack.Push(R1 / R2);
                else
                    stack.Push(R1 % R2);
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                stack.Push(float.Parse(getContenido()));
                if (tipoDatoExpresion < getTipo(float.Parse(getContenido())))
                {
                    tipoDatoExpresion = getTipo(float.Parse(getContenido()));
                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }

                stack.Push(getValor(getContenido()));
                if (tipoDatoExpresion < getTipo(getContenido()))
                {
                    tipoDatoExpresion = getTipo(getContenido());
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TiposDatos tipoDatoCast = Variable.TiposDatos.Char;
                match("(");
                //Vamos a realizar un casteo, para reconocer el tipo de dato de la expresion.
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCast = true;
                    switch (getContenido())
                    {
                        //Con esto vamos a identificar el tipo de dato de la expresion.
                        case "int": tipoDatoCast = Variable.TiposDatos.Int; break;
                        case "float": tipoDatoCast = Variable.TiposDatos.Float; break;
                            //case "char": tipoDatoCast = Variable.TiposDatos.Char; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCast)
                {
                    tipoDatoExpresion = tipoDatoCast;
                    //Modificamos el resultado de la expresión.
                    stack.Push(Castea(stack.Pop(), tipoDatoCast));
                }
            }
        }
        //Funcion Castea -> Castea
        float Castea(float valor, Variable.TiposDatos tipoDato)
        {
            switch (tipoDato)
            {
                //Para desarrollar la funcion Castea se debe de dividir el valor entre el maximo valor que puede tener el tipo de dato.
                //Por ultimo se debe de sacar el modulo del resultado entre el maximo valor que puede tener el tipo de dato al que se quiere castear.
                //Vamos a utilizar la funcion "Math.Round()" para redondear el valor y despues hacer las operaciones.
                case Variable.TiposDatos.Char:
                    return ((char)Math.Round(valor)) % 256;
                case Variable.TiposDatos.Int:
                    return (int)Math.Round(valor) % 65536;
                default: return valor; //Si es float, no se hace nada.
            }
        }
    }
}