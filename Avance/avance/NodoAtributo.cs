using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class NodoAtributo : Nodo
    {
        public const int tipo = 2;
        public int tipoDato;
        public NodoAlfanumerico longitud_1;
        public NodoAlfanumerico longitud_2;
        public NodoAlfanumerico identificador;
        public bool noNulo, irrepetible;
        public string Línea;
        public int contador;

        public Tabla tabla_origen;

        public NodoAtributo sig;

        public NodoAtributo enlace;

        private Nodo q, r, aux;
        public NodoAlfanumerico raiz;
        public List<string> errores;

        public void agregarElemento(NodoAlfanumerico na)
        {
            insertar(raiz, raiz, na);
        }

        protected void insertar(Nodo p, Nodo ant, NodoAlfanumerico datos)
        {
            int fbx = 0, fby = 0;
            if (p == null)
            {
                datos.nivel = 0;
                raiz = datos;
                aux = r = raiz;
            }
            else
            {
                if (datos.codigo < p.codigo)
                {
                    if (p.izq == null)
                    {
                        datos.nivel = p.nivel + 1;
                        q = datos;
                        p.izq = q;
                        r = raiz;
                    }
                    else
                        insertar(p.izq, p, datos);
                }
                else
                {
                    if (datos.codigo > p.codigo)
                    {
                        if (p.der == null)
                        {
                            datos.nivel = p.nivel + 1;
                            q = datos;
                            p.der = q;
                            r = raiz;
                        }
                        else
                            insertar(p.der, p, datos);
                    }
                }
                if (ant != null)
                {
                    fbx = fby = 0;
                    NivelMax(p.izq, ref fbx);
                    if (fbx == 0)
                        fbx = p.nivel;
                    NivelMax(p.der, ref fby);
                    if (fby == 0)
                        fby = p.nivel;
                    p.fb = fbx - fby;
                    bool era = false, der = false;
                    if (p.fb > 1 || p.fb < -1)
                    {
                        era = p == raiz;
                        der = ant.der == p;
                        if (p.fb > 0)
                            if (p.izq.fb > 0)
                                Rder(ref p);
                            else
                                DRder(ref p);
                        else
                            if (p.der.fb > 0)
                                DRizq(ref p);
                            else
                                Rizq(ref p);
                        if (!era)
                            if (der)
                                ant.der = p;
                            else
                                ant.izq = p;
                        else
                            raiz = (NodoAlfanumerico)p;
                        Nivel(ref raiz, 0);
                    }
                }
            }
        }

        protected void Nivel(ref NodoAlfanumerico p, int cont)
        {
            if (p != null)
            {
                p.nivel = cont;
                NodoAlfanumerico na = (NodoAlfanumerico)p.izq;
                Nivel(ref na, cont + 1);
                na = (NodoAlfanumerico)p.der;
                Nivel(ref na, cont + 1);
            }
        }

        protected void DRder(ref Nodo p)
        {
            Nodo q = p.izq, r = q.der;
            q.der = r.izq;
            r.izq = q;
            p.izq = r.der;
            r.der = p;
            p = r;
            int niv = p.nivel;
            p.nivel = p.der.nivel;
            p.der.nivel = niv;
        }

        protected void DRizq(ref Nodo p)
        {
            Nodo q = p.der, r = q.izq;
            q.izq = r.der;
            r.der = q;
            p.der = r.izq;
            r.izq = p;
            p = r;
            int niv = p.nivel;
            p.nivel = p.izq.nivel;
            p.izq.nivel = niv;
        }

        protected void Rder(ref Nodo p)
        {
            Nodo q = p.izq;
            p.izq = q.der;
            q.der = p;
            p = q;
            int niv = p.nivel;
            p.nivel = q.der.nivel;
            q.der.nivel = niv;
        }

        protected void Rizq(ref Nodo p)
        {
            Nodo q = p.der;
            p.der = q.izq;
            q.izq = p;
            p = q;
            int niv = p.nivel;
            p.nivel = q.izq.nivel;
            q.izq.nivel = niv;
        }

        protected void NivelMax(Nodo p, ref int x)
        {
            if (p != null)
            {
                if (x < p.nivel)
                    x = p.nivel;
                if (p.izq != null)
                    NivelMax(p.izq, ref x);
                if (p.der != null)
                    NivelMax(p.der, ref x);
            }
        }

        public void agregar(Nodo fuente, ref NodoAtributo destino)
        {
            if (fuente != null)
            {
                destino.agregarElemento((NodoAlfanumerico)fuente);
                agregar(fuente.izq, ref destino);
                agregar(fuente.der, ref destino);
            }
        }

        public NodoAlfanumerico buscar(int codigo)
        {
            Nodo r = raiz;
            if (r != null)
            {
                while (r != null && r.codigo != codigo)
                {
                    if (codigo < r.codigo)
                        r = r.izq;
                    if (r == null)
                        return null;
                    if (r.codigo < codigo)
                        r = r.der;
                }
                return (NodoAlfanumerico)r;
            }
            return null;
        }

        public NodoAtributo() { }

        public NodoAtributo(int c, NodoAlfanumerico i, string l, List<string> e)
        {
            codigo = c;
            identificador = i;
            Línea = l;
            errores = e;
        }

        public NodoAtributo copia()
        {
            NodoAtributo nat = new NodoAtributo();
            nat.tipoDato = tipoDato;
            nat.longitud_1 = longitud_1;
            nat.longitud_2 = longitud_2;
            nat.noNulo = noNulo;
            nat.identificador = identificador;
            nat.Línea = Línea;
            nat.codigo = codigo;
            nat.fb = fb;
            nat.nivel = nivel;
            nat.irrepetible = irrepetible;
            nat.enlace = enlace;
            nat.tabla_origen = tabla_origen;
            if (sig != null)
                nat.sig = sig.copia();
            return nat;
        }
    }
}