using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class Tabla : Arbol
    {
        public Tabla izq;
        public Tabla der;
        public int fb;
        public int nivel;
        public int codigo;
        public int num_tabla;
        public NodoAlfanumerico identificador;
        public string Línea;
        public const int tipo = 1;
        public Tabla ligaIzq, ligaDer;
        public int Count = 0;

        public NodoAlfanumerico nuevo_nombre;
        public NodoAtributo inicio, fin;
        public Tabla sig;

        //raiz1 = raizAtributos
        //raiz2 = raizRestriccion

        public Tabla(){}

        public void ins_nat_Lista(NodoAtributo nat)
        {
            if (inicio == null)
            {
                nat.contador = 1;
                inicio = fin = nat;
            }
            else
            {
                nat.contador = fin.contador + 1;
                fin.sig = nat;
                fin = nat;
            }
            Count = fin.contador;
        }

        public Tabla copia()
        {
            Tabla tab = new Tabla();
            tab.codigo = codigo;
            tab.identificador = identificador;
            tab.raiz1 = raiz1;
            tab.raiz2 = raiz2;
            tab.Count = Count;
            return tab;
        }

        public Tabla(int c, NodoAlfanumerico i, string l, List<string> e)
        {
            codigo = c;
            identificador = i;
            Línea = l;
            errores = e;
        }

        public NodoRestriccion buscarRestricción(int codigo)
        {
            return (NodoRestriccion)buscar(codigo, raiz2);
        }

        public NodoAtributo buscarAtributo(int codigo)
        {
            return (NodoAtributo)buscar(codigo, raiz1);
        }

        public void agregar(ref Nodo raiz, Nodo n)
        {
            insertar(raiz, raiz, n, ref raiz);
        }

        public bool contiene(int codigo, Nodo R)
        {
            return buscar(codigo, R) != null;
        }

        public NodoRestriccion buscarRestricciónPrimaria(NodoRestriccion p)
        {
            NodoRestriccion q = null;
            if (p != null)
            {
                if (p.tipo == 3)
                    return p;
                q = buscarRestricciónPrimaria((NodoRestriccion)p.izq);
                if (q != null)
                    return q;
                q = buscarRestricciónPrimaria((NodoRestriccion)p.der);
            }
            return q;
        }
    }
}