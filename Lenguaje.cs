using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/*
    Requerimiento 1: Programar scanf 
    Requerimiento 2: Programar printf
    Requerimiento 3: Programar ++,--,+=,-=,*=,/=,%=
    Requerimiento 4: Programar else
    Requerimiento 5: Programar do para que gerenre una sola vez el codigo
    Requerimiento 6: Programar while para que gerenre una sola vez el codigo
    Requerimiento 7: Programar el for para que gerenre una sola vez el codigo
    Requerimiento 8: Programar el CAST
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        int contIf, contFor, contWh, contDo, contEl;
        bool primeraVez = true;


        Variable.TiposDatos tipoDatoExpresion;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
            contIf = contEl = contFor = contWh = contDo = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
            contIf = contEl = contFor = contWh = contDo = 0;
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
        private void BloqueInstrucciones(bool ejecuta, bool primeraVez)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta, primeraVez);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta, bool primeraVez)
        {
            Instruccion(ejecuta, primeraVez);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta, primeraVez);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion(bool ejecuta, bool primeraVez)
        {
            if (getContenido() == "printf")
            {
                Printf(ejecuta, primeraVez);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(ejecuta, primeraVez);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta, primeraVez);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta, primeraVez);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta, primeraVez);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta, primeraVez);
            }
            else
            {
                Asignacion(ejecuta, primeraVez);
            }
        }
        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta, bool primeraVez)
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
                Expresion(primeraVez);
                resultado = stack.Pop();
                asm.WriteLine("POP AX");
                asm.WriteLine("; Asignacion " + variable);
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
                    Expresion(primeraVez);
                    resultado += stack.Pop();
                    asm.WriteLine("POP AX");
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion(primeraVez);
                    resultado -= stack.Pop();
                    asm.WriteLine("POP AX");
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion(primeraVez);
                    resultado *= stack.Pop();
                    asm.WriteLine("POP AX");
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion(primeraVez);
                    resultado /= stack.Pop();
                    asm.WriteLine("POP AX");
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion(primeraVez);
                    resultado %= stack.Pop();
                    asm.WriteLine("POP AX");
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
        private void While(bool ejecuta, bool primeraVez)
        {
            int inicia = caracter;
            int lineaInicio = linea;

            log.WriteLine("while: ");
            do
            {
                match("while");
                match("(");
                ejecuta = Condicion("", true) && ejecuta;
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }
                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = inicia - 5;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }
                asm.WriteLine("JMP InicioWhile" + contWh);
            }
            while (ejecuta);
        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta, bool primeraVez)
        {
            int inicia = caracter;
            int lineaInicio = linea;

            log.WriteLine("do: ");
            asm.WriteLine("; Do: " + contDo);

            do
            {
                match("do");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, true);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }
                match("while");
                match("(");
                ejecuta = Condicion("", true) && ejecuta;
                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = inicia - 2;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }
            }
            while (ejecuta);
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion

        private void For(bool ejecuta, bool primera)
        {
            asm.WriteLine("; For: " + contFor);
            match("for");
            match("(");
            Asignacion(ejecuta, primeraVez);

            string etiquetaInicio = "InicioFor" + contFor;
            string etiquetaFin = "FinFor" + contFor++;

            int inicia = caracter;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();

            log.WriteLine("for: " + variable);
            if (primeraVez)
            {
                asm.WriteLine(etiquetaInicio + ":");
            }

            bool inc = false;
            bool dec = false;

            do
            {
                ejecuta = Condicion(etiquetaFin, primeraVez) && ejecuta;
                match(";");
                resultado = Incremento(ejecuta);
                match(")");

                // Agregamos la condiciones para saber si es un incremento o decremento
                if (getValor(variable) < resultado)
                {
                    inc = true;
                    dec = false;
                }
                else if (getValor(variable) > resultado)
                {
                    inc = false;
                    dec = true;
                }
                else
                {
                    inc = false;
                    dec = false;
                }

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }
                primeraVez = false;

                // Generamos el incremento o decremento en asm
                if (primeraVez)
                {
                    if (inc)
                    {
                        asm.WriteLine("INC " + variable);
                    }
                    else if (dec)
                    {
                        asm.WriteLine("DEC " + variable);
                    }
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
                if (primeraVez)
                {
                    asm.WriteLine("JMP " + etiquetaInicio);
                }
            }
            while (ejecuta);

            if (primeraVez)
            {
                asm.WriteLine(etiquetaFin + ":");
            }
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
        private bool Condicion(string etiqueta, bool primeraVez)
        {
            Expresion(primeraVez);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(primeraVez);
            float R1 = stack.Pop();  // Expresion 2
            float R2 = stack.Pop();  // Expresion 1

            if (primeraVez)
            {
                asm.WriteLine("POP BX"); // Expresion 2
                asm.WriteLine("POP AX"); // Expresion 1
                asm.WriteLine("CMP AX, BX");
            }
            switch (operador)
            {
                case "==":
                    if (primeraVez) asm.WriteLine("JNE " + etiqueta);
                    return R2 == R1;
                case ">":
                    if (primeraVez) asm.WriteLine("JBE " + etiqueta);
                    return R2 > R1;
                case ">=":
                    if (primeraVez) asm.WriteLine("JB " + etiqueta);
                    return R2 >= R1;
                case "<":
                    if (primeraVez) asm.WriteLine("JAE " + etiqueta);
                    return R2 < R1;
                case "<=":
                    if (primeraVez) asm.WriteLine("JA " + etiqueta);
                    return R2 <= R1;
                default:
                    if (primeraVez) asm.WriteLine("JE " + etiqueta);
                    return R2 != R1;
            }
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If(bool ejecuta, bool primeraVez)
        {
            match("if");
            match("(");
            asm.WriteLine("; if: " + contIf);
            string etiqueta = "Eif" + contIf++;
            bool evaluacion = Condicion(etiqueta, true);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion && ejecuta, true);
            }
            else
            {
                Instruccion(evaluacion && ejecuta, true);
            }
            asm.WriteLine(etiqueta + ":");
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!evaluacion && ejecuta, true);
                }
                else
                {
                    Instruccion(!evaluacion && ejecuta, true);
                }
            }
        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta, bool primeraVez)
        {
            match("printf");
            match("(");
            if (ejecuta)
            {
                string cadena = getContenido().TrimStart('"');
                cadena = cadena.Remove(cadena.Length - 1);
                cadena = cadena.Replace(@"\n", "\n");

                string printasm = getContenido().TrimStart('"');
                printasm = printasm.Remove(printasm.Length - 1);
                printasm = printasm.Replace(@"\n", " ");

                string lbprint = "PRINTF";
                string lbend = "$";
                Console.Write(cadena);

                //Generamos el printf en asm
                if (primeraVez)
                {
                    asm.WriteLine("PRINT " + "\"" + getContenido().Replace("\"", "").Replace("\\n", " ") + "\"");

                    //Generamos el printf en asm
                    asm.WriteLine("MOV AH, 09h"); // Funcion para imprimir cadena
                    asm.WriteLine("MOV DX, OFFSET " + lbprint); // Dirección de la cadena a imprimir
                    asm.WriteLine("INT 21h"); // Interrupcion para imprimir
                    asm.WriteLine("JMP " + lbend + "Fin Printf");
                    asm.WriteLine(lbprint + ": DB '" + printasm + "$" + "'");
                    asm.WriteLine(lbprint + "Fin:");
                }
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

                    //Generamos el printf en asm
                    asm.WriteLine("MOV AH, 02h"); // Funcion para imprimir caracter
                    asm.WriteLine("MOV DL, " + getContenido()); // Caracter a imprimir
                    asm.WriteLine("INT 21h"); // Interrupcion para imprimir
                }
                match(Tipos.Identificador);
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta, bool primeraVez)
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
                string captura = "" + Console.ReadLine();
                float resultado = float.Parse(captura);
                Modifica(variable, resultado);

                // Generar código ensamblador para simular la entrada
                asm.WriteLine("MOV AH, 0Ah");  // AH=0Ah para leer una cadena
                asm.WriteLine("MOV DX, OFFSET buffer"); // buffer es donde se almacenará la entrada
                asm.WriteLine("INT 21h");  // Llama a la interrupción 21h para leer una cadena
                asm.WriteLine("MOV SI, DX");  // SI apunta al inicio de la cadena
            }
            // Llamamos el scanf en asm
            asm.WriteLine("call scan_num");
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
            BloqueInstrucciones(ejecuta, true);
        }
        //Expresion -> Termino MasTermino
        private void Expresion(bool primeraVez)
        {
            Termino(primeraVez);
            MasTermino(primeraVez);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool primeraVez)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(primeraVez);
                log.Write(" " + operador);

                float R2 = stack.Pop();
                float R1 = stack.Pop();

                if (primeraVez)
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                }
                if (operador == "+")
                {
                    stack.Push(R1 + R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                    }
                }
                else
                {
                    stack.Push(R1 - R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                    }
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool primeraVez)
        {
            Factor(primeraVez);
            PorFactor(primeraVez);
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor(bool primeraVez)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(primeraVez);
                log.Write(" " + operador);

                float R2 = stack.Pop();
                float R1 = stack.Pop();

                if (primeraVez)
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                }
                if (operador == "*")
                {
                    stack.Push(R1 * R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                    }
                }
                else if (operador == "/")
                {
                    stack.Push(R1 / R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("DIV  BX");
                        asm.WriteLine("PUSH AX");
                    }
                }
                else
                {
                    stack.Push(R1 % R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("DIV  BX");
                        asm.WriteLine("PUSH DX");
                    }
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool primeraVez)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                if (primeraVez)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }

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

                if (primeraVez)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }

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
                Expresion(primeraVez);
                match(")");
                if (huboCast)
                {
                    tipoDatoExpresion = tipoDatoCast;
                    stack.Push(castea(stack.Pop(), tipoDatoCast));

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                    }
                }
            }
        }
        float castea(float resultado, Variable.TiposDatos tipoDato)
        {
            switch (tipoDato)
            {
                case Variable.TiposDatos.Char: return MathF.Round(resultado) % 256;
                case Variable.TiposDatos.Int: return MathF.Round(resultado) % 65536;
            }
            return resultado;
        }
    }
}