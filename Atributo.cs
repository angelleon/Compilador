using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semantica
{
    public class Atributo
    {
        private string nombre;
        private int tipoDato; // 0 es char, 1 es int, 2 es float
        private float valor;
        private int zona; // 0 es private, 1 es protected, 2 es public

        public string GETNombre()
        {
            return nombre;
        }

        public int GETTipoDato()
        {
            return tipoDato;
        }

        public float GETValor()
        {
            return valor;
        }

        public int GETZona()
        {
            return zona;
        }

        public void SET(int zona, int tipoDato, string nombre)
        {
            valor = 0;
            this.zona = zona;
            this.tipoDato = tipoDato;
            this.nombre = nombre;
        }

        public void SETValor(float valor)
        {
            this.valor = valor;
        }

        public string GETZonaStr()
        {
            if (zona == 0)
            {
                return "private";
            }
            else if (zona == 1)
            {
                return "protected";
            }
            else
            {
                return "public";
            }
        }

        public string GETTipoDatoStr()
        {
            if (tipoDato == 0)
            {
                return "char";
            }
            else if (tipoDato == 1)
            {
                return "int";
            }
            else
            {
                return "float";
            }
        }

        public static string TipoToStr(int tipoDato)
        {
            if (tipoDato == 0)
            {
                return "char";
            }
            else if (tipoDato == 1)
            {
                return "int";
            }
            else
            {
                return "float";
            }
        }
        public static string ZonaToStr(int zona)
        {
            if (zona == 0)
            {
                return "private";
            }
            else if (zona == 1)
            {
                return "protected";
            }
            else
            {
                return "public";
            }
        }

        public static int StrToTipo(string tipo)
        {
            switch (tipo)
            {
                case "char":
                    return 0;
                case "int":
                    return 1;
                case "float":
                    return 2;
            }
            return -1;
        }
    }
}