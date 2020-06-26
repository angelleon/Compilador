using System;
using System.IO;

namespace Semantica
{
    public class ErrorSintactico : CompiladorException
    {
        public ErrorSintactico()
        {
        }
        public ErrorSintactico(string esperado, string Buffer, int renglon, int columna) :
            base("Error sintactico, se esperaba " + esperado + "\n" +
                 "Se encontró: (" + Buffer + ")\n" +
                 "Renglón: " + renglon + ", Columna: " + columna)
        {
        }
    }
}