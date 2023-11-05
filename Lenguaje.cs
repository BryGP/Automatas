using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/*
    [LISTO]   Requerimiento 1: Programar scanf 
    [LISTO]   Requerimiento 2: Programar printf
    [LISTO]   Requerimiento 3: Programar ++,--,+=,-=,*=,/=,%=
    [LISTO]   Requerimiento 4: Programar else
    [DETAILS] Requerimiento 5: Programar do para que genere una sola vez el codigo
    [DETAILS] Requerimiento 6: Programar while para que genere una sola vez el codigo
    [LISTO]   Requerimiento 7: Programar el for para que genere una sola vez el codigo
    [HELP!]   Requerimiento 8: Programar el CAST
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
                Scanf(ejecuta);
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

                if (primeraVez)
                {
                    asm.WriteLine("POP AX");
                    asm.WriteLine("; Asignacion " + variable);
                    asm.WriteLine("MOV " + variable + ", AX");
                }
            }

            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    resultado = getValor(variable) + 1;
                    if (primeraVez)
                    {
                        asm.WriteLine("INC " + variable);
                    }
                }
                else
                {
                    match("--");
                    resultado = getValor(variable) - 1;
                    if (primeraVez)
                    {
                        asm.WriteLine("DEC " + variable);
                    }
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

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("ADD " + variable + ", AX");
                    }
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion(primeraVez);
                    resultado -= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("SUB " + variable + ", AX");
                    }
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion(primeraVez);
                    resultado *= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("MUL " + variable + ", AX");
                    }
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion(primeraVez);
                    resultado /= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("DIV " + variable + ", AX");
                    }
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion(primeraVez);
                    resultado %= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("MOD" + variable + ", AX");
                        primeraVez = false;
                    }
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
            log.WriteLine("while: ");
            asm.WriteLine("; While: " + contWh);

            int inicia = caracter;
            int lineaInicio = linea;

            string etiquetaInicio = "IniWhile" + (contWh++);
            string etiquetaFin = "FinWhile" + (contWh);

            if (primeraVez)
            {
                asm.WriteLine(etiquetaInicio + ":");
            }

            do
            {
                match("while");
                match("(");
                ejecuta = Condicion(etiquetaFin, primeraVez) && ejecuta;
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
                asm.WriteLine("JMP " + etiquetaInicio);
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin + ":");
            asm.WriteLine();
            match(")");
            match(";");
        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta, bool primeraVez)
        {
            log.WriteLine("Do While: ");
            asm.WriteLine();
            asm.WriteLine("; Do While: " + contDo);
            match("do");

            string etiquetaInicio = "InicioDo" + (contDo++);
            string etiquetaFin = "FinDo" + (--contDo);

            int inicia = caracter;
            int lineaInicio = linea;

            if (primeraVez)
            {
                asm.WriteLine(etiquetaInicio + ":");
            }

            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }
                primeraVez = false;

                match("while");
                match("(");
                ejecuta = Condicion(etiquetaFin, primeraVez) && ejecuta;
                match(")");
                match(";");

                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = inicia - 2;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }
         
                if (primeraVez)
                {
                    asm.WriteLine("CMP AX, BX");
                    asm.WriteLine("JMP " + etiquetaInicio);
                }
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin + ":");
            asm.WriteLine();
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta, bool primeraVez)
        {
            log.WriteLine("for: ");
            asm.WriteLine();
            asm.WriteLine("; For: " + contFor);
            match("for");
            match("(");
            Asignacion(ejecuta, primeraVez);

            string etiquetaInicio = "InicioFor" + (contFor++);
            string etiquetaFin = "FinFor" + (--contFor);

            int inicia = caracter;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();

            log.WriteLine("for de: " + variable);
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

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }
                primeraVez = false;

                // Agregamos las banderas para saber si es un incremento o decremento
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
                    asm.WriteLine("INC " + variable);  // Agregar esta línea para incrementar i
                    asm.WriteLine("JMP " + etiquetaInicio);
                }
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin + ":");
            asm.WriteLine(); // Salto de linea            
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
                    if (primeraVez) asm.WriteLine("JLE " + etiqueta);
                    return R2 > R1;
                case ">=":
                    if (primeraVez) asm.WriteLine("JL " + etiqueta);
                    return R2 >= R1;
                case "<":
                    if (primeraVez) asm.WriteLine("JGE " + etiqueta);
                    return R2 < R1;
                case "<=":
                    if (primeraVez) asm.WriteLine("JG " + etiqueta);
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
            string etiquetaIf = "IniIf" + (contIf++);
            string etiquetafin = "Iffin" + contIf;
            string etiquetaElse = "Else" + contEl;

            if (primeraVez)
            {
                asm.WriteLine(etiquetaIf + ":");
                asm.WriteLine("; Condicion del " + etiquetaIf);
            }
            bool evaluacion = Condicion(etiquetaIf, primeraVez) && ejecuta;
            Console.WriteLine(evaluacion);
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion && ejecuta, primeraVez);
            }
            else
            {
                Instruccion(evaluacion && ejecuta, primeraVez);
            }
            if (primeraVez)
            {
                asm.WriteLine("JMP " + etiquetafin);
            }

            if (getContenido() == "else")
            {
                if (primeraVez)
                {
                    asm.WriteLine("; Condicion del " + etiquetaIf);
                    asm.WriteLine(etiquetaElse + ":");
                }
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!evaluacion && ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(!evaluacion && ejecuta, primeraVez);
                }
            }
            if (primeraVez)
            {
                asm.WriteLine(etiquetafin + ":");
                asm.WriteLine();
            }
        }

        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta, bool primeraVez)
        {
            asm.WriteLine();
            asm.WriteLine("; Printf");
            match("printf");
            match("(");

            if (getClasificacion() == Tipos.Cadena) // Si se clasifica como una cadena, entonces...
            {
                string cadena = getContenido().TrimStart('"');
                cadena = cadena.Remove(cadena.Length - 1);
                cadena = cadena.Replace(@"\n", "\n");

                if (ejecuta)
                {
                    // Cadena con ASM
                    asm.WriteLine("print " + "\"" + getContenido().Replace("\"", "").Replace("\\n", ".") + "\"");
                    // Elimina el \n y da un salto de línea
                    Console.Write(getContenido().Replace("\"", "").Replace("\\n", Environment.NewLine));
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
                    if (primeraVez)
                    {
                        asm.WriteLine("call print_num");
                    }
                }
                match(Tipos.Identificador);
                primeraVez = false;
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
        {
            asm.WriteLine();
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

                if (primeraVez)
                {
                    asm.WriteLine("MOV " + variable + ", CX");
                    asm.WriteLine("call scan_num");
                    asm.WriteLine("MOV AX, CX");
                    asm.WriteLine();
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
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                    }
                }
                else
                {
                    stack.Push(R1 % R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("DIV BX");
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
                log.WriteLine(" " + getContenido());
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
                case Variable.TiposDatos.Char:
                    return MathF.Round(resultado) % 256;
                    asm.WriteLine("POP");
                    asm.WriteLine("PUSH");

                case Variable.TiposDatos.Int:
                    return MathF.Round(resultado) % 65536;
                    asm.WriteLine("POP");
                    asm.WriteLine("PUSH");
            }
            return resultado;
        }
    }
}