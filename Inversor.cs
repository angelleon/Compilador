using System;

namespace Semantica
{
    public static class Inversor
    {
        public static string Invertir(this string s)
        {
            char[] chArr = s.ToCharArray();
            Array.Reverse(chArr);
            return new String(chArr);
        }
    }
}