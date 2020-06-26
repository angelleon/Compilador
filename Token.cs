using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semantica
{
    public class Token
    {
        private string contenido;
        private int clasificacion;
        protected const int identificador = 0; //L(L|D)*
        protected const int numero = 1; //D+(.D+)?(E(+|-)?D+)?
        protected const int caracter = 2; //2 // #|.|(|)|...
        protected const int asignacion = 3; //4 =
        protected const int finSentencia = 4; //3
        protected const int operadorTermino = 5;
        protected const int operadorFactor = 6;
        protected const int inicioBloque = 7;
        protected const int finBloque = 8;
        protected const int operadorLogico = 9; // (&&) | (||)
        protected const int operadorRelacional = 10; //  >(=)? | <(=)? | == | !=
        protected const int flujo = 11; // >> | <<
        protected const int incrementoTermino = 12; // += | -=
        protected const int incrementoFactor = 13; // *= | /= | %=
        protected const int cadena = 14; //"C*"
        protected const int zona = 15;
        protected const int tipoDato = 16;

        public void SETContenido(string c)
        {
            contenido = c;
        }

        public void SETClasificacion(int c)
        {
            clasificacion = c;
        }

        public string GETContenido()
        {
            return contenido;
        }

        public int GETClasificacion()
        {
            return clasificacion;
        }

        public string SAYClasificacion(int clasificacion)
        {
            switch (clasificacion)
            {
                case identificador:
                    return "identificador";
                case numero:
                    return "número";
                case caracter:
                    return "caracter";
                case asignacion:
                    return "asignación";
                case finSentencia:
                    return "fin de sentencia";
                case operadorTermino:
                    return "operador de termino";
                case operadorFactor:
                    return "operador de factor";
                case inicioBloque:
                    return "inicio de bloque";
                case finBloque:
                    return "fin de bloque";
                case operadorLogico:
                    return "operador lógico";
                case operadorRelacional:
                    return "operador relacional";
                case flujo:
                    return "operador de flujo";
                case incrementoTermino:
                    return "operador de incremento de termino";
                case incrementoFactor:
                    return "operador de incremnto de factor";
                case cadena:
                    return "cadena";
                case zona:
                    return "zona de accesibilidad";
                case tipoDato:
                    return "tipo de dato";
                default:
                    return "sin clasificación";
            }
        }
    }
}
