using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class Analizador_Semántico
    {
        public ArbolTablas arbTab = new ArbolTablas();
        Tabla tab_aux = new Tabla();
        NodoAtributo att_aux, lista_atributos, lista_inicio;
        NodoRestriccion rest_aux;
        public List<string> errores;
        public Consulta consulta;
        public Subconsulta sub_aux = new Subconsulta();
        public Optimizar optimizar = new Optimizar();
        public string select_optimizado = "";
        public string from_optimizado = "";
        public string where_optimizado = "";

        public Analizador_Semántico() { }

        public Analizador_Semántico(List<string> err)
        {
            arbTab = new ArbolTablas(err);
            errores = err;
        }

        public void Acción(int regla, NodoToken nt)
        {
            try
            {
                switch (regla)
                {
                    //Crear tabla
                    case 201:
                        switch (nt.codigo)
                        {
                            case 100:
                                tab_aux = new Tabla(nt.codigo_interno, nt.ligaNodoAlfanumerico, nt.Línea, errores);
                                break;
                            case 16:
                                if (tab_aux.raiz1 != null)
                                    arbTab.insertar(tab_aux);
                                break;
                        }
                        break;
                    //Área de atributo
                    case 202:
                        if (nt.codigo == 100)
                            att_aux = new NodoAtributo(nt.codigo_interno, nt.ligaNodoAlfanumerico, nt.Línea, errores);
                        break;
                    case 203:
                        if (nt.codigo == 18 || nt.codigo == 19 || nt.codigo == 32)
                            att_aux.tipoDato = nt.codigo;
                        break;
                    case 218:
                        if (nt.codigo == 61)
                            att_aux.longitud_1 = nt.ligaNodoAlfanumerico;
                        break;
                    case 221:
                        if (nt.codigo == 61)
                            att_aux.longitud_2 = nt.ligaNodoAlfanumerico;
                        break;
                    case 204:
                        if (nt.codigo == 20)
                            att_aux.noNulo = true;
                        break;
                    //Agregar atributo a la tabla
                    case 205:
                        tab_aux.agregar(ref tab_aux.raiz1, att_aux);
                        tab_aux.ins_nat_Lista(att_aux);
                        att_aux = new NodoAtributo();
                        break;
                    //Área de restricción
                    case 207:
                        switch (nt.codigo)
                        {
                            case 22:
                                if (att_aux.identificador != null)
                                {
                                    tab_aux.agregar(ref tab_aux.raiz1, att_aux);
                                    tab_aux.ins_nat_Lista(att_aux);
                                    att_aux = new NodoAtributo();
                                }
                                break;
                            case 100:
                                if (!tab_aux.contiene(nt.codigo_interno, tab_aux.raiz1))
                                    rest_aux = new NodoRestriccion(nt.codigo_interno, nt.ligaNodoAlfanumerico, nt.Línea);
                                else
                                    errores.Add("Error 3:307 Línea: " + nt.Línea + " Ya existe un atributo \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" con el nombre de esa restricción");
                                break;
                        }
                        break;
                    case 208:
                        switch (nt.codigo)
                        {
                            case 24:
                                if (tab_aux.buscarRestricciónPrimaria((NodoRestriccion)tab_aux.raiz2) == null)
                                    rest_aux.tipo = 3;
                                else
                                    errores.Add("Error 3:310 Línea: " + nt.Línea + " Ya existe llave primaria dentro de la tabla \"" + tab_aux.identificador.valor_de_referencia + "\"");
                                break;
                            case 25:
                                rest_aux.tipo = 4;
                                break;
                            case 100:
                                NodoAtributo nat = tab_aux.buscarAtributo(nt.codigo_interno);
                                if (nat != null)
                                {
                                    if (rest_aux.tipo == 3)
                                        nat.irrepetible = true;
                                    rest_aux.ligaNodoAtributo = tab_aux.buscarAtributo(nt.codigo_interno);
                                }
                                else
                                    errores.Add("Error 3:303 Línea: " + nt.Línea + " El nombre del atributo \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" no existe dentro de la tabla \"" + tab_aux.identificador.valor_de_referencia + "\"");
                                break;
                        }
                        break;
                    case 222://References
                        if (nt.codigo == 100)
                        {
                            Tabla t = arbTab.buscar(nt.codigo_interno);
                            if (t != null)
                                rest_aux.ligaTabla = t;
                            else
                                errores.Add("Error 3:308 Línea: " + nt.Línea + " La tabla \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" no existe");
                        }
                        break;
                    case 219:
                        if (nt.codigo == 100)
                        {
                            NodoAtributo nat = rest_aux.ligaTabla.buscarAtributo(nt.codigo_interno);
                            if (nat != null)
                            {
                                if (nat.tipoDato == rest_aux.ligaNodoAtributo.tipoDato && nat.longitud_1 == rest_aux.ligaNodoAtributo.longitud_1 && nat.longitud_2 == rest_aux.ligaNodoAtributo.longitud_2)
                                {
                                    if (nat.irrepetible)
                                    {
                                        rest_aux.ligaTablaNodoAtributo = nat;
                                        rest_aux.ligaNodoAtributo.enlace = nat;
                                        rest_aux.ligaNodoAtributo.enlace.tabla_origen = rest_aux.ligaTabla;
                                    }
                                    else
                                        errores.Add("Error 3:315 Línea: " + nt.Línea + " El atributo \"" + nat.identificador.valor_de_referencia + "\" de la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\" no es llave primaria");
                                }
                                else
                                    errores.Add("Error 3:301 Línea: " + nt.Línea + " El tipo de dato o longitud del atributo \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" es diferente al del atributo que se encuentra en la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\"");
                            }
                            else
                                errores.Add("Error 3:305 Línea: " + nt.Línea + " El atributo \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" no existe en la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\"");
                        }
                        break;
                    case 210: //Regla 210 y 223 son iguales, deben tener el mismo código dentro
                        if (rest_aux.ligaTablaNodoAtributo == null && rest_aux.tipo == 4)
                        {
                            NodoRestriccion nr = rest_aux.ligaTabla.buscarRestricciónPrimaria((NodoRestriccion)rest_aux.ligaTabla.raiz2);
                            if (nr != null)
                                if (nr.ligaNodoAtributo.tipoDato == rest_aux.ligaNodoAtributo.tipoDato && nr.ligaNodoAtributo.longitud_1 == rest_aux.ligaNodoAtributo.longitud_1 && nr.ligaNodoAtributo.longitud_2 == rest_aux.ligaNodoAtributo.longitud_2)
                                    rest_aux.ligaTablaNodoAtributo = nr.ligaNodoAtributo;
                                else
                                    errores.Add("Error 3:301 Línea: " + nt.Línea + " El tipo de dato o longitud del atributo \"" + rest_aux.ligaNodoAtributo.identificador.valor_de_referencia + "\" es diferente a la llave primaria que se encuentra en la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\"");
                            else
                                errores.Add("Error 3:309 Línea: " + nt.Línea + " No existe llave primaria dentro de la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\"");
                        }
                        tab_aux.agregar(ref tab_aux.raiz2, rest_aux);
                        break;
                    case 223:
                        if (rest_aux.ligaTablaNodoAtributo == null && rest_aux.tipo == 4)
                        {
                            NodoRestriccion nr = rest_aux.ligaTabla.buscarRestricciónPrimaria((NodoRestriccion)rest_aux.ligaTabla.raiz2);
                            if (nr != null)
                                if (nr.ligaNodoAtributo.tipoDato == rest_aux.ligaNodoAtributo.tipoDato && nr.ligaNodoAtributo.longitud_1 == rest_aux.ligaNodoAtributo.longitud_1 && nr.ligaNodoAtributo.longitud_2 == rest_aux.ligaNodoAtributo.longitud_2)
                                    rest_aux.ligaTablaNodoAtributo = nr.ligaNodoAtributo;
                                else
                                    errores.Add("Error 3:301 Línea: " + nt.Línea + " El tipo de dato o longitud del atributo \"" + rest_aux.ligaNodoAtributo.identificador.valor_de_referencia + "\" es diferente a la llave primaria que se encuentra en la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\"");
                            else
                                errores.Add("Error 3:309 Línea: " + nt.Línea + " No existe llave primaria dentro de la tabla \"" + rest_aux.ligaTabla.identificador.valor_de_referencia + "\"");
                        }
                        tab_aux.agregar(ref tab_aux.raiz2, rest_aux);
                        break;
                    //Agregar tabla al arbol de tablas
                    case 217:
                        arbTab.insertar(tab_aux);
                        tab_aux = new Tabla();
                        break;
                    case 211:
                        switch (nt.codigo)
                        {
                            case 27:
                                if (tab_aux.raiz1 != null)
                                    arbTab.insertar(tab_aux);
                                tab_aux = new Tabla();
                                break;
                            //Este es el nombre de la tabla a insertar
                            case 100:
                                tab_aux = arbTab.buscar(nt.codigo_interno);
                                if (tab_aux == null)
                                    errores.Add("Error 3:314 Línea: " + nt.Línea + " No existe la tabla \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" dentro de la base de datos");
                                break;
                        }
                        break;
                    //Se inserta
                    case 213:
                        string tipo = "";
                        switch (lista_atributos.tipoDato)
                        {
                            case 18:
                                tipo = "CHAR";
                                break;
                            case 19:
                                tipo = "NUMERIC";
                                break;
                        }
                        switch (nt.codigo)
                        {
                            //Se insertó un digito
                            case 61:
                                if (lista_atributos.tipoDato == 19)
                                {
                                    if (nt.ligaNodoAlfanumerico.valor_de_referencia.Length <= int.Parse(lista_atributos.longitud_1.valor_de_referencia))
                                    {
                                        NodoAlfanumerico na;
                                        if (lista_atributos.enlace != null)
                                        {
                                            na = lista_atributos.enlace.buscar(nt.codigo_interno);
                                            if (na == null)
                                                errores.Add("Error 3:317 Línea: " + nt.Línea + " No existe \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" en la tabla \"" + lista_atributos.enlace.tabla_origen.identificador.valor_de_referencia + "\"");
                                        }
                                        if (lista_atributos.irrepetible)
                                        {
                                            NodoAtributo at = tab_aux.buscarAtributo(lista_atributos.codigo);
                                            na = at.buscar(nt.codigo_interno);
                                            if (na == null)
                                                lista_atributos.agregarElemento(nt.ligaNodoAlfanumerico.copia());
                                            else
                                                errores.Add("Error 3:311 Línea: " + nt.Línea + " El atributo \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" ya existe dentro de la tabla \"" + tab_aux.identificador.valor_de_referencia + "\" debido a la llave no se permite repetir valores");
                                        }
                                        break;
                                    }
                                }
                                errores.Add("Error 3:312 Línea: " + nt.Línea + " Se esperaba un valor " + tipo + "(" + lista_atributos.longitud_1.valor_de_referencia + ")");
                                break;
                            //Se insertó un char
                            case 62:
                                if (lista_atributos.tipoDato == 18)
                                {
                                    if (nt.ligaNodoAlfanumerico.valor_de_referencia.Length <= int.Parse(lista_atributos.longitud_1.valor_de_referencia))
                                    {
                                        NodoAlfanumerico na;
                                        if (lista_atributos.enlace != null)
                                        {
                                            na = lista_atributos.enlace.buscar(nt.codigo_interno);
                                            if (na == null)
                                                errores.Add("Error 3:317 Línea: " + nt.Línea + " No existe \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" en la tabla \"" + lista_atributos.enlace.tabla_origen.identificador.valor_de_referencia + "\"");
                                        }
                                        if (lista_atributos.irrepetible)
                                        {
                                            NodoAtributo at = tab_aux.buscarAtributo(lista_atributos.codigo);
                                            na = at.buscar(nt.codigo_interno);
                                            if (na == null)
                                                lista_atributos.agregarElemento(nt.ligaNodoAlfanumerico.copia());
                                            else
                                                errores.Add("Error 3:311 Línea: " + nt.Línea + " El atributo \"" + nt.ligaNodoAlfanumerico.valor_de_referencia + "\" ya existe dentro de la tabla \"" + tab_aux.identificador.valor_de_referencia + "\" debido a la llave no se permite repetir valores");
                                        }
                                        break;
                                    }
                                }
                                errores.Add("Error 3:313 Línea: " + nt.Línea + " Se esperaba un valor " + tipo + "(" + lista_atributos.longitud_1.valor_de_referencia + ")");
                                break;
                        }
                        break;
                    case 214:
                        switch (nt.codigo)
                        {
                            //Se recorre al siguiente nodo atributo
                            case 50:
                                lista_atributos = lista_atributos.sig;
                                if (lista_atributos == null)
                                    errores.Add("Error 3:316 Línea: " + nt.Línea + " La tabla \"" + tab_aux.identificador.valor_de_referencia + "\" tiene " + tab_aux.fin.contador + " elementos");
                                break;
                        }
                        break;
                    //Se pone la lista de atributos al inicio de la lista de la tabla de atributos
                    case 215:
                        if (nt.codigo == 52)
                        {
                            lista_atributos = tab_aux.inicio.copia();
                            lista_inicio = lista_atributos;
                        }
                        break;
                    case 216:
                        if (nt.codigo == 50 || nt.codigo == 55 || nt.codigo == 199)
                        {
                            if (lista_atributos.sig != null)
                            {
                                errores.Add("Error 3:317 Línea: " + nt.Línea + " La tabla \"" + tab_aux.identificador.valor_de_referencia + "\" tiene " + tab_aux.fin.contador + " elementos");
                                return;
                            }
                            lista_atributos = lista_inicio;
                            NodoAtributo nat = tab_aux.inicio;
                            while (nat != null && errores.Count == 0)
                            {
                                nat.agregar(lista_atributos.raiz, ref nat);
                                nat = nat.sig;
                                lista_atributos = lista_atributos.sig;
                            }
                            arbTab.consulta_ejecutada_parcialmente = true;
                            tab_aux = new Tabla();
                        }
                        break;
                    //Select
                    case 300:
                        if (nt.codigo == 10)
                        {
                            if (tab_aux.raiz1 != null)
                                arbTab.insertar(tab_aux);
                            tab_aux = new Tabla();
                            consulta.agregar(errores, arbTab);
                        }
                        break;
                    case 301:
                        if (consulta.fin.nivel != 0)
                            errores.Add("Error 3:324 Línea: " + nt.Línea + " Solo se puede especificar un atributo en la lista de selección cuando es subconsulta");
                        if (nt.codigo == 30)
                            select_optimizado = select_optimizado + "DISTINCT ";
                        break;
                    case 304:
                        switch (nt.codigo)
                        {
                            case 100:
                                consulta.fin.agregar_campo(nt.ligaNodoAlfanumerico, nt.Línea);
                                if (consulta.fin.nivel != 0 && consulta.fin.s_fin.indice > 1)
                                    errores.Add("Error 3:323 Línea: " + nt.Línea + " Solo se puede especificar un atributo en la lista de selección cuando es subconsulta");
                                break;
                        }
                        break;
                    case 305:
                        if (nt.codigo == 100)
                            consulta.fin.cambiar_supuestos_CampoFin(nt.ligaNodoAlfanumerico, nt.Línea);
                        break;
                    case 308:
                        if (nt.codigo == 100)
                            consulta.fin.agregar_tabla(nt);
                        break;
                    case 309:
                        if (nt.codigo == 100)
                            consulta.fin.agregar_alias(nt);
                        break;
                    case 310:
                        switch (nt.codigo)
                        {
                            case 12:
                                consulta.fin.encontrar_tabla();
                                if (errores.Count == 0)
                                    if (consulta.fin.nivel == 0)
                                        select_optimizado = select_optimizado + optimizar.select(consulta.fin, arbTab);
                                    else
                                    {
                                        consulta.inicio.opt_subs = true;
                                        where_optimizado = where_optimizado + optimizar.campo(consulta.fin, consulta.fin.s_inicio, ref consulta.inicio.opt_alias) + " AND ";
                                    }
                                consulta.fin.where = true;
                                consulta.fin.cant = 0;
                                break;
                            case 53: //Fin de subconsulta sin WHERE
                                consulta.fin.encontrar_tabla();
                                if (errores.Count != 0)
                                    return;
                                Subconsulta ant = consulta.fin.izq;
                                NodoAtributo aux0 = ant.w_fin.atributo, aux1 = consulta.fin.s_inicio.atributo;
                                if (aux0.tipoDato != aux1.tipoDato || aux0.longitud_1 != aux1.longitud_1 || aux0.longitud_2 != aux1.longitud_2)
                                    errores.Add("Error 3:326 Línea: " + consulta.fin.w_inicio.Línea_atributo + " El atributo \"" + aux1.identificador.valor_de_referencia + "\" no coincide con el atributo que se requiere buscar");
                                else
                                    where_optimizado = where_optimizado + optimizar.campo(consulta.fin, consulta.fin.s_inicio, ref consulta.inicio.opt_alias) + " ";
                                sub_aux.agregar_tablas(consulta.fin);
                                consulta.eliminarultimo();
                                consulta.fin.subs = true;
                                break;
                            case 199://Fin de consulta
                                consulta.fin.encontrar_tabla();
                                if (errores.Count == 0)
                                    if (consulta.fin.nivel == 0)
                                        select_optimizado = select_optimizado + optimizar.select(consulta.fin, arbTab);
                                optimizar_from();
                                break;
                        }
                        break;
                    case 312: switch (nt.codigo)
                        {
                            case 53: //Fin de subconsulta con WHERE
                                consulta.fin.encontrar_tabla();
                                if (errores.Count != 0)
                                    return;
                                Subconsulta ant = consulta.fin.izq;
                                NodoAtributo aux0 = ant.w_fin.atributo, aux1 = consulta.fin.s_inicio.atributo;
                                if (aux0.tipoDato != aux1.tipoDato || aux0.longitud_1 != aux1.longitud_1 || aux0.longitud_2 != aux1.longitud_2)
                                {
                                    errores.Add("Error 3:326 Línea: " + consulta.fin.w_inicio.Línea_atributo + " El atributo \"" + aux1.identificador.valor_de_referencia + "\" no coincide con el atributo que se requiere buscar");
                                    return;
                                }
                                if (!consulta.fin.constante && !consulta.fin.subs)
                                {
                                    NodoAtributo auxx0 = consulta.fin.w_fin.izq.atributo, auxx1 = consulta.fin.w_fin.atributo;
                                    if (auxx0.tipoDato != auxx1.tipoDato || auxx0.longitud_1 != auxx1.longitud_1 || auxx0.longitud_2 != auxx1.longitud_2)
                                        errores.Add("Error 3:326 Línea: " + consulta.fin.w_fin.Línea_atributo + " El atributo \"" + auxx1.identificador.valor_de_referencia + "\" no coincide con el atributo que se requiere buscar");
                                    else
                                        where_optimizado = where_optimizado + optimizar.campo(consulta.fin, consulta.fin.w_fin, ref consulta.inicio.opt_alias) + " ";
                                }
                                sub_aux.agregar_tablas(consulta.fin);
                                consulta.eliminarultimo();
                                consulta.fin.subs = true;
                                break;
                            case 199:
                                if (!consulta.fin.constante && !consulta.fin.subs)
                                {
                                    consulta.fin.encontrar_tabla();
                                    if (errores.Count != 0)
                                        return;
                                    NodoAtributo auxx0 = consulta.fin.w_fin.izq.atributo, auxx1 = consulta.fin.w_fin.atributo;
                                    if (auxx0.tipoDato != auxx1.tipoDato || auxx0.longitud_1 != auxx1.longitud_1 || auxx0.longitud_2 != auxx1.longitud_2)
                                        errores.Add("Error 3:326 Línea: " + consulta.fin.w_fin.Línea_atributo + " El atributo \"" + auxx1.identificador.valor_de_referencia + "\" no coincide con el atributo que se requiere buscar");
                                    else
                                        where_optimizado = where_optimizado + optimizar.campo(consulta.fin, consulta.fin.w_fin, ref consulta.inicio.opt_alias) + " ";
                                }
                                else
                                    consulta.fin.constante = false;
                                optimizar_from();
                                break;
                        }
                        break;
                    case 314:
                        consulta.fin.encontrar_tabla();
                        if (nt.codigo == 13)
                            where_optimizado = where_optimizado + optimizar.campo(consulta.fin, consulta.fin.w_fin, ref consulta.inicio.opt_alias) + " = ";
                        break;
                    case 315:
                        consulta.fin.encontrar_tabla();
                        if (errores.Count != 0)
                            return;
                        where_optimizado = where_optimizado + optimizar.campo(consulta.fin, consulta.fin.w_fin, ref consulta.inicio.opt_alias) + " = ";
                        consulta.fin.subs = false;
                        break;
                    case 317:
                        if (!consulta.fin.constante && !consulta.fin.subs)
                        {
                            consulta.fin.encontrar_tabla();
                            if (errores.Count != 0)
                                return;
                            NodoAtributo auxx0 = consulta.fin.w_fin.izq.atributo, auxx1 = consulta.fin.w_fin.atributo;
                            if (auxx0.tipoDato != auxx1.tipoDato || auxx0.longitud_1 != auxx1.longitud_1 || auxx0.longitud_2 != auxx1.longitud_2)
                                errores.Add("Error 3:326 Línea: " + consulta.fin.w_fin.Línea_atributo + " El atributo \"" + auxx1.identificador.valor_de_referencia + "\" no coincide con el atributo que se requiere buscar");
                            if (errores.Count != 0)
                                return;
                        }
                        else
                        {
                            consulta.fin.subs = false;
                            consulta.fin.constante = false;
                        }
                        switch (nt.codigo)
                        {
                            case 14:
                                where_optimizado = where_optimizado + "AND ";
                                break;
                            case 15:
                                where_optimizado = where_optimizado + "OR ";
                                break;
                        }
                        break;
                    case 318:
                        if (consulta.fin.w_fin.atributo.tipoDato == 19 && nt.codigo == 62)
                            errores.Add("Error 3:327 Línea: " + nt.Línea + " El atributo \"" + consulta.fin.w_fin.atributo.identificador.valor_de_referencia + "\" no coincide con el tipo de dato");
                        consulta.fin.constante = true;
                        if (errores.Count == 0 && nt.codigo == 62)
                            where_optimizado = where_optimizado + "'" + nt.ligaNodoAlfanumerico.valor_de_referencia + "' ";
                        break;
                    case 319:
                        if (nt.codigo == 61)
                        {
                            if (att_aux != null)
                                att_aux.longitud_1 = nt.ligaNodoAlfanumerico;
                            else
                                if (consulta.fin != null)
                                {
                                    if (consulta.fin.w_fin.atributo.tipoDato == 18)
                                        errores.Add("Error 3:327 Línea: " + nt.Línea + " El atributo \"" + consulta.fin.w_fin.atributo.identificador.valor_de_referencia + "\" no coincide con el tipo de dato");
                                    consulta.fin.constante = true;
                                    if (errores.Count == 0)
                                        where_optimizado = where_optimizado + nt.ligaNodoAlfanumerico.valor_de_referencia + " ";
                                }
                        }
                        break;
                }
            }
            catch (Exception ms)
            {
                errores.Add("Error fatal semántico:\nLínea: " + nt.Línea + " Regla: " + regla.ToString() + " Terminal: " + nt.codigo.ToString() + " \nVerifique que no exista ninguna asignación de un resultado de una búsqueda nula");
            }
        }

        private void optimizar_from()
        {
            consulta.inicio.agregar_tablas(sub_aux);
            Subconsulta lista = consulta.inicio.crear_tabla_inicio();
            consulta.inicio.Ordenar();
            from_optimizado = optimizar.from(consulta.inicio);
        }
    }
}