using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class ArbolDiccionario : Arbol
    {
        public NodoDiccionario inicioPalRes, finPalRes;
        public NodoDiccionario inicioDelim, finDelim;
        public NodoDiccionario inicioOper, finOper;
        public NodoDiccionario inicioRel, finRel;

        public void agregar(string tipo, string lexema, int codigo)
        {
            NodoDiccionario nd = new NodoDiccionario(tipo, lexema, codigo);
            insertar(raiz1, raiz1, nd, ref raiz1);
        }

        public void insertarNodo(string tipo, string lexema, int codigo)
        {
            NodoDiccionario nd = new NodoDiccionario(tipo, lexema, codigo);
            switch (tipo)
            {
                case "Palabra reservada":
                    if (inicioPalRes == null)
                        inicioPalRes = finPalRes = nd;
                    else
                    {
                        finPalRes.liga = nd;
                        finPalRes = nd;
                    }
                    break;
                case "Delimitador": 
                    if (inicioDelim == null)
                        inicioDelim = finDelim = nd;
                    else
                    {
                        finDelim.liga = nd;
                        finDelim = nd;
                    }
                    break;
                case "Operador":
                    if (inicioOper == null)
                        inicioOper = finOper = nd;
                    else
                    {
                        finOper.liga = nd;
                        finOper = nd;
                    }
                    break;
                case "Relacional":
                    if (inicioRel == null)
                        inicioRel = finRel = nd;
                    else
                    {
                        finRel.liga = nd;
                        finRel = nd;
                    }
                    break;
            }
        }

        public NodoDiccionario getRaiz()
        {
            return (NodoDiccionario)raiz1;
        }

        public NodoDiccionario buscar(int val)
        {
            return (NodoDiccionario)buscar(val, raiz1);
        }

        public NodoDiccionario buscarNodo(string lexema, NodoDiccionario nd)
        {
            NodoDiccionario p = nd;
            int longitud = lexema.Length;
            lexema = lexema.ToUpper();
            while (p != null)
            {
                if (longitud == p.lexema.Length)
                    if (p.lexema == lexema)
                        return p;
                p = p.liga;
            }
            return null;
        }

        public NodoDiccionario buscar(string tipo, Nodo p)
        {
            NodoDiccionario inicial = (NodoDiccionario)buscar(5, raiz1);
            NodoDiccionario q = null;
            if (p != null)
            {
                q = buscar(tipo, p.izq);
                if (((NodoDiccionario)p).tipo.ToUpper() == tipo.ToUpper() || ((NodoDiccionario)p).lexema.ToUpper() == tipo.ToUpper() || ((NodoDiccionario)p).lexema == tipo)
                    return (NodoDiccionario)p;
                if (q == null)
                    q = buscar(tipo, p.der);
            }
            return q;
        }
    }
}
