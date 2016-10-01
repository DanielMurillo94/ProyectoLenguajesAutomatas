using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class Árbol
    {
        private Nodo q, r, aux;
        public Nodo raiz;

        protected void insertar(Nodo p, Nodo ant, Nodo datos)
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
                    else
                        MessageBox.Show("Ya hay un nodo con ese dato.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            raiz = p;
                        nivel(ref raiz, 0);
                    }
                }
            }
        }

        protected void nivel(ref Nodo p, int cont)
        {
            if (p != null)
            {
                p.nivel = cont;
                nivel(ref p.izq, cont + 1);
                nivel(ref p.der, cont + 1);
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

        protected Nodo buscar(int codigo, Nodo R)
        {
            Nodo r = R;
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
                return r;
            }
            return null;

        }
    }
}
