using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class ArbolAlfanumerico : Arbol
    {
        public int codigoInterno;

        public ArbolAlfanumerico()
        { }

        public ArbolAlfanumerico(int c)
        {
            codigoInterno = c;
        }

        public NodoAlfanumerico insertar(string cadena, int tipo)
        {
            cadena = cadena.ToUpper();
            NodoAlfanumerico na = buscar(cadena, tipo, (NodoAlfanumerico)raiz1);
            if (na == null)
            {
                codigoInterno++;
                na = new NodoAlfanumerico(cadena, codigoInterno, tipo);
                insertar(raiz1, raiz1, na, ref raiz1);
            }
            return na;
        }

        public NodoAlfanumerico buscar(string s, int tipo, NodoAlfanumerico p)
        {
            NodoAlfanumerico q;
            if (p != null)
            {
                if (p.valor_de_referencia == s && p.tipo == tipo)
                    return p;
                q = buscar(s, tipo, (NodoAlfanumerico)p.izq);
                if (q != null)
                    return q;
                q = buscar(s, tipo, (NodoAlfanumerico)p.der);
                if (q != null)
                    return q;
            }
            return null;
        }
        /*Cambios 1*/

        public NodoAlfanumerico buscar(int código)
        {
            return (NodoAlfanumerico)buscar(código, raiz1);
        }
    }
}