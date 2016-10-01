using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoToken
    {
        public int numero = 0;
        public string Línea = "";
        public string nombreToken = "";
        public string tipo = "";
        public int codigo = 0;
        public int codigo_interno;
        public NodoAlfanumerico ligaNodoAlfanumerico;
        public NodoToken sig;
        
    }
}
