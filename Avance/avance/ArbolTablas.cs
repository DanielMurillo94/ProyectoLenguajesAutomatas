using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class ArbolTablas
    {
        public Tabla raiz;
        private Tabla q, r, aux;
        public List<string> errores;
        public bool consulta_ejecutada_parcialmente = false;

        
        public void mens(ref string m, Tabla t)
        {
            if (t != null)
            {
                mens(ref m, t.izq);
                m = m + ", \"" + t.identificador.valor_de_referencia + "\"";
                mens(ref m, t.der);
            }
        }

        public ArbolTablas()
        { }

        public void agregar(Tabla tab)
        {
            if (tab != null)
            {
                if (buscar(tab.codigo) == null)
                    insertar(tab);
                if (tab.izq != null)
                    agregar(tab.izq);
                if (tab.der != null)
                    agregar(tab.der);
            }
        }

        public ArbolTablas(List<string> err)
        {
            errores = err;
        }

        public void insertar(Tabla t)
        {
            insertar(raiz, raiz, t);
        }

        public ArbolTablas copiar(List<string>e)
        {
            ArbolTablas arbTab = new ArbolTablas();
            if (raiz != null)
            {
                arbTab.raiz = raiz.copia();
                arbTab.errores = e;
            }
            return arbTab;
        }

        private void insertar(Tabla p, Tabla ant, Tabla datos)
        {
            int fbx = 0, fby = 0;
            if (p == null)
            {
                consulta_ejecutada_parcialmente = true;
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
                        consulta_ejecutada_parcialmente = true;
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
                            consulta_ejecutada_parcialmente = true;
                            datos.nivel = p.nivel + 1;
                            q = datos;
                            p.der = q;
                            r = raiz;
                        }
                        else
                            insertar(p.der, p, datos);
                    }
                    else
                        errores.Add("Error 3:306 Línea: " + datos.Línea + " La tabla \"" + datos.identificador.valor_de_referencia + "\" ya existe dentro de la base de datos");
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

        public void nivel(ref Tabla p, int cont)
        {
            if (p != null)
            {
                p.nivel = cont;
                nivel(ref p.izq, cont + 1);
                nivel(ref p.der, cont + 1);
            }
        }

        public void DRder(ref Tabla p)
        {
            Tabla q = p.izq, r = q.der;
            q.der = r.izq;
            r.izq = q;
            p.izq = r.der;
            r.der = p;
            p = r;
            int niv = p.nivel;
            p.nivel = p.der.nivel;
            p.der.nivel = niv;
        }

        public void DRizq(ref Tabla p)
        {
            Tabla q = p.der, r = q.izq;
            q.izq = r.der;
            r.der = q;
            p.der = r.izq;
            r.izq = p;
            p = r;
            int niv = p.nivel;
            p.nivel = p.izq.nivel;
            p.izq.nivel = niv;
        }

        public void Rder(ref Tabla p)
        {
            Tabla q = p.izq;
            p.izq = q.der;
            q.der = p;
            p = q;
            int niv = p.nivel;
            p.nivel = q.der.nivel;
            q.der.nivel = niv;
        }

        public void Rizq(ref Tabla p)
        {
            Tabla q = p.der;
            p.der = q.izq;
            q.izq = p;
            p = q;
            int niv = p.nivel;
            p.nivel = q.izq.nivel;
            q.izq.nivel = niv;
        }

        public void NivelMax(Tabla p, ref int x)
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

        public Tabla buscar(int codigo)
        {
            Tabla r = raiz;
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
