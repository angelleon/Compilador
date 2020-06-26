namespace Semantica
{
    public class AtributoNoDeclaradoException : CompiladorException
    {
        public AtributoNoDeclaradoException() { }
        public AtributoNoDeclaradoException(string message) : base(message) { }
        public AtributoNoDeclaradoException(string nombre, int renglon, int columna) :
            this("Se intenta accesar al atributo (" + nombre + ") que aún no ha sido declarado \n" +
                 "Renglón: " + renglon + ", Columna: " + columna + "\n")
        { }
    }
}