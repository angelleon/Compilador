using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semantica
{
    public class ListaAtributos
    {
        private Atributo[] Atributos;
        private int contador;
        private int maxElementos;

        public ListaAtributos()
        {
            maxElementos = 10;
            contador = 0;
            Atributos = new Atributo[maxElementos];
        }

        public ListaAtributos(int tamaño)
        {
            maxElementos = tamaño;
            contador = 0;
            Atributos = new Atributo[maxElementos];
        }

        public void Agregar(int zona, int tipo, string nombre)
        {
            if (contador < maxElementos)
            {
                Atributos[contador] = new Atributo();
                Atributos[contador++].SET(zona, tipo, nombre);
            }
            else
            {
                /*
                Lanzar excepcion indicando que no hay espacio
                en el arreglo de atributos
                */
                /*
                try
                {
                */
                throw new DesbordamientoListaAtributosException(
                    Atributo.ZonaToStr(zona), Atributo.TipoToStr(tipo),
                    nombre, maxElementos);
                /*
                }
                catch (DesbordamientoListaAtributosException ex)
                {
                    ImprimirError(ex);
                }
                */
            }
        }

        public void Eliminar(string nombre)
        {
            for (int i = 0; i < contador; i++)
            {
                if (Atributos[i].GETNombre() == nombre)
                {
                    Atributos[i] = null;
                    contador--;
                }
            }
        }

        public bool Existe(string nombre)
        {
            for (int i = 0; i < contador; i++)
            {
                if (Atributos[i].GETNombre() == nombre) return true;
            }
            return false;
        }

        public float GETValor(string nombre)
        {
            for (int i = 0; i < contador; i++)
            {
                if (Atributos[i].GETNombre() == nombre) return Atributos[i].GETValor();
            }
            return 0f;
        }

        public int GETTipo(string nombre)
        {
            for (int i = 0; i < contador; i++)
            {
                if (Atributos[i].GETNombre() == nombre)
                {
                    return Atributos[i].GETTipoDato();
                }
            }
            return 0;
        }

        public string GETTipoStr(string nombre)
        {
            for (int i = 0; i < contador; i++)
            {
                if (Atributos[i].GETNombre() == nombre)
                {
                    return Atributos[i].GETTipoDatoStr();
                }
            }
            return "";
        }

        public void SETValor(string nombre, float valor)
        {
            for (int i = 0; i < contador; i++)
            {
                if (Atributos[i].GETNombre() == nombre) Atributos[i].SETValor(valor);
            }
        }
        public void Desplegar()
        {
            for (int i = 0; i < contador; i++)
            {
                Console.WriteLine(ToStr());
            }
        }

        public string ToStr()
        {
            string listaStr = "";
            for (int i = 0; i < contador; i++)
            {
                listaStr += "-------------------------------------------\n";
                listaStr += Atributos[i].GETZonaStr() + "\n";
                listaStr += Atributos[i].GETTipoDatoStr() + "\n";
                listaStr += Atributos[i].GETNombre() + "\n";
                listaStr += Atributos[i].GETValor() + "\n";
            }
            return listaStr;
        }
    }
}