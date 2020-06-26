using System;
using System.IO;

namespace Semantica
{
    public class Ejecutable : IDisposable
    {
        string nombreArchivo;
        public Ejecutable()
        {
            nombreArchivo = @"/archivos/prueba.asm";
        }

        public Ejecutable(string nombreArchivo)
        {
            nombreArchivo = Path.ChangeExtension(nombreArchivo, "asm");
        }

        public void Dispose()
        {
        }

        public void CrearEjecutable()
        {
            string cmd = "ensamblar " + nombreArchivo;
            cmd.RunCmd();
            Console.Write("\n¿Quiere ejecutar el programa?\n(S/n): ");
            if (Console.ReadKey().KeyChar == 'n')
            {
                return;
            }
            Console.WriteLine("Ejecucion");
            Console.WriteLine("==============================================");
            cmd = nombreArchivo.Replace(".asm", "");
            cmd.RunCmd();
        }
    }
}