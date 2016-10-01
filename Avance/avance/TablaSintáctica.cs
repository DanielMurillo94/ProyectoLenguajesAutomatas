using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class TablaSintáctica : Arbol
    {
        public int Nnodos = 0;

        //raiz1 es para el arbol de reglas
        //raiz2 es para el arbol de columnas

        public TablaSintáctica()
        { }

        public NodoCabezal buscarCabezal(int val, NodoCabezal raiz)
        {
            return (NodoCabezal)buscar(val, raiz);
        }

        public void agregarCabezal(ref NodoCabezal r, int valor)
        {
            NodoCabezal nc = new NodoCabezal(valor);
            Nodo raiz = r;
            insertar(raiz, raiz, nc, ref raiz);
            r = (NodoCabezal)raiz;
        }

        public void agregar(int regla, int terminal, string producto)
        {
            NodoCabezal ncf, ncc;
            ncf = (NodoCabezal)buscar(regla, raiz1);
            ncc = (NodoCabezal)buscar(terminal, raiz2);
            NodoProducto np = new NodoProducto();
            np.regla = ncf;
            np.terminal = ncc;
            np.Producto = producto;
            ncf.agregar(np, true);
            ncc.agregar(np, false);
        }

        public NodoProducto buscar(int regla, int terminal)
        {
            NodoCabezal ncf = (NodoCabezal)buscar(regla, raiz1);
            NodoCabezal ncc = (NodoCabezal)buscar(terminal, raiz2);
            if (ncc != null && ncf != null)
            {
                NodoProducto np = ncc.inicio;
                while (np != null)
                {
                    if (np.regla == ncf)
                        return np;
                    np = np.ligaRegla;
                }
            }
            return null;
        }
    }
}
