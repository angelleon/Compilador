namespace Semantica
{
    public class FormatoNoSoportadoException : CompiladorException
    {
        public FormatoNoSoportadoException() { }
        public FormatoNoSoportadoException(string nombreArchivo) :
        base(nombreArchivo + "\n" +
             "¡Archivo no compatible!\n" +
             "Especifique un archivo fuente .cs"
        )
        { }
    }
}