using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class Alias
    {
        public NodoAlfanumerico nuevo_nombre;
        public Tabla tabla;
        public Alias sig;

        public Alias() { }

        public Alias(NodoAlfanumerico new_name, Tabla t)
        {
            nuevo_nombre = new_name;
            tabla = t;
        }
    }
}