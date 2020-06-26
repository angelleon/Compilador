using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semantica
{
    public class StackEvalua
    {
        private float[] elementos;
        private int maxElemntos;
        private int ultimo;

        public StackEvalua()
        {
            maxElemntos = 3;
            ultimo = 0;
            elementos = new float[maxElemntos];
        }
        public StackEvalua(int size)
        {
            maxElemntos = size;
            ultimo = 0;
            elementos = new float[maxElemntos];
        }

        public void INIT()
        {
            ultimo = 0;
        }
        public void PUSH(float valor)
        {
            if (ultimo < maxElemntos)
            {
                elementos[ultimo++] = valor;
            }
            else
            {
                // lanzar excepcion indicando StackOverflow
                /*try
                {*/
                throw new DesbordamientoPilaException(maxElemntos, valor);
                //throw new Exception("");
                /*}
                catch (DesbordamientoPilaException ex)
                {
                    ImprimirError(ex);
                }
                */
            }
        }

        public float POP()
        {
            if (ultimo > 0)
            {
                return elementos[--ultimo];
            }
            else
            {
                throw new AgotamientoPilaException();
            }
        }

        public void DISPLAY()
        {
            Console.Write("Stack " + "elementos " + ultimo + ": ");
            for (int i = 0; i < ultimo; i++)
            {
                Console.Write(" " + elementos[i]);
            }
            Console.WriteLine();
        }

        /*
        public void ImprimirError(CompiladorException ex)
        {
            ConsoleColor ColorActual = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("==================================================================");
            Console.WriteLine("Error:");
            Console.WriteLine(ex.Message);
            Console.WriteLine("==================================================================");
            Console.WriteLine();
            Console.ForegroundColor = ColorActual;
        }
        */
    }
}