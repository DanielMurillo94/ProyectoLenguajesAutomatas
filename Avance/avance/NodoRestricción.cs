using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoRestriccion : Nodo
    {
        public int tipo;
        public NodoAtributo ligaNodoAtributo;
        public Tabla ligaTabla;
        public NodoAtributo ligaTablaNodoAtributo;
        public NodoAlfanumerico identificador;
        public string Línea;

        public NodoRestriccion(){}
        
        public NodoRestriccion(int c, NodoAlfanumerico i, string l)
        {
            codigo = c;
            identificador = i;
            Línea = l;
        }

        public NodoRestriccion copia()
        {
            NodoRestriccion nr = new NodoRestriccion();
            nr.tipo = tipo;
            nr.ligaNodoAtributo = ligaNodoAtributo;
            nr.ligaTabla = ligaTabla;
            nr.ligaTablaNodoAtributo = ligaTablaNodoAtributo;
            nr.identificador = identificador;
            nr.Línea = Línea;
            nr.codigo = codigo;
            nr.fb = fb;
            nr.nivel = nivel;
            if (izq != null)
                nr.izq = ((NodoRestriccion)izq).copia();
            if (der != null)
                nr.der = ((NodoRestriccion)der).copia();
            return nr;
        }
    }
}
