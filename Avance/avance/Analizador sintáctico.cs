using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class Analizador_sintáctico
    {
        TablaSintáctica tabSint;
        Stack<string> pila;
        public NodoSintáctico ns_inicio, ns_fin = null, ns = null;
        int valor = 0, cod = 0, l = 0;
        string p1 = "", cad = "";
        int term = 0, reg = 0;
        string X = "", K = "";
        public Analizador_léxico anLex = new Analizador_léxico();

        ArbolAlfanumerico constantes;
        ArbolAlfanumerico identificadores;
        List<string> errores = new List<string>();
        ArbolDiccionario dicc = new ArbolDiccionario();

        //-----------------Semántico-------
        public Analizador_Semántico anSem;

        public Analizador_sintáctico()
        {}

        public Analizador_sintáctico(TablaSintáctica ts, ArbolDiccionario d, List<string> e, string ent, ArbolAlfanumerico c, ArbolAlfanumerico i)
        {
            anSem = new Analizador_Semántico(e);
            tabSint = ts;
            constantes = c;
            identificadores = i;
            dicc = d;
            errores = e;
            anLex = new Analizador_léxico(d, constantes, identificadores, errores, ent);
            ini_Parser();
        }

        public void def_Sint(DataGridView tp)
        {
            tp.ColumnCount = 4;
            tp.RowCount = 1;
            tp[0, 0].Value = "Pila";
            tp[1, 0].Value = "Tabla léxica";
            tp[2, 0].Value = "X";
            tp[3, 0].Value = "K";
        }

        void agregar(NodoSintáctico ns)
        {
            if (ns_inicio == null)
            {
                ns.numero = 1;
                ns_inicio = ns_fin = ns;
            }
            else
            {
                ns.numero = ns_fin.numero + 1;
                ns_fin.sig = ns;
                ns_fin = ns;
            }
        }

        int codigos(int valor)
        {
            if (valor >= 10 && valor < 50)
                return 205;
            if (valor >= 50 && valor < 60)
                return 202;
            if (valor >= 61 && valor < 70)
                return 201;
            if (valor == 60)
                return 206;
            if (valor >= 70 && valor < 99)
                return 204;
            return 203;
        }

        public void ini_Parser()
        {
            cad = p1 = X = K = "";
            pila = new Stack<string>();
            ns_inicio = ns_fin = null;
            pila.Push("199");
            p1 = "199 " + p1;
            pila.Push("200");
            p1 = "200 " + p1;
            ns = new NodoSintáctico();
            ns.pila = p1;
            valor = l = cod = term = reg = 0;
        }

        public void Parser()
        {
            try
            {
                NodoProducto np;
                NodoCabezal nc;
                NodoDiccionario nd;
                anSem.consulta = new Consulta();
                NodoToken nt = anLex.getToken();
                int reglaActual = 0;
                if (errores.Count != 0)
                    return;
                do
                {
                    agregar(ns);
                    ns = new NodoSintáctico();
                    X = pila.Pop();
                    p1 = p1.Remove(0, X.Length + 1);
                    ns.pila = p1;
                    nc = tabSint.buscarCabezal(int.Parse(X), (NodoCabezal)tabSint.raiz1);
                    if (nc == null)
                        reg = -1;
                    else
                        reglaActual = reg = nc.codigo;
                    nc = tabSint.buscarCabezal(nt.codigo, (NodoCabezal)tabSint.raiz2);
                    term = nc != null ? nc.codigo : -1;
                    K = nc != null ? nc.codigo.ToString() : "";
                    ns.X = X;
                    ns.K = K;
                    np = tabSint.buscar(reg, term);
                    if (terminal(X) || X == "199")
                        if (X == K)
                        {
                            anSem.Acción(reglaActual, nt);
                            if (errores.Count != 0)
                                return;
                            nt = anLex.getToken();
                            if (errores.Count != 0)
                                return;
                        }
                        else
                        {
                            valor = 0;
                            cod = 0;
                            NodoToken token_anterior = anLex.ant(nt);
                            l = int.Parse(token_anterior == null ? nt.Línea : token_anterior.Línea);
                            if (anLex.constante && anLex.Entrada.Length == anLex.index_letraActual)
                                errores.Add("Error 2.202" + " Línea: " + (l < 10 ? "0" + l.ToString() : l.ToString()) + " Falta el delimitador apóstrofe");
                            if (X == "199")
                            {
                                cad = "Sobra ";
                                valor = int.Parse(K);
                                cod = codigos(valor);
                                if (cod == 206)
                                {
                                    cod = 204;
                                    cad += "Operador";
                                    nd = new NodoDiccionario();
                                    nd.lexema = "";
                                    nd.tipo = " relacional";
                                }
                                else
                                    nd = dicc.buscar(valor);
                                cad = cad + (nd == null ? "Revisar caso" : nd.tipo + " " + nd.lexema);
                            }
                            else
                            {
                                cad = "Se esperaba ";
                                valor = int.Parse(X);
                                cod = codigos(valor);
                                if (cod == 206)
                                {
                                    cod = 204;
                                    cad += "Operador";
                                    nd = new NodoDiccionario();
                                    nd.lexema = "";
                                    nd.tipo = " relacional";
                                }
                                else
                                    nd = dicc.buscar(valor);
                                cad = cad + (nd == null ? "Revisar caso" : nd.tipo + " " + (cod != 203 ? nd.lexema : ""));
                            }
                            errores.Add("Error 2." + cod + " Línea " + (l < 10 ? "0" + l.ToString() : l.ToString()) + " " + cad);
                            return;
                        }
                    else
                        if (np != null)
                        {
                            if (np.Producto != "99")
                            {
                                string prod = np.Producto;
                                string[] productos = prod.Split(' ');
                                for (int k = productos.Length - 1; k != -1; k--)
                                {
                                    p1 = productos[k] + " " + p1;
                                    pila.Push(productos[k]);
                                }
                                agregar(ns);
                                ns = new NodoSintáctico();
                                ns.pila = p1;
                            }
                        }
                        else
                        {
                            NodoToken token_anterior = anLex.ant(nt);
                            l = int.Parse(token_anterior == null ? nt.Línea : token_anterior.Línea);
                            if (anLex.constante && anLex.Entrada.Length == anLex.index_letraActual)
                                errores.Add("Error 2.202" + " Línea " + (l < 10 ? "0" + l.ToString() : l.ToString()) + " Falta el delimitador apóstrofe");
                            if (reg == -1)
                                errores.Add("Error fatal en analizador sintáctico:\nLínea: " + (l < 10 ? "0" + l.ToString() : l.ToString()) + " Regla: \"Desconocida\"" + " Terminal: " + term.ToString() + " \nAsegúrese que la regla o terminales se encuentren en la tabla.\nAdemás, verifique que no hay espacios dobles, iniciales o finales dentro de los productos de la tabla.");
                            else
                                cadenasError(reg, l, nt);
                            return;
                        }
                } while (X != "199");
            }
            catch (Exception ms)
            {
                NodoToken nt = anLex.getToken();
                l = int.Parse(nt == null ? "-1" : nt.Línea);
                errores.Add("Error fatal desconocido:\nLínea: " + (l < 10 && l > 0 ? "0" + l.ToString() : l.ToString()) + " Regla: " + reg.ToString() + " Terminal: " + term.ToString() + " \nOcurrió una excepción no controlada dentro de uno de los métodos internos del compilador");
            }
        }

        void cadenasError(int regla, int Línea, NodoToken nt)
        {
            string cad = "Se esperaba ";
            NodoCabezal ncf = tabSint.buscarCabezal(regla, (NodoCabezal)tabSint.raiz1);
            NodoProducto np = ncf.inicio;
            NodoDiccionario nd;
            int cod = 0;
            while (np != null)
            {
                NodoCabezal ncc = np.terminal;
                if (np.Producto != "99")
                {
                    nd = dicc.buscar(ncc.codigo);
                    cod = codigos(nd.codigo);
                    if (cod == 206)
                    {
                        cod = 204;
                        cad += "Operador";
                        nd = new NodoDiccionario();
                        nd.lexema = "";
                        nd.tipo = " relacional";
                    }
                    cad = cad + nd.tipo + " " + (cod != 203 ? nd.lexema : "");
                    if (cod == 201)
                        cad = "Tipo de dato " + nt.nombreToken + " no válido";
                    errores.Add("Error 2." + cod + " Línea: " + (Línea < 10 ? "0" + Línea.ToString() : Línea.ToString()) + " " + cad);
                    cad = "Se esperaba ";
                }
                np = np.ligaTerminal;
            }
        }

        public void llenarParser(DataGridView tp)
        {
            if (ns_inicio != null)
            {
                NodoSintáctico nt = ns_inicio;
                def_Sint(tp);
                tp.RowCount = 1;
                tp.RowCount = ns_fin.numero + 1;
                int cont = 0;
                while (nt != null)
                {
                    cont++;
                    tp[0, cont].Value = nt.pila;
                    tp[1, cont].Value = nt.X;
                    tp[2, cont].Value = nt.K;
                    nt = nt.sig;
                }
            }
        }

        bool terminal(string s)
        {
            NodoCabezal ncc = tabSint.buscarCabezal(int.Parse(s), (NodoCabezal)tabSint.raiz2);
            return ncc != null;
        }
    }
}
