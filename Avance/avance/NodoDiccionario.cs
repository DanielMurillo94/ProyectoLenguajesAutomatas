using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoDiccionario : Nodo
    {
        public string tipo;
        public string lexema;
        public NodoDiccionario liga;

        public NodoDiccionario()
        {}

        public NodoDiccionario(string t, string l, int c)
        {
            tipo = t;
            lexema = l;
            codigo = c;
            izq = der = null;
        }

        public NodoDiccionario(NodoDiccionario nd)
        {
            tipo = nd.tipo;
            lexema = nd.lexema;
            codigo = nd.codigo;
            izq = der = null;
            nivel = nd.nivel;
        }
    }
}
