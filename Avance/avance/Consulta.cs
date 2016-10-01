using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class Consulta
    {
        public Subconsulta inicio, fin;
        public List<string> errores;

        public Consulta() { }

        public void agregar(List<string> e, ArbolTablas arbt)
        {
            if (inicio == null)
            {
                Subconsulta sub = new Subconsulta(-1, e, arbt);
                inicio = fin = sub;
            }
            else
            {
                Subconsulta sub = new Subconsulta(fin.nivel, e, arbt);
                fin.der = sub;
                sub.izq = fin;
                fin = sub;
            }
        }

        public void eliminarultimo()
        {
            if (inicio != fin)
            {
                fin = fin.izq;
                fin.der.izq = null;
                fin.der = null;
            }
            else
            {
                inicio = fin = null;
            }
        }

    }
}
