namespace Semantica
{
    public class AgotamientoPilaException : CompiladorException
    {
        public AgotamientoPilaException() :
            this("¡No se pudo efectuar POP!\n" +
                 "No quedan elementos en el stack")
        { }

        public AgotamientoPilaException(string message) : base(message) { }
    }
}
