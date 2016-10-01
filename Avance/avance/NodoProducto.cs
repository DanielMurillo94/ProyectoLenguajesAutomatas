using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoProducto
    {
        public NodoCabezal regla;
        public NodoCabezal terminal;
        public NodoProducto ligaRegla, ligaTerminal;
        public string Producto;

        public NodoProducto()
        { }
    }
}
