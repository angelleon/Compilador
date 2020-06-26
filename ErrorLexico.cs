using System;
using System.IO;

namespace Semantica
{
    public class ErrorLexico : CompiladorException
    {
        public ErrorLexico() : base()
        {
        }
        public ErrorLexico(string clasificacion, string Buffer) :
            base("Error léxico, se esperaba " + clasificacion + "\n" +
                 "Se encontró: (" + Buffer + ")")
        {
        }

        public ErrorLexico(string clasificacion, string Buffer, int renglon, int columna) :
            base("Error léxico, se esperaba " + clasificacion + "\n" +
                 "Se encontró: (" + Buffer + ")\n" +
                 "Renglón: " + renglon + ", Columna: " + columna)
        {
        }
    }
}