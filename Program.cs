using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Semantica
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Lenguaje l = args.Length > 0 ? new Lenguaje(args[0]) : new Lenguaje())
            // using (Lenguaje l = new Lenguaje(@"/archivos/prueba.cs")) // Llama al método Dispose al finalizar el bloque
            {
                l.Programa();
            }
            using (Ejecutable e = args.Length > 0 ? new Ejecutable(args[0]) : new Ejecutable())
            {
                e.CrearEjecutable();
            }
            Console.ReadKey();
        }
    }
}
