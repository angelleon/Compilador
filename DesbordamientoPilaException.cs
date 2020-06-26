namespace Semantica
{
    public class DesbordamientoPilaException : CompiladorException
    {
        public DesbordamientoPilaException() { }
        public DesbordamientoPilaException(string message) : base(message) { }

        public DesbordamientoPilaException(int maxElementos, float valor) :
            this("¡No fue posible efectuar PUSH(" + valor + ")!\n" +
                 "Se alcanzó el máximo de " + maxElementos +  " elementos en el Stack\n")
        { }
    }
}