namespace Semantica
{
    public class AtributoDeclaradoException : CompiladorException
    {
        public AtributoDeclaradoException() { }

        public AtributoDeclaradoException(string message) : base(message)
        {
        }

        public AtributoDeclaradoException(int renglon, int columna, string nombre) :
        this("El atributo (" + nombre + ") ya fue declarado.\n" +
             "Renglón: " + renglon + ", Columna: " + columna + "\n")
        {
        }
    }
}