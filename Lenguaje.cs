using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Semantica
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        ListaAtributos atributos;
        int Zona;
        int TipoDato;
        //int TipoDatoExpresion;
        int TipoPredominante;
        int contadorIf;
        int contadorFor;

        public Lenguaje() : base()
        {
            atributos = new ListaAtributos();
            contadorIf = 0;
            contadorFor = 0;
        }

        public Lenguaje(string ruta) : base(ruta)
        {
            atributos = new ListaAtributos(20);
            contadorIf = 0;
            contadorFor = 0;
        }
        public new void Dispose()
        {
            base.Dispose();
        }
        public void Programa()
        {
            try
            {
                Librerias();
                main();
            }
            catch (CompiladorException ex)
            {
                ImprimirError(ex);
                throw;
            }
        }

        public void Librerias()
        {
            Libreria();
            if (GETContenido() == "using") //Caso base
            {
                Librerias();
            }
        }

        public void Libreria()
        {
            MATCH("using");
            SubLibreria();
            MATCH(";");
        }

        public void SubLibreria()
        {
            MATCH(identificador);
            if (GETContenido() == ".")
            {
                MATCH(".");
                SubLibreria();
            }
        }
        public void main()
        {
            MATCH("namespace");
            MATCH(identificador);
            MATCH(inicioBloque);
            Clase();
            MATCH(finBloque);
        }

        public void Clase()
        {
            MATCH("class");
            MATCH("Program");
            MATCH(inicioBloque);

            AsmIncludes();
            Asm.WriteLine("SECTION .data");
            Asm.WriteLine("    bufferIn times 255 db 0");
            Atributos();
            MetodoMain();
            atributos.Desplegar();
            Log.WriteLine("\n" + atributos.ToStr());
            MATCH(finBloque);
        }

        public void Atributos()
        {
            Atributo();
            MATCH(finSentencia);
            if (GETClasificacion() == zona || GETClasificacion() == tipoDato)
            {
                Atributos();
            }
        }

        public void Atributo()
        {
            Zona = 0; // Por omisión private
            if (GETClasificacion() == zona)
            {
                switch (GETContenido())
                {
                    case "protected":
                        Zona = 1;
                        break;
                    case "public":
                        Zona = 2;
                        break;
                }
                MATCH(zona);
            }
            TipoDato = Semantica.Atributo.StrToTipo(GETContenido());
            MATCH(tipoDato);
            ListaIdentificadores();
        }

        public void ListaIdentificadores()
        {
            if (GETClasificacion() == identificador && !atributos.Existe(GETContenido()))
            {
                atributos.Agregar(Zona, TipoDato, GETContenido());
            }
            else
            {
                // : lanzar excepcion AtributoDuplicado
                throw new AtributoDeclaradoException(renglon, columna, GETContenido());
            }
            Asm.Write(new String(' ', 4) + GETContenido() + ":");
            if (TipoDato == 0)
            {
                // Asm.WriteLine(" db ?");
                Asm.WriteLine(" db 0");
            }
            else if (TipoDato == 1)
            {
                // Asm.WriteLine(" dw ?");
                Asm.WriteLine(" dw 0");
            }
            // Validar para flotantes
            else
            {
                Asm.WriteLine(" dd 0.0");
            }
            MATCH(identificador);
            if (GETContenido() == ",")
            {
                MATCH(",");
                ListaIdentificadores();
            }
        }

        public void MetodoMain()
        {
            AsmInicioPrograma();

            MATCH("static");
            MATCH("void");
            MATCH("Main");
            MATCH("(");
            MATCH("string");
            MATCH("[");
            MATCH("]");
            MATCH("args");
            MATCH(")");
            MATCH(inicioBloque);
            ListaInstrucciones(true);
            MATCH(finBloque);

            AsmFinPrograma();
        }

        public void ListaInstrucciones(bool DebeEjecutar)
        {
            Instruccion(true);
            if (GETClasificacion() != finBloque)
            {
                ListaInstrucciones(DebeEjecutar);
            }
        }

        public void Instruccion(bool DebeEjecutar)
        {
            if (GETContenido() == "Console")
            {
                MATCH("Console");
                MATCH(".");
                if (GETContenido() == "WriteLine")  //ToDo: Implementar WriteLine en ensamblador 
                {
                    MATCH("WriteLine");
                    MATCH("(");
                    if (GETClasificacion() == cadena)
                    {
                        string cadenaAImprimir = GETContenido();
                        cadenaAImprimir = cadenaAImprimir.Substring(1, cadenaAImprimir.Length - 2);
                        if (DebeEjecutar)
                        {
                            Console.WriteLine(cadenaAImprimir);
                        }
                        AsmWriteString(cadenaAImprimir);
                        MATCH(cadena);
                    }
                    else if (GETClasificacion() == identificador)
                    {
                        string nombre = GETContenido();
                        // Lanzar excepcion Si el atributo No existe
                        if (!atributos.Existe(nombre))
                        {
                            throw new AtributoNoDeclaradoException(nombre, renglon, columna);
                        }
                        MATCH(identificador);
                        // Imprimir en pantalla el valor del atributo
                        switch (atributos.GETTipo(nombre))
                        {
                            case 0:
                                Asm.WriteLine("mov rax, [" + nombre + "]");
                                Asm.WriteLine("call writeChar");
                                break;
                            case 1:
                                Asm.WriteLine("mov rax, [" + nombre + "]");
                                Asm.WriteLine("call writeInt");
                                break;
                            case 2:
                                Asm.WriteLine("mov xmm0, [" + nombre + "]");
                                Asm.WriteLine("call writeFloat");
                                break;
                        }
                        if (DebeEjecutar)
                        {
                            Console.WriteLine(atributos.GETValor(nombre));
                        }
                    }
                    else if (GETContenido() == ")")
                    {
                        AsmWriteString("");
                        if (DebeEjecutar)
                        {
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        if (DebeEjecutar)
                        {
                            Console.WriteLine(Expresion());
                        }
                    }
                    MATCH(")");
                }
                else if (GETContenido() == "ReadLine")  // ToDo: Implementar ReadLine en ensamblador
                {
                    MATCH("ReadLine");
                    MATCH("(");
                    if (GETContenido() == ")")
                    {
                        MATCH(")");
                        AsmReadLine();
                        if (DebeEjecutar)
                        {
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        string nombre = GETContenido();
                        if (!atributos.Existe(nombre))
                        {
                            throw new AtributoNoDeclaradoException(nombre, renglon, columna);
                        }
                        MATCH(identificador);
                        if (DebeEjecutar)
                        {
                            atributos.SETValor(nombre, ReadLine());
                        }
                        MATCH(")");
                        Asm.WriteLine("mov rax, bufferIn");
                        Asm.WriteLine("mov rbx, " + nombre);
                        switch (atributos.GETTipo(nombre))
                        {
                            case 0:
                                Asm.WriteLine("call readChar");
                                break;
                            case 1:
                                Asm.WriteLine("call readInt");
                                break;
                            case 2:
                                Asm.WriteLine("call readFloat");
                                break;
                        }
                    }
                }
                else  // Implementar ReadKey en ensamblador
                {
                    MATCH("ReadKey");
                    MATCH("(");
                    MATCH(")");
                    AsmReadKey();
                    if (DebeEjecutar)
                    {
                        Console.ReadKey();
                    }
                }
                MATCH(finSentencia);
            }
            else if (GETContenido() == "if")
            {
                Condicional(DebeEjecutar);
            }
            else if (GETContenido() == "for")
            {
                CicloFor(DebeEjecutar);
            }
            else
            {
                string nombre = GETContenido();
                if (!atributos.Existe(nombre))
                {
                    throw new AtributoNoDeclaradoException(nombre, renglon, columna);
                }
                Log.Write(nombre + " = ");
                MATCH(identificador);
                MATCH(asignacion);
                TipoPredominante = -1;
                float valor = Expresion();
                if (EvaluaExpresion(valor) > TipoPredominante)
                {
                    TipoPredominante = EvaluaExpresion(valor);
                }
                if (atributos.GETTipo(nombre) < TipoPredominante)
                {
                    throw new CompiladorException("Tipo incompatible\n" +
                    "un dato tipo " + Semantica.Atributo.TipoToStr(TipoPredominante) +
                    " no puede ser asignado a (" + atributos.GETTipoStr(nombre) + " " + nombre + ")\n" +
                    "Renglón: " + renglon + ", Columna: " + columna);
                }
                MATCH(finSentencia);
                Asm.WriteLine("pop rax"); // resultado de la evaluacion
                if (DebeEjecutar)
                {
                    atributos.SETValor(nombre, valor);
                    Asm.WriteLine("mov [" + nombre + "], rax");
                }
                Log.WriteLine();
                Log.WriteLine(nombre + " = " + atributos.GETValor(nombre));
            }
        }

        public float Expresion()
        {
            return MasTermino();
        }

        public float Termino()
        {
            return PorFactor();//Factor() * PorFactor();
        }

        public float MasTermino()
        {
            float A = Termino();
            int tipoPredominante = TipoPredominante;
            if (GETClasificacion() == operadorTermino)
            {
                string operador = GETContenido();
                MATCH(operadorTermino);
                float t = Termino();
                Log.Write(operador + " ");
                Asm.WriteLine("pop rbx");
                Asm.WriteLine("pop rax");
                if (EvaluaExpresion(t) > TipoPredominante)
                {
                    TipoPredominante = EvaluaExpresion(t);
                }
                if (tipoPredominante > TipoPredominante)
                {
                    TipoPredominante = tipoPredominante;
                }
                if (operador == "+")
                {
                    Asm.WriteLine("add rax, rbx");
                    Asm.WriteLine("push rax");
                    return A + t;
                }
                else
                {
                    Asm.WriteLine("sub rax, rbx");
                    Asm.WriteLine("push rbx");
                    return A - t;
                }
            }
            return A + 0f;
        }

        public float Factor()
        {
            if (GETContenido() == "true")
            {
                MATCH("true");
                return 1f;
            }
            else if (GETContenido() == "false")
            {
                MATCH("false");
                return 0f;
            }
            else if (GETClasificacion() == identificador)
            {
                string nombre = GETContenido();
                if (!atributos.Existe(nombre))
                {
                    throw new AtributoNoDeclaradoException(nombre, renglon, columna);
                }
                if (atributos.GETTipo(nombre) > TipoPredominante)
                {
                    TipoPredominante = atributos.GETTipo(nombre);
                }
                Log.Write(GETContenido() + " ");
                MATCH(identificador);
                Asm.WriteLine("mov rax, [" + nombre + "]");
                Asm.WriteLine("push rax");
                return atributos.GETValor(nombre);
            }
            else if (GETClasificacion() == numero)
            {
                Log.Write(GETContenido() + " ");
                Asm.WriteLine("mov rax, " + GETContenido());
                Asm.WriteLine("push rax");
                float n = float.Parse(GETContenido());
                if (EvaluaExpresion(n) > TipoPredominante)
                {
                    TipoPredominante = EvaluaExpresion(n);
                }
                MATCH(numero);
                return n;
            }
            else
            {
                //  Implementar cast 60%
                MATCH("(");
                string tipoCast = "";
                if (GETClasificacion() == tipoDato)
                {
                    tipoCast = GETContenido();
                    //TipoPredominante = Semantica.Atributo.StrToTipo(tipoCast);
                    MATCH(tipoDato);
                    MATCH(")");
                    MATCH("(");
                }
                float A = Expresion();
                if (tipoCast.Length > 0)
                {
                    TipoPredominante = Semantica.Atributo.StrToTipo(tipoCast);
                    if (TipoPredominante == 0)
                    {
                        A = (byte)A;
                    }
                    else if (TipoPredominante == 1)
                    {
                        A = (Int16)A;
                    }
                }
                MATCH(")");
                return A;
            }
        }

        public float PorFactor()
        {
            float A = Factor();
            int tipoPredominante = TipoPredominante;
            if (GETClasificacion() == operadorFactor)
            {
                string operador = GETContenido();
                MATCH(operadorFactor);
                float f = Factor();
                Log.Write(operador + " ");
                Asm.WriteLine("pop rbx");
                Asm.WriteLine("pop rax");
                if (EvaluaExpresion(f) > TipoPredominante)
                {
                    TipoPredominante = EvaluaExpresion(f);
                }
                if (tipoPredominante > TipoPredominante)
                {
                    TipoPredominante = tipoPredominante;
                }
                if (operador == "*")
                {
                    Asm.WriteLine("mul rbx");
                    Asm.WriteLine("push rax");
                    return A * f;
                }
                else if (operador == "/")
                {
                    Asm.WriteLine("div rbx");
                    Asm.WriteLine("push rax");
                    return A / f;
                }
                else
                {
                    Asm.WriteLine("div rbx");
                    Asm.WriteLine("push rdx");
                    return A % f;
                }
            }
            return A;
        }

        private int EvaluaExpresion(float expresion)
        {
            if (expresion % 1 != 0)
            {
                return 2;
            }
            else if (expresion < 256)
            {
                return 0;  // char
            }
            else if (expresion < 65536)
            {
                return 1;  // int
            }
            return 2;  // float
        }

        public void Condicional(bool DebeEjecutar)
        {
            int numEtiqueta = contadorIf;
            contadorIf++;
            MATCH("if");
            MATCH("(");
            bool P = Condicion("if", numEtiqueta);
            MATCH(")");
            Asm.WriteLine("\nbeginIf" + numEtiqueta + ":");
            if (GETClasificacion() == inicioBloque)
            {
                MATCH(inicioBloque);
                if (GETClasificacion() == finBloque)
                {
                    MATCH(finBloque);
                    return;
                }
                //Instruccion(DebeEjecutar ? P : false);
                ListaInstrucciones(DebeEjecutar ? P : false);
                MATCH(finBloque);
                Asm.WriteLine("jmp endElse" + numEtiqueta);
                Asm.WriteLine("endIf" + numEtiqueta + ":\n");
            }
            else
            {
                Instruccion(DebeEjecutar ? P : false);
                Asm.WriteLine("jmp endElse" + numEtiqueta);
                Asm.WriteLine("endIf" + numEtiqueta + ":\n");
            }
            if (GETContenido() == "else")
            {
                Asm.WriteLine("beginElse" + numEtiqueta + ":");
                MATCH("else");
                if (GETClasificacion() == inicioBloque)
                {
                    MATCH(inicioBloque);
                    if (GETClasificacion() == finBloque)
                    {
                        MATCH(finBloque);
                    }
                    else
                    {
                        ListaInstrucciones(DebeEjecutar ? !P : false);
                        MATCH(finBloque);
                    }
                }
                else
                {
                    Instruccion(DebeEjecutar ? !P : false);
                }
            }
            Asm.WriteLine("endElse" + numEtiqueta + ":\n");
        }

        public bool Condicion(string etiqueta, int numEtiqueta)
        {
            return ExpresionBooleana(etiqueta, numEtiqueta); // || ORExpresionBooleana(etiqueta);
        }

        private bool ExpresionBooleana(string etiqueta, int numEtiqueta)
        {
            return ORExpresionBooleana(etiqueta, numEtiqueta);
        }

        private bool ORExpresionBooleana(string etiqueta, int numEtiqueta)
        {
            bool A = ANDExpresionBooleana(etiqueta, numEtiqueta);
            if (GETContenido() == "||")
            {
                MATCH("||");
                bool B = ANDExpresionBooleana(etiqueta, numEtiqueta);
                return A || B;
            }
            return A;
        }

        private bool ANDExpresionBooleana(string etiqueta, int numEtiqueta)
        {
            bool A = NOTExpresionBooleana(etiqueta, numEtiqueta);
            if (GETContenido() == "&&")
            {
                MATCH("&&");
                bool B = NOTExpresionBooleana(etiqueta, numEtiqueta);
                return A && B;
            }
            return A;
        }

        private bool NOTExpresionBooleana(string etiqueta, int numEtiqueta)
        {
            bool A;
            string operador = GETContenido();
            if (operador == "!")
            {
                MATCH("!");
                A = !NOTExpresionBooleana(etiqueta, numEtiqueta);
            }
            else if (operador == "(")
            {
                MATCH("(");
                A = ExpresionBooleana(etiqueta, numEtiqueta);
                MATCH(")");
            }
            else
            {
                A = ExpresionRelacional(etiqueta, numEtiqueta);
            }
            return A;
        }

        private bool ExpresionRelacional(string etiqueta, int numEtiqueta)
        {
            float A = Expresion();
            Asm.WriteLine("pop rax");
            if (GETClasificacion() == operadorRelacional)
            {
                string operador = GETContenido();
                MATCH(operadorRelacional);
                Asm.WriteLine("push rax");
                float B = Expresion();
                Asm.WriteLine("pop rbx");
                Asm.WriteLine("pop rax");
                AsmEvaluarCondicion(operador, etiqueta, numEtiqueta);
                return EvaluarCondicion(A, operador, B);
            }
            return A != 0;
        }

        public void CicloFor(bool DebeEjecutar)
        {
            int numEtiqueta = contadorFor;
            contadorFor++;

            string tipoIterador;
            string nombreIterador;
            string operador;
            float B;
            string incremento;

            MATCH("for");  // ToDo: Implementar for en ensamblador
            MATCH("(");
            tipoIterador = GETContenido();
            MATCH(tipoDato);
            nombreIterador = GETContenido();
            if (atributos.Existe(nombreIterador))
            {
                throw new AtributoDeclaradoException(renglon, columna, nombreIterador);
            }
            atributos.Agregar(0, Semantica.Atributo.StrToTipo(tipoIterador), nombreIterador);

            MATCH(identificador);
            MATCH(asignacion);
            int valorInicial = (int)Expresion();
            //Asm.WriteLine("mov rax, " + valorInicial);
            //Asm.WriteLine("push rax");
            atributos.SETValor(nombreIterador, valorInicial);
            MATCH(finSentencia);

            // condicion de ejecucion
            Asm.WriteLine("\nbeginCondicionFor" + numEtiqueta + ":");
            string identificadorCondicion = GETContenido();
            MATCH(identificador);
            operador = GETContenido();
            MATCH(operadorRelacional);
            B = Expresion();

            // FixMe: Separar la ejecucion en vivo de la generacion de codigo ensamblador
            if (identificadorCondicion == nombreIterador)
            {
                Asm.WriteLine("pop rbx");
            }
            else
            {
                Asm.WriteLine("mov rbx, [" + GETContenido() + "]");
            }
            Asm.WriteLine("pop rax");
            AsmEvaluarCondicion(operador, "for", numEtiqueta);
            Asm.WriteLine("push rax");
            Asm.WriteLine("jmp beginCuerpoFor" + numEtiqueta);
            Asm.WriteLine("endCondicionFor" + numEtiqueta + ":\n");

            MATCH(finSentencia);

            Asm.WriteLine("beginIncrementoFor" + numEtiqueta + ":");
            MATCH(identificador);
            incremento = GETContenido();
            MATCH(incrementoTermino);
            Asm.WriteLine("pop rax");
            Asm.WriteLine((incremento == "++" ? "inc" : "dec") + " rax");
            Asm.WriteLine("push rax");
            Asm.WriteLine("jmp beginCondicionFor" + numEtiqueta);
            Asm.WriteLine("endIncrementoFor" + numEtiqueta + ":\n");
            MATCH(")");

            Asm.WriteLine("beginCuerpoFor" + numEtiqueta + ":");
            if (GETClasificacion() == inicioBloque)
            {
                MATCH(inicioBloque);
                if (GETClasificacion() == finBloque)
                {
                    MATCH(finBloque);
                    atributos.Eliminar(nombreIterador);
                    return;
                }
                if (GETContenido() == "Console")
                {
                    MATCH("Console");
                    MATCH(".");
                    //AsmConsole(GETContenido());
                    EjecutaFor(DebeEjecutar, incremento == "++" ? 1 : -1,
                               nombreIterador, operador, B);
                    if (GETClasificacion() != finBloque)
                    {
                        ListaInstrucciones(true);
                    }
                }
                else
                {
                    ListaInstrucciones(true);
                }
                MATCH(finBloque);
            }
            else
            {
                if (GETContenido() == "Console")
                {
                    MATCH("Console");
                    MATCH(".");
                    EjecutaFor(DebeEjecutar, incremento == "++" ? 1 : -1,
                               nombreIterador, operador, B);
                }
                else
                {
                    Instruccion(true);
                }
            }
            atributos.Eliminar(nombreIterador);
            Asm.WriteLine("jmp beginIncrementoFor" + numEtiqueta);
            Asm.WriteLine("endCuerpoFor" + numEtiqueta + ":\n");
            //Asm.WriteLine("pop rax");
        }

        private void EjecutaFor(bool DebeEjecutar, int incremento,
                                string nombreIterador, string operador,
                                float tope)
        {
            string caden = "";
            string nombre = "";
            float expr = 0f;
            bool sinArgumento = false;
            string instruccion = GETContenido();
            if (GETContenido() == "WriteLine")
            {
                MATCH("WriteLine");
                MATCH("(");
                if (GETClasificacion() == cadena)
                {
                    caden = GETContenido();
                    AsmWriteString(caden.Substring(1, caden.Length - 2));
                    MATCH(cadena);
                }
                else if (GETClasificacion() == identificador)
                {
                    nombre = GETContenido();
                    // Lanzar excepcion Si el atributo No existe
                    if (!atributos.Existe(nombre))
                    {
                        throw new AtributoNoDeclaradoException(nombre, renglon, columna);
                    }
                    AsmWriteInt(nombre);
                    MATCH(identificador);
                }
                else if (GETContenido() == ")")
                {
                    AsmWriteString("");
                    sinArgumento = true;
                }
                else
                {
                    expr = Expresion();
                }
                MATCH(")");
            }
            else if (GETContenido() == "ReadLine")
            {
                MATCH("ReadLine");
                MATCH("(");
                if (GETContenido() == ")")
                {
                    sinArgumento = true;
                }
                else
                {
                    nombre = GETContenido();
                    if (!atributos.Existe(nombre))
                    {
                        throw new AtributoNoDeclaradoException(nombre, renglon, columna);
                    }
                    MATCH(identificador);
                    MATCH(")");
                    Asm.WriteLine("mov rax, bufferIn");
                    Asm.WriteLine("mov rbx, " + nombre);
                    Asm.WriteLine("call readInt");
                }
            }
            else
            {
                MATCH("ReadKey");
                MATCH("(");
                MATCH(")");
                AsmReadKey();
            }
            MATCH(finSentencia);
            switch (instruccion)
            {
                case "WriteLine":
                    if (DebeEjecutar)
                    {
                        while (true)
                        {
                            if (!EvaluarCondicion(
                                atributos.GETValor(nombreIterador),
                                operador, tope)
                               )
                            {
                                break;
                            }
                            if (sinArgumento)
                            {
                                Console.WriteLine();
                            }
                            // WriteLine recibe un identificador
                            else if (nombre.Length > 0)
                            {
                                Console.WriteLine(atributos.GETValor(nombre));
                            }
                            // WriteLine recibe una cadena
                            else if (caden.Length > 0)
                            {
                                Console.WriteLine(
                                    caden.Substring(1, caden.Length - 2));
                            }
                            // WriteLine recibe una expresion
                            else
                            {
                                Console.WriteLine(expr);
                            }
                            atributos.SETValor(nombreIterador, atributos.GETValor(nombreIterador) + incremento);
                        }
                    }
                    break;
                case "ReadLine":
                    if (DebeEjecutar)
                    {
                        while (true)
                        {
                            if (EvaluarCondicion(
                                atributos.GETValor(nombreIterador),
                                operador, tope)
                               )
                            {
                                break;
                            }
                            if (sinArgumento)
                            {
                                Console.ReadLine();
                            }
                            else
                            {
                                atributos.SETValor(nombre, float.Parse(Console.ReadLine()));
                            }
                            atributos.SETValor(
                                nombreIterador, atributos.GETValor(nombre) + incremento);
                        }
                    }
                    break;
                case "ReadKey":
                    if (DebeEjecutar)
                    {
                        while (true)
                        {
                            if (
                                EvaluarCondicion(
                                atributos.GETValor(nombreIterador),
                                operador, tope)
                               )
                            {
                                break;
                            }
                            Console.ReadKey();
                            atributos.SETValor(nombre, atributos.GETValor(nombre) + incremento);
                        }
                    }
                    break;
            }
        }

        private void AsmEvaluarCondicion(string operador, string etiequeta, int numEtiqueta)
        {
            Asm.WriteLine("cmp rax, rbx");
            switch (operador)
            {
                case "<":
                    Asm.Write("jnl ");
                    AsmSaltosCondicionales(etiequeta, numEtiqueta);
                    break;
                case "<=":
                    Asm.Write("jnle ");
                    AsmSaltosCondicionales(etiequeta, numEtiqueta);
                    break;
                case ">":
                    Asm.Write("jng ");
                    AsmSaltosCondicionales(etiequeta, numEtiqueta);
                    break;
                case ">=":
                    Asm.Write("jng ");
                    AsmSaltosCondicionales(etiequeta, numEtiqueta);
                    break;
                case "==":
                    Asm.Write("jne ");
                    AsmSaltosCondicionales(etiequeta, numEtiqueta);
                    break;
                case "!=":
                    Asm.Write("je ");
                    AsmSaltosCondicionales(etiequeta, numEtiqueta);
                    break;
            }
            //Asm.Write("jmp ");
            //AsmSaltosCondicionales(etiequeta, numEtiqueta);
        }

        private bool EvaluarCondicion(float A, string operador, float B)
        {
            switch (operador)
            {
                case "<":
                    return A < B;
                case "<=":
                    return A <= B;
                case ">":
                    return A > B;
                case ">=":
                    return A >= B;
                case "==":
                    return A == B;
                case "!=":
                    return A != B;
            }
            return false;
        }

        private void AsmWriteString(string cadenaAImprimir) // FixMe: arreglar cadena vacia
        {
            // ToDo: terminar implementacion
            /* 
            Implementacion para Linux x86_64 (64 bits)
            Para imprimir es necesario almacenar el string en memoria.
            En esta implementacion se guarda el string en el stack para pasarla
            como argumento a la subrutina writeString.
            De esta forma la longitud de la cadena no está limitada a la longitud
            de los registros, no es necesario incluir otro archivo (biblioteca),
            ni se necesita declarar un buffer en .data
            */
            cadenaAImprimir += '\n';
            cadenaAImprimir = cadenaAImprimir.Invertir();
            string cadenaAux = "";
            int longitud = cadenaAImprimir.Length;
            int iteraciones = longitud / 8;
            int pushCont = iteraciones;
            if (longitud % 8 != 0)
            {
                cadenaAux = ToAsciiHex(cadenaAImprimir.Substring(0, longitud % 8));
                cadenaAImprimir = cadenaAImprimir.Substring(longitud % 8, longitud - longitud % 8);
                pushCont++;
                Asm.WriteLine("mov rax, 0x" + cadenaAux);
                Asm.WriteLine("push rax");
            }
            for (int i = 0; i < iteraciones; i++)
            {
                cadenaAux = ToAsciiHex(cadenaAImprimir.Substring(i * 8, 8));
                Asm.WriteLine("mov rax, 0x" + cadenaAux);
                Asm.WriteLine("push rax");
            }
            Asm.WriteLine("mov rax, rsp"); // Direccion de la cadena, tope del stack
            Asm.WriteLine("mov rbx, " + longitud);
            Asm.WriteLine("mov rcx, " + pushCont);
            Asm.WriteLine("call writeString");
        }

        private void AsmReadLine()
        {
            Asm.WriteLine("mov rax, bufferIn");
            Asm.WriteLine("call readLine");
        }

        private void AsmInicioPrograma()
        {
            Asm.WriteLine("\nSECTION .TEXT");

            // Punto de entrada. Necesario para el enlazador
            Asm.WriteLine(new String(' ', 4) + "GLOBAL main");
            Asm.WriteLine("main:");
            Asm.WriteLine("mov byte [bufferIn + 255], 0xFF");
        }

        private void AsmFinPrograma()
        {
            // Lamadar al kernel para terminar el programa 
            /*Asm.WriteLine("\nfinPrograma:");
            Asm.WriteLine("mov rax, 60"); // syscall 60 sys_exit
            Asm.WriteLine("mov rdi, 0"); // codigo de salida, equivalente a return 0
            Asm.WriteLine("syscall"); // Llamada al kernel*/
            Asm.WriteLine("call finPrograma");
            //Console.ReadLine()
        }

        private void AsmSaltosCondicionales(string etiqueta, int contador)
        {
            if (etiqueta == "if")
            {
                Asm.WriteLine("endIf" + contador + "\n");
            }
            else
            {
                Asm.WriteLine("endCuerpoFor" + contador + "\n");
            }
        }

        private void AsmWriteInt(string nombre)
        {
            Asm.WriteLine("mov rax, [" + nombre + "]");
            Asm.WriteLine("call ");
        }

        private void AsmReadKey()
        {
            Asm.WriteLine("mov rax, bufferIn");
            Asm.WriteLine("call readKey");
        }

        private void AsmIncludes()
        {
            Asm.WriteLine("extern writeFloat");
            Asm.WriteLine("extern writeChar");
            Asm.WriteLine("extern writeInt");
            Asm.WriteLine("extern writeString");
            Asm.WriteLine("extern readKey");
            Asm.WriteLine("extern readLine");
            Asm.WriteLine("extern readInt");
            Asm.WriteLine("extern finPrograma");
            Asm.WriteLine("extern finProgramaError");
        }

        private string ToAsciiHex(string s)
        {
            return BitConverter.ToString(Encoding.ASCII.GetBytes(s)).Replace("-", "");
        }

        private char ReadChar()
        {
            return Console.ReadKey().KeyChar;
        }

        private int ReadInt()
        {
            string Buffer = "";
            char c;
            int contador = 0;
            while (true)
            {
                c = Console.ReadKey().KeyChar;
                contador++;
                if (!char.IsDigit(c))
                {
                    if (c != '-')
                    {
                        throw new CompiladorException();
                    }
                }
                if (contador > 10) // Overflow del tipo int
                {
                    throw new CompiladorException();
                }
                if (c == '\n')
                {
                    break;
                }
                Buffer += c;
            }
            return int.Parse(Buffer);
        }

        private float ReadFloat()
        {
            return 0f;
        }

        private float ReadLine()
        {
            return float.Parse(Console.ReadLine()); // FixMe
        }
    }
}