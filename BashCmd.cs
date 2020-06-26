using System;
using System.Diagnostics;

namespace Semantica
{
    public static class BashCmd
    {
        public static void RunCmd(this string cmd)
        {
            var argumentosEscapados = cmd.Replace("\"", "\\\"");
            var proceso = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash", // se ejecuta bash con los argumentos especificados en el string
                    // convierte al string "foo" en la llamada a shell "bash -c \"foo\""
                    // donde la opción -c indica a bash (shell por defecto en muchos sistemas *nix)
                    // que se ejecute el comando subsecuente
                    // el equivalente en windows sería una llamada a cmd o powershell
                    Arguments = $"-c \"{argumentosEscapados}\"",
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            proceso.Start();
            proceso.WaitForExit();
        }
    }
}