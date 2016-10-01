using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avance
{
    class Subconsulta
    {
        public Tabla t_inicio, t_fin;
        public Campo s_inicio, s_fin;
        public Campo w_inicio, w_fin;
        public Alias a_inicio, a_fin;
        public Subconsulta izq;
        public Subconsulta der;
        public int nivel;
        public int cant = 0;
        public List<string> errores;
        public ArbolTablas arbTab;
        public bool where = false;
        public bool subs = false;
        public bool constante = false;
        public int num_tabla = 0;

        public bool opt_sel, opt_from, opt_alias, opt_subs;

        public Subconsulta() { }

        public Subconsulta(int n, List<string> e, ArbolTablas arbt)
        {
            arbTab = arbt;
            errores = e;
            nivel = n + 1;
        }

        public Alias buscar_alias(string nombre_tabla)
        {
            Alias al = a_inicio;
            while (al != null)
            {
                if (nombre_tabla == al.tabla.identificador.valor_de_referencia)
                    return al;
                al = al.sig;
            }
            return null;
        }

        public bool atributoRepetido(int codigo_buscar)
        {
            int cant = 0;
            Tabla t = t_inicio;
            NodoAtributo nat;
            while (t != null && cant < 2)
            {
                nat = t.buscarAtributo(codigo_buscar);
                if (nat != null)
                    cant++;
                t = t.ligaDer;
            }
            Alias a = a_inicio;
            while (a != null && cant < 2)
            {
                nat = a.tabla.buscarAtributo(codigo_buscar);
                if (nat != null)
                    cant++;
                a = a.sig;
            }
            return cant > 1;
        }

        public void buscar_atributo(Campo camp)
        {
            int codigo_buscar = camp.supuesto_atributo.codigo;
            Tabla t = t_inicio;
            NodoAtributo nat = null;
            while (t != null)
            {
                nat = t.buscarAtributo(codigo_buscar);
                if (nat != null)
                {
                    camp.tabla = t;
                    camp.atributo = nat;
                    return;
                }
                t = t.ligaDer;
            }
            Alias a = a_inicio;
            while (a != null)
            {
                nat = a.tabla.buscarAtributo(codigo_buscar);
                if (nat != null)
                {
                    camp.tabla = a.tabla;
                    camp.atributo = nat;
                    return;
                }
                a = a.sig;
            }
        }

        public void agregar_tabla(NodoToken nt)
        {
            num_tabla++;
            Tabla t = arbTab.buscar(nt.ligaNodoAlfanumerico.codigo);
            if (t == null)
            {
                errores.Add("Error 3:319 Línea: " + nt.Línea + " No existe la tabla \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" dentro de la base de datos");
                return;
            }
            Tabla tab = buscar_tabla(nt.codigo);
            if (tab == null)
            {
                t = t.copia();
                t.num_tabla = num_tabla;
                if (t_inicio == null)
                    t_inicio = t_fin = t;
                else
                {
                    t_fin.ligaDer = t;
                    t.ligaIzq = t_fin;
                    t_fin = t;
                }
            }
            else
                errores.Add("Error 3:318 Línea: " + nt.Línea + " Ya existe una tabla llamada \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" dentro de la subconsulta");
        }

        public void agregar_tabla(Tabla t)
        {
            Tabla tab = buscar_tabla(t.identificador.codigo);
            if (tab == null)
            {
                t.num_tabla = 0;
                if (t_inicio == null)
                    t_inicio = t_fin = t;
                else
                {
                    t_fin.ligaDer = t;
                    t.ligaIzq = t_fin;
                    t_fin = t;
                }
            }
        }

        public void agregar_alias(Alias al)
        {
            Alias a = buscar_alias(al.nuevo_nombre.codigo);
            if (a != null)
            {
                if (t_fin != t_inicio)
                {
                    t_fin.ligaIzq.ligaDer = null;
                    a.tabla.ligaIzq = null;
                }
                else
                    t_fin = t_inicio = null;
                if (a_inicio == null)
                    a_inicio = a_fin = a;
                else
                {
                    a_fin.sig = a;
                    a_fin = a;
                }
            }
        }

        public void agregar_alias(NodoToken nt)
        {
            if (!where)
            {
                NodoAlfanumerico nombre_alias = nt.ligaNodoAlfanumerico;
                Alias a = buscar_alias(nombre_alias.codigo);
                if (a != null)
                {
                    errores.Add("Error 3:318 Línea: " + nt.Línea + " Ya existe un alias llamado \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" dentro de la subconsulta");
                    return;
                }
                a = new Alias(nombre_alias, t_fin);
                if (t_fin != t_inicio)
                {
                    t_fin.ligaIzq.ligaDer = null;
                    a.tabla.ligaIzq = null;
                }
                else
                    t_fin = t_inicio = null;
                if (a_inicio == null)
                    a_inicio = a_fin = a;
                else
                {
                    a_fin.sig = a;
                    a_fin = a;
                }
            }
            else
            {
                Alias a = buscar_alias(nt.codigo);
                if (a == null)
                    errores.Add("Error 3:325 Línea: " + nt.Línea + " No existe un alias llamado \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" dentro de la subconsulta");
            }
        }

        public void agregar_campo(NodoAlfanumerico supuesto_atributo, string lin)
        {
            if (!where)
                if (s_inicio == null)
                {
                    Campo c = new Campo(0, supuesto_atributo, lin);
                    s_inicio = s_fin = c;
                }
                else
                {
                    Campo c = null;
                    c = new Campo(s_fin.indice, supuesto_atributo, lin);
                    s_fin.der = c;
                    c.izq = s_fin;
                    s_fin = c;
                }
            else
                if (w_inicio == null)
                {
                    Campo c = new Campo(0, supuesto_atributo, lin);
                    w_inicio = w_fin = c;
                }
                else
                {
                    Campo c = null;
                    c = new Campo(w_fin.indice, supuesto_atributo, lin);
                    w_fin.der = c;
                    c.izq = w_fin;
                    w_fin = c;
                }
        }

        public void cambiar_supuestos_CampoFin(NodoAlfanumerico sa, string Línea)
        {
            if (!where)
            s_fin.cambio_supuestos(sa, Línea);
            else
                w_fin.cambio_supuestos(sa,Línea);
        }

        public Tabla buscar_tabla(int codigo_interno)
        {
            Tabla t = t_inicio;
            while (t != null)
            {
                if (t.codigo == codigo_interno)
                    return t;
                t = t.ligaDer;
            }
            return null;
        }

        public Alias buscar_alias(int codigo_interno)
        {
            Alias a = a_inicio;
            while (a != null)
            {
                if (a.nuevo_nombre.codigo == codigo_interno)
                    return a;
                a = a.sig;
            }
            return null;
        }

        public void encontrar_tabla()
        {
            Campo auxiliar = !where ? s_inicio : w_inicio;
            while (auxiliar != null)
            {
                if (auxiliar.tabla == null)
                {
                    if (auxiliar.supuesta_tabla != null)
                    {
                        Alias a = null;
                        auxiliar.tabla = buscar_tabla(auxiliar.supuesta_tabla.codigo);
                        if (auxiliar.tabla == null)
                        {
                            a = buscar_alias(auxiliar.supuesta_tabla.codigo);
                            if (a != null)
                                auxiliar.tabla = a.tabla;
                            if (auxiliar.tabla == null)
                            {
                                errores.Add("Error 3:320 Línea: " + auxiliar.Línea_tabla + " No está definido el alias \"" + auxiliar.supuesta_tabla.valor_de_referencia + "\" en el FROM");
                                return;
                            }
                        }
                        NodoAtributo nat = auxiliar.tabla.buscarAtributo(auxiliar.supuesto_atributo.codigo);
                        if (nat != null)
                            auxiliar.atributo = nat;
                        else
                        {
                            if (a == null)
                                errores.Add("Error 3:321 Línea: " + auxiliar.Línea_atributo + " El atributo \"" + auxiliar.supuesto_atributo.valor_de_referencia + "\" no pertenece a ninguna tabla del FROM");
                            else
                                errores.Add("Error 3:328 Línea: " + auxiliar.Línea_atributo + " El atributo \"" + auxiliar.supuesto_atributo.valor_de_referencia + "\" no pertenece a la tabla \"" + auxiliar.tabla.identificador.valor_de_referencia + "\"");
                            return;
                        }
                    }
                    else
                    {
                        if (atributoRepetido(auxiliar.supuesto_atributo.codigo))
                        {
                            errores.Add("Error 3:322 Línea: " + auxiliar.Línea_atributo + " El atributo \"" + auxiliar.supuesto_atributo.valor_de_referencia + "\" es ambiguo");
                            return;
                        }
                        buscar_atributo(auxiliar);
                        if (auxiliar.atributo == null)
                        {
                            errores.Add("Error 3:321 Línea: " + auxiliar.Línea_atributo + " El atributo \"" + auxiliar.supuesto_atributo.valor_de_referencia + "\" no pertenece a ninguna tabla del FROM");
                            return;
                        }
                    }
                }
                auxiliar = auxiliar.der;
            }
        }

        public Subconsulta crear_tabla_inicio()
        { 
            Tabla tab = t_inicio;
            Subconsulta lista = new Subconsulta();
            while (tab != null)
            {
                lista.agregar_tabla(tab);
                tab = tab.ligaDer;
            }
            Alias a = a_inicio;
            while (a != null)
            {
                lista.agregar_tabla(a.tabla);
                a = a.sig;
            }
            return lista;
        }

        public void agregar_tablas(Subconsulta sub)
        {
            Tabla tab = sub.t_inicio;
            Alias al = sub.a_inicio;
            while (tab != null)
            {
                agregar_tabla(tab);
                tab = tab.sig;
            }
            while (al != null)
            {
                agregar_alias(al);
                al = al.sig;
            }
        }

        public void Ordenar()
        {
            throw new NotImplementedException();
        }
    }
}