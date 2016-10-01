using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoAlfanumerico : Nodo
    {
        public string valor_de_referencia;
        public int tipo;

        public NodoAlfanumerico()
        {}

        public NodoAlfanumerico(string i, int v, int t)
        {
            valor_de_referencia = i;
            codigo = v;
            tipo = t;
        }

        public NodoAlfanumerico copia()
        {
            NodoAlfanumerico na = new NodoAlfanumerico();
            na.valor_de_referencia = valor_de_referencia;
            na.codigo = codigo;
            na.tipo = tipo;
            return na;
        }
    }
}
