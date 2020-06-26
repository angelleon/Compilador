namespace Semantica
{
    public class DesbordamientoListaAtributosException : CompiladorException
    {
        public DesbordamientoListaAtributosException() { }
        public DesbordamientoListaAtributosException(string message) : base(message) { }
        public DesbordamientoListaAtributosException(string zona, string tipo, string nombre, int maxElementos) :
            this("No queda espacio en la lista de atributos\n" +
                 "No se ha podido agregar\n" +
                 zona + " " + tipo + " " + nombre + "\n" +
                 "Se alcanzó el máximo de " + maxElementos + " atributos en la lista"
            )
        {
        }
    }
}