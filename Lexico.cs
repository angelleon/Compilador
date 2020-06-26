using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Semantica
{
    public class Lexico : Token, IDisposable  // Interfaz para el metodo Dispose que es llamado por el GC
    {
        private StreamReader Archivo;
        protected StreamWriter Log;
        protected StreamWriter Asm;

        private const int f = -1;
        private const int e = -2;

        private int[,] TRAND = new int[,] {
        // arreglar comentario multilinea
        //    0   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22
        //   WS   L   D   .   E   +   -  LA   =   ;   %   {   }   &   |   !   >   <   "   C   /   *  \n
            { 0,  1,  2, 10,  1, 13, 14,  8, 11, 12, 16, 18, 19, 20, 21, 23, 24, 25, 28, 10, 30, 34,  0}, // 0
            { f,  1,  1,  f,  1,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 1
            { f,  f,  2,  3,  5,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 2
            { e,  e,  4,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e}, // 3
            { f,  f,  4,  f,  5,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 4
            { e,  e,  7,  e,  e,  6,  6,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e}, // 5
            { e,  e,  7,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e}, // 6
            { f,  f,  7,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 7
            { 9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9}, // 8
            { e,  e,  e,  e,  e,  e,  e, 10,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e}, // 9
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 10
            { f,  f,  f,  f,  f,  f,  f,  f, 27,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 11
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 12
            { f,  f,  f,  f,  f, 15,  f,  f, 15,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 13
            { f,  f,  f,  f,  f,  f, 15,  f, 15,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 14
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 15
            { f,  f,  f,  f,  f,  f,  f,  f, 17,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 16
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 17
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 18
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 19
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f, 22,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 20
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f, 22,  f,  f,  f,  f,  f,  f,  f,  f}, // 21
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 22
            { f,  f,  f,  f,  f,  f,  f,  f, 27,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 23
            { f,  f,  f,  f,  f,  f,  f,  f, 27,  f,  f,  f,  f,  f,  f,  f, 26,  f,  f,  f,  f,  f,  f}, // 24
            { f,  f,  f,  f,  f,  f,  f,  f, 27,  f,  f,  f,  f,  f,  f,  f,  f, 26,  f,  f,  f,  f,  f}, // 25
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 26
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 27
            {28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 29, 28, 28, 28, 28}, // 28
            { f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}, // 29
            { f,  f,  f,  f,  f,  f,  f,  f, 17,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f, 31, 32,  f}, // 30
            {31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31, 31,  0}, // 31
            {32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 33, 32}, // 32
            {32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,  0, 33, 32}, // 33
            { f,  f,  f,  f,  f,  f,  f,  f, 17,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f}  // 34
        //   WS   L   D   .   E   +   -  LA   =   ;   %   {   }   &   |   !   >   <   "   C   /   *  \n
        //    0   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22
        };

        protected int renglon;
        protected int columna;

        public Lexico()
        {
            Archivo = new StreamReader(@"/archivos/prueba.cs");  //Debe existir
            Log = new StreamWriter(@"/archivos/prueba.log");  //Lo crea si no existe, si no, lo sobreescribe
            Asm = new StreamWriter(@"/archivos/prueba.asm");
            Cabecera(@"/archivos/prueba.cs");
            columna = 1;
            renglon = 1;
        }

        public Lexico(string nombre)
        {
            /*
                validar parametro con extensión .cs, de lo contrario generar excepción critica
                archivo log tenga el mismo nombre /ruta/nombre.log
                ejemplo: suma.cs suma.log
            */
            Log = new StreamWriter(Path.ChangeExtension(nombre, "log"));
            Asm = new StreamWriter(Path.ChangeExtension(nombre, "asm"));
            Cabecera(nombre);
            try
            {
                if (Path.GetExtension(nombre).ToLower() != ".cs")
                {
                    throw new FormatoNoSoportadoException(nombre);
                }
                Archivo = new StreamReader(nombre);
                Archivo.Peek();
            }
            catch (UnauthorizedAccessException)
            {
                ImprimirError(new CompiladorException(nombre + "\n¡No se puede leer el archivo!"));
            }
            catch (FileNotFoundException)
            {
                ImprimirError(new CompiladorException(nombre + "\n¡No existe el archivo!"));
            }
            catch (CompiladorException ex)
            {
                ImprimirError(ex);
            }
            renglon = 1;
            columna = 1;
        }

        ~Lexico()
        {
            Dispose();
        }

        public void Dispose()
        {
            Archivo.Close();
            Log.Close();
            Asm.Close();
            Console.WriteLine("Fin de analisis léxico");
        }

        private int Columna(char transicion)
        {
            if (transicion == '\n')
            {
                return 22;
            }
            else if (char.IsWhiteSpace(transicion))
            {
                return 0;
            }
            else if (char.ToUpper(transicion) == 'E')// && GETClasificacion() == numero)
            {
                return 4;
            }
            else if (char.IsLetter(transicion))
            {
                return 1;
            }
            else if (char.IsDigit(transicion))
            {
                return 2;
            }
            else if (transicion == '.')
            {
                return 3;
            }
            else if (transicion == '+')
            {
                return 5;
            }
            else if (transicion == '-')
            {
                return 6;
            }
            else if (transicion == '"')
            {
                return 18;
            }
            else if (transicion == '\'')
            {
                return 7;
            }
            else if (transicion == '=')
            {
                return 8;
            }
            else if (transicion == ';')
            {
                return 9;
            }
            else if (transicion == '%')
            {
                return 10;
            }
            else if (transicion == '{')
            {
                return 11;
            }
            else if (transicion == '}')
            {
                return 12;
            }
            else if (transicion == '&')
            {
                return 13;
            }
            else if (transicion == '|')
            {
                return 14;
            }
            else if (transicion == '!')
            {
                return 15;
            }
            else if (transicion == '>')
            {
                return 16;
            }
            else if (transicion == '<')
            {
                return 17;
            }
            else if (transicion == '/')
            {
                return 20;
            }
            else if (transicion == '*')
            {
                return 21;
            }
            else
            {
                return 19;
            }
        }

        private int automata(int estado, char transicion)
        {
            int nuevoEstado = TRAND[estado, Columna(transicion)];
            switch (nuevoEstado)
            {
                case 1:
                    SETClasificacion(identificador);
                    break;
                case 2:
                    SETClasificacion(numero);
                    break;
                case 11:
                    SETClasificacion(asignacion);
                    break;
                case 12:
                    SETClasificacion(finSentencia);
                    break;
                case 13:
                    SETClasificacion(operadorTermino);
                    break;
                case 14:
                    SETClasificacion(operadorTermino);
                    break;
                case 15:
                    SETClasificacion(incrementoTermino);
                    break;
                case 16:
                    SETClasificacion(operadorFactor);
                    break;
                case 17:
                    SETClasificacion(incrementoFactor);
                    break;
                case 18:
                    SETClasificacion(inicioBloque);
                    break;
                case 19:
                    SETClasificacion(finBloque);
                    break;
                case 22:
                    SETClasificacion(operadorLogico);
                    break;
                case 23:
                    SETClasificacion(operadorLogico);
                    break;
                case 24:
                    SETClasificacion(operadorRelacional);
                    break;
                case 25:
                    SETClasificacion(operadorRelacional);
                    break;
                case 26:
                    SETClasificacion(flujo);
                    break;
                case 27:
                    SETClasificacion(operadorRelacional);
                    break;
                case 28:
                    SETClasificacion(cadena);
                    break;
                case 8:
                case 10:
                case 20:
                case 21:
                    SETClasificacion(caracter);
                    break;
                case 30:
                case 34:
                    SETClasificacion(operadorFactor);
                    break;
            }
            return nuevoEstado;
        }

        public void NetxToken()
        {
            char transicion;
            string Buffer = "";
            int estado = 0;
            try
            {
                while (estado >= 0)
                {
                    transicion = (char)Archivo.Peek(); // Lee, no avanza
                    estado = automata(estado, transicion);
                    if (Archivo.EndOfStream)
                    {
                        break;
                    }
                    if (estado > 0 && estado < 31 || estado == 34)
                    {
                        Buffer += transicion;
                    }
                    else if (estado == 31 || estado == 32)
                    {
                        Buffer = "";
                    }
                    if (estado >= 0)
                    {
                        columna++;
                        if (transicion == '\n')
                        {
                            renglon++;
                            columna = 1;
                        }
                        Archivo.Read(); // Lee y avanza en el archivo, se pierde al no asignar
                    }
                }
                if (estado == e)
                {
                    throw new ErrorLexico(SAYClasificacion(GETClasificacion()), Buffer, renglon, columna);
                }
                else
                {
                    SETContenido(Buffer);
                    if (GETClasificacion() == identificador)
                    {
                        if (Buffer == "private" || Buffer == "protected" || Buffer == "public")
                        {
                            SETClasificacion(zona);
                        }
                        else if (Buffer == "char" || Buffer == "int" || Buffer == "float")
                        {
                            SETClasificacion(tipoDato);
                        }
                    }
                }
            }
            catch (ErrorLexico ELexico)
            {
                SETContenido("");
                SETClasificacion(-1);
                ImprimirError(ELexico);
            }
        }

        protected void ImprimirError(CompiladorException ex)
        {
            ConsoleColor ColorActual = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.ForegroundColor = ColorActual;
            Log.WriteLine(ex.Message);
            Log.Close();
            Asm.Close();
            Environment.Exit(1);
        }

        private void Cabecera(string nombre)
        {
            CultureInfo cultura = new CultureInfo("es-MX");
            Log.WriteLine("Programador:\tAngel Leon <user@domain>");
            Log.WriteLine("Compilando:\t" + nombre);
            Log.WriteLine("Fecha:\t\t" + DateTime.Now.ToString("ddd dd MMM yyyy HH:mm:ss", cultura));

            Asm.WriteLine("; Programador:\tAngel Leon <user@domain>");
            Asm.WriteLine("; Compilando:\t" + nombre);
            Asm.WriteLine("; Fecha:\t" + DateTime.Now.ToString("ddd dd MMM yyyy HH:mm:ss", cultura)); // escribir fecha de compilacion al archivo
            /* 
            Asm.WriteLine("org 100h");
            Asm.WriteLine(".MODEL small");
            Asm.WriteLine(".STACK 64");
            Asm.WriteLine(".DATA");            
            */
            Asm.WriteLine();
        }
    }
}