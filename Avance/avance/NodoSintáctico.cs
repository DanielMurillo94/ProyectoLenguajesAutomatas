using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{       
    class NodoSintáctico
    {
        public string pila;
        public string X;
        public string K;
        public NodoSintáctico sig;
        public int numero;

        public NodoSintáctico()
        {
            pila = X = K = "";
            numero = 0;
        }
    }
}
