using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoCabezal : Nodo
    {
        public NodoProducto inicio = null;
        public NodoProducto finRegla = null, finTerminal = null;

        public NodoCabezal()
        { }
        
        public NodoCabezal(int val)
        {
            codigo = val;
        }

        public void agregar(NodoProducto np, bool regla)
        {
            if (inicio == null)
                inicio = finRegla = finTerminal = np;
            else
            {
                if (regla)
                {
                    finTerminal.ligaTerminal = np;
                    finTerminal = np;
                }
                else
                {
                    finRegla.ligaRegla = np;
                    finRegla = np;
                }
            }
        }
    }
}
