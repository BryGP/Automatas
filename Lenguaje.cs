using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    UNIDAD 3, TERCERA ENTREGA DE PROYECTO
    Requerimiento 1: Programar el printf
    Requerimiento 2: Programar el scanf ---> Buscar librerias para imprimir y leer
    Requerimiento 3: Programar ++, --, +=, -=, *=, /=, %=
    Requerimiento 4: Programar el else
    Requerimiento 5: Programar el do
    Requerimiento 6: Programar el while
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        int contIf, contFor, contWhile, contDo, contElse;

        Variable.TiposDatos tipoDatoExpresion;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
            contIf = contFor = contWhile = contDo = contElse = 1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
            contIf = contFor = contWhile = contDo = contElse = 1;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("org 100h");
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main(true);
            asm.WriteLine("int 20h");
            asm.WriteLine("RET");
            asm.WriteLine("define_scan_num");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num_uns");
            Imprime();
            asm.WriteLine("END");
        }

        private void Imprime()
        {
            log.WriteLine("-----------------");
            log.WriteLine("V a r i a b l e s");
            log.WriteLine("-----------------");
            asm.WriteLine("; V a r i a b l e s");
            foreach (Variable v in lista)
            {
                log.WriteLine(v.getNombre() + " " + v.getTiposDato() + " = " + v.getValor());
                asm.WriteLine(v.getNombre() + " dw 0h");
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

                asm.WriteLine("POP AX");
                asm.WriteLine("; Asignacion "+variable);
                asm.WriteLine("MOV " + variable + ", AX");
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
                    
                    asm.WriteLine("POP AX");
                    asm.WriteLine("ADD " + variable + ", AX");
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion();
                    resultado -= stack.Pop();

                    asm.WriteLine("POP AX");
                    asm.WriteLine("SUB " + variable + ", AX");
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion();
                    resultado *= stack.Pop();

                    asm.WriteLine("POP AX");
                    asm.WriteLine("MUL " + variable + ", AX");
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion();
                    resultado /= stack.Pop();

                    asm.WriteLine("POP BX");
                    asm.WriteLine("MOV AX" + variable);
                    asm.WriteLine("DIV BX");
                    asm.WriteLine("MOV " + variable + ", AX");
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion();
                    resultado %= stack.Pop();

                    asm.WriteLine("POP BX");
                    asm.WriteLine("MOV AX" + variable);
                    asm.WriteLine("DIV BX");
                    asm.WriteLine("MOV " + variable + ", DX");
                }
            }
            log.WriteLine(" = " + resultado);
            if (ejecuta)
            {
                Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                // Console.WriteLine(variable + " = "+tipoDatoVariable);
                // Console.WriteLine(resultado + " = "+tipoDatoResultado);
                // Console.WriteLine("expresion = "+tipoDatoExpresion);

                if (tipoDatoExpresion > tipoDatoResultado)
                {
                    tipoDatoResultado = tipoDatoExpresion;
                }
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
            int inicia = caracter;
            int lineaInicio = linea;
            bool primeraVez = true;

            string etiquetaInicio = "InicioWhile" + contWhile;
            string etiquetaFin    = "FinWhile" + contWhile++;

            log.WriteLine("while: ");
            asm.WriteLine(etiquetaInicio + ":");

            do
            {
                asm.WriteLine("; While: " + contWhile);
                match("while");
                match("(");
                ejecuta = Condicion(etiquetaFin) && ejecuta;
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
                    archivo.DiscardBufferedData();
                    caracter = inicia - 5;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }
                asm.WriteLine("JMP " + etiquetaInicio);
                primeraVez = false;
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin + ":");
        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta)
        {
            int inicia = caracter;
            int lineaInicio = linea;
            bool primeraVez = true;

            string etiquetaInicio = "InicioDo" + contDo;
            string etiquetaFin    = "FinDo" + contDo++;
            
            log.WriteLine("do: ");
            asm.WriteLine(etiquetaInicio + ":");

            do
            {
                asm.WriteLine("; Do: " + contDo);
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
                ejecuta = Condicion(etiquetaFin) && ejecuta;
                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = inicia - 2;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }
                asm.WriteLine("JMP " + etiquetaInicio);
                primeraVez = false;
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin + ":");
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion

        private void For(bool ejecuta)
        {
            asm.WriteLine("; For: " + contFor);
            match("for");
            match("(");
            Asignacion(ejecuta);

            string etiquetaInicio = "InicioFor" + contFor;
            string etiquetaFin    = "FinFor" + contFor++;

            int inicia = caracter;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();
            bool primeraVez = true;

            log.WriteLine("for: " + variable);
            asm.WriteLine(etiquetaInicio + ":");

            do
            {
                ejecuta = Condicion(etiquetaFin) && ejecuta;
                match(";");
                resultado = Incremento(ejecuta);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                if (getValor(variable) < resultado)
                {
                    asm.WriteLine("INC " + variable);
                }
                else if (getValor(variable) > resultado)
                {
                    asm.WriteLine("DEC " + variable);
                }
                if (ejecuta)
                {
                    Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                    Variable.TiposDatos tipoDatoResultado = getTipo(resultado);
                    if (tipoDatoVariable >= tipoDatoResultado)
                    {
                        Modifica(variable, resultado);
                    }
                    else
                    {
                        throw new Error("de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                    }
                    archivo.DiscardBufferedData();
                    caracter = inicia - variable.Length - 1;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;                    
                }
                asm.WriteLine("JMP " + etiquetaInicio);
                primeraVez = false;
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin+":");
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(bool ejecuta)
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            string variable = getContenido();
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                return getValor(variable) + 1;
            }
            else
            {
                match("--");
                return getValor(variable) - 1;
            }
        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float R1 = stack.Pop();  // Expresion 2
            float R2 = stack.Pop();  // Expresion 1

            asm.WriteLine("POP BX"); // Expresion 2
            asm.WriteLine("POP AX"); // Expresion 1

            asm.WriteLine("CMP AX, BX");

            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE "+etiqueta);
                    return R2 == R1;
                case ">": 
                    asm.WriteLine("JLE "+etiqueta);
                    return R2 > R1;
                case ">=": 
                    asm.WriteLine("JL "+etiqueta);
                    return R2 >= R1;
                case "<": 
                    asm.WriteLine("JGE "+etiqueta);
                    return R2 < R1;
                case "<=": 
                    asm.WriteLine("JG "+etiqueta);
                    return R2 <= R1;
                default: 
                    asm.WriteLine("JE "+etiqueta);
                    return R2 != R1;
            }
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If(bool ejecuta)
        {
            match("if");
            match("(");

            string etiqueta = "Eif" + contIf++;
            bool evaluacion = Condicion(etiqueta) && ejecuta;

            asm.WriteLine("; If: " + contIf);
            asm.WriteLine("; Condicion " + etiqueta);
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion && ejecuta);
            }
            else
            {
                Instruccion(evaluacion && ejecuta);
            }            

            if (getContenido() == "else")
            {
                string etiquetaElse = "Else" + contElse++;
                asm.WriteLine("; Else: " + etiqueta);
                match("else");                

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!evaluacion && ejecuta);
                }
                else
                {
                    Instruccion(!evaluacion && ejecuta);
                }
                asm.WriteLine(etiqueta + ":");                
            }
        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta)
        {
            asm.WriteLine("; Printf");
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
                if (ejecuta)
                {
                    Console.Write(getValor(getContenido()));
                }
                asm.WriteLine("Print " + getValor(getContenido()) + " " + "\"");
                match(Tipos.Identificador);
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
        {
            asm.WriteLine("; Scanf");
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
                string captura = "" + Console.ReadLine();
                float resultado = float.Parse(captura);
                Modifica(variable, resultado);
            }
            asm.WriteLine("Call scan_num");
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

                asm.WriteLine("POP BX");
                asm.WriteLine("POP AX");

                if (operador == "+")
                {
                    stack.Push(R1 + R2);
                    asm.WriteLine("ADD AX, BX");
                    asm.WriteLine("PUSH AX");
                }
                else
                {
                    stack.Push(R1 - R2);
                    asm.WriteLine("SUB AX, BX");
                    asm.WriteLine("PUSH AX");
                }
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

                asm.WriteLine("POP BX");
                asm.WriteLine("POP AX");

                if (operador == "*")
                {
                    stack.Push(R1 * R2);
                    asm.WriteLine("MUL  BX");
                    asm.WriteLine("PUSH AX");
                }
                else if (operador == "/")
                {
                    stack.Push(R1 / R2);
                    asm.WriteLine("DIV  BX");
                    asm.WriteLine("PUSH AX");
                }
                else
                {
                    stack.Push(R1 % R2);
                    asm.WriteLine("DIV  BX");
                    asm.WriteLine("PUSH DX");
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                asm.WriteLine("MOV AX, "+getContenido());
                asm.WriteLine("PUSH AX");
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
                asm.WriteLine("MOV AX, "+getContenido());
                asm.WriteLine("PUSH AX");

                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);

                if (tipoDatoExpresion < getTipo(getContenido()))
                {
                    tipoDatoExpresion = getTipo(getContenido());
                }
            }
            else
            {
                bool huboCast = false;
                Variable.TiposDatos tipoDatoCast = Variable.TiposDatos.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCast = true;
                    switch (getContenido())
                    {
                        case "int": tipoDatoCast = Variable.TiposDatos.Int; break;
                        case "float": tipoDatoCast = Variable.TiposDatos.Float; break;
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
                    stack.Push(castea(stack.Pop(), tipoDatoCast));
                    asm.WriteLine("POP AX");
                }
            }
        }
        float castea(float resultado, Variable.TiposDatos tipoDato)
        {
            switch (tipoDato)
            {
                case Variable.TiposDatos.Char: return MathF.Round(resultado) % 256;
                asm.WriteLine("POP");
                asm.WriteLine("PUSH");

                case Variable.TiposDatos.Int : return MathF.Round(resultado) % 65536;
                asm.WriteLine("POP");
                asm.WriteLine("PUSH");
            }
            return resultado;
        }
    }
}