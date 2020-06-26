using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis : Lexico, IDisposable
    {
        // parte de la gramática que define la secuencia de tokens
        // oracion := sujeto  predicado
        // sujeto := art sust adj?
        // predicado := verb adv? sujeto

        // statment := asignacion
        // asignacion := ident = expres
        public Sintaxis() : base()
        {
            NetxToken();
        }
        public Sintaxis(string ruta) : base(ruta)
        {
            NetxToken();
        }
        public new void Dispose()
        {
            base.Dispose();
            Console.WriteLine("Fin de analisis sintactico.");
        }

        public void MATCH(int esperado)
        {
            //Console.WriteLine(SAYClasificacion(GETClasificacion()) + " " + SAYClasificacion(esperado));
            //Console.ReadKey();
            if (GETClasificacion() == esperado)
            {
                NetxToken();
            }
            else
            {
                /*
                try
                {
                */
                throw new ErrorSintactico(SAYClasificacion(esperado), SAYClasificacion(GETClasificacion()), renglon, columna);
                //Console.WriteLine("Error de sintaxis: se espera un {0} . Se encontró un: {1}", esperado, GETContenido());
                /*
                }
                catch (ErrorSintactico ESint)
                {
                    Console.WriteLine(ESint.Message);
                    Console.WriteLine(GETContenido());
                    Environment.Exit(1);
                    //throw ESint;
                }
                */
            }
        }

        public void MATCH(string esperado)
        {
            //Console.WriteLine(GETContenido() + " " + esperado);
            //Console.ReadKey();
            if (GETContenido() == esperado)
            {
                NetxToken();
            }
            else
            {
                /*
                try
                {
                */
                throw new ErrorSintactico(esperado, GETContenido(), renglon, columna);
                //Console.WriteLine("Error de sintaxis: se espera un {0} . Se encontró un: {1}", esperado, GETContenido());
                /*
                }
                catch (ErrorSintactico ESint)
                {
                    Console.WriteLine(ESint.Message);
                    Console.WriteLine(GETContenido());
                    Environment.Exit(1);
                    //throw ESint;
                }
                */
            }
        }

    }
}