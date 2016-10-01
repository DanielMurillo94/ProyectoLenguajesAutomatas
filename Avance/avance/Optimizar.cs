using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class Optimizar
    {
        public string select(Subconsulta cons, ArbolTablas arb) // Regresa la cadena de todos los atributos usados en el select en la forma [alias | tabla].atributo;  
        {
            string optimizado = "";
            if (cons.s_inicio != null)
            {
                Campo aux = cons.s_inicio;
                while (aux != null)
                {
                    optimizado = optimizado + campo(cons, aux, ref cons.opt_alias) + ", ";
                    aux = aux.der;
                }
            }
            else
            {
                cons.opt_sel = true;
                int cont = 0;
                Tabla tab = null, sin_alias = cons.t_inicio;
                Alias al = cons.a_inicio;
                do
                {
                    if (tab != null)
                    {
                        Tabla aux = arb.buscar(tab.codigo);
                        tab.inicio = aux.inicio.copia();
                        NodoAtributo nat = tab.inicio;
                        while (nat != null)
                        {
                            optimizado = optimizado + tab.nuevo_nombre.valor_de_referencia + "." + nat.identificador.valor_de_referencia + ", ";
                            nat = nat.sig;
                        }
                    }
                    cont++;
                    tab = seleccion(ref al, al != null ? al.tabla : null, ref sin_alias, cont);
                } while (tab != null);
            }
            optimizado = optimizado.Remove(optimizado.Length - 2, 2);
            return optimizado;
        }

        public string from(Subconsulta cons)
        {
            string optimizado = "";
            
            return optimizado;
        }

        public string campo(Subconsulta cons, Campo camp, ref bool option)
        {
            string optimizado = "";
            Alias al = cons.buscar_alias(camp.tabla.identificador.valor_de_referencia);
            if (al != null)
                optimizado = optimizado + al.nuevo_nombre.valor_de_referencia + "." + camp.atributo.identificador.valor_de_referencia;
            else
            {
                option = true;
                optimizado = optimizado + camp.tabla.identificador.valor_de_referencia + "." + camp.atributo.identificador.valor_de_referencia;
            }
            return optimizado;
        }

        private Tabla seleccion(ref Alias a, Tabla ca, ref Tabla sa, int cont)
        {
            if (sa == null && sa == null)
                return null;
            if (sa == null)
            {
                ca.nuevo_nombre = a.nuevo_nombre;
                a = a.sig;
                return ca;
            }
            if (ca != null && sa != null)
            {
                if (sa.num_tabla == cont)
                {
                    Tabla copia = sa.copia();
                    copia.nuevo_nombre = sa.identificador;
                    sa = sa.ligaDer;
                    return copia;
                }
                else
                {
                    ca.nuevo_nombre = a.nuevo_nombre;
                    a = a.sig;
                    return ca;
                }
            }
            if (ca == null)
            {
                Tabla copia = sa.copia();
                copia.nuevo_nombre = sa.identificador;
                sa = sa.ligaDer;
                return copia;
            }
            return null;
        }
    }
}
