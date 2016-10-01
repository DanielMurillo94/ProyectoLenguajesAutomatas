using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class Campo
    {
        public NodoAlfanumerico supuesta_tabla;
        public NodoAlfanumerico supuesto_atributo;
        public string Línea_tabla;
        public string Línea_atributo;
        public Tabla tabla;
        public NodoAtributo atributo;
        public int indice;
        public Campo der, izq;

        public Campo() { }

        public Campo(int i, NodoAlfanumerico sa, string l) 
        {
            indice = i + 1;
            supuesto_atributo = sa;
            Línea_atributo = l;
        }

        public void cambio_supuestos(NodoAlfanumerico sa, string l)
        {
            Línea_tabla = Línea_atributo;
            Línea_atributo = l;
            supuesta_tabla = supuesto_atributo;
            supuesto_atributo = sa;
        }
    }
}
