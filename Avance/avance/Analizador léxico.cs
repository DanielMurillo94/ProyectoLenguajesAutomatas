using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avance
{
    class Analizador_léxico
    {
        ArbolDiccionario dicc;
        List<string> errores;
        ArbolAlfanumerico constantes;
        ArbolAlfanumerico identificadores;
        public NodoToken nt_inicio = null, nt_fin = null;
        int[,] autom = 
                       {//L,N, #,D,',\n
                        { 1,2,-1,3,3,0 }, //0
                        { 1,1, 3,0,0,0 }, //1
                        { 0,2, 0,0,0,0 }, //2
                        { 0,0, 0,0,0,0 }, //3
                        };
        public bool constante = false;
        bool salto = false;
        int lin = 0, f = 0, c = 0, fant = 0;
        public int index_letraActual = 0;
        public string Entrada = "";

        public Analizador_léxico()
        {}

        public Analizador_léxico(ArbolDiccionario d, ArbolAlfanumerico c, ArbolAlfanumerico i, List<string> e, string ent)
        {
            Entrada = ent;
            dicc = d;
            constantes = c;
            identificadores = i;
            errores = e;
            ini_Analizador_léxico();
        }

        NodoToken verificar(string s, NodoDiccionario nd)
        {
            nd = dicc.buscarNodo(s, nd);
            if (nd != null)
            { 
                if (nd.codigo < 86 || nd.codigo == 88)
                {
                    if (nd.codigo == 88)
                        return InsTabla(nd.lexema, nd.tipo, 60, 60);
                    return InsTabla(nd.lexema, nd.tipo, nd.codigo, nd.codigo);
                }
                else
                {
                    char car;
                    if (index_letraActual != Entrada.Length)
                    {
                        car = Entrada[index_letraActual];
                        if (car == '=')
                            switch (nd.codigo)
                            {
                                case 86:
                                    index_letraActual++;
                                    return InsTabla(nd.lexema + "=", nd.tipo, 60, 89);
                                case 87:
                                    index_letraActual++;
                                    return InsTabla(nd.lexema + "=", nd.tipo, 60, 90);
                            }
                        else
                            c = ' ';
                    }
                    else
                        c = ' ';
                    if (c == ' ')
                        switch (nd.codigo)
                        {
                            case 86:
                                return InsTabla(nd.lexema, nd.tipo, 60, 86);
                            case 87:
                                return InsTabla(nd.lexema, nd.tipo, 60, 87);
                        }
                }
            }
            return null;
        }

        NodoToken InsDat(string s, int pos)
        {
            long L = 0;
            NodoAlfanumerico na;
            if (s == "'")
            {
                constante = !constante;
                return InsTabla("'", "Delimitador", 54, 54);
            }
            else
                if (constante)
                {
                    na = constantes.insertar(s, 62);
                    return InsTabla(s, "Constante", 62, na);
                }
                else
                {
                    NodoToken nt = null;
                    if (s.Length > 1)
                        nt = verificar(s, dicc.inicioPalRes);
                    if (nt != null)
                        return nt;
                    nt = verificar(s, dicc.inicioRel);
                    if (nt != null)
                        return nt;
                    nt = verificar(s, dicc.inicioDelim);
                    if (nt != null)
                        return nt;
                    nt = verificar(s, dicc.inicioOper);
                    if (nt != null)
                        return nt;
                }
            try
            {
                L = long.Parse(s);
                na = constantes.insertar(L.ToString(), 61);
                return InsTabla(L.ToString(), "Constante", 61, na);
            }
            catch (Exception ms)
            {
                na = identificadores.insertar(s, 100);
                return InsTabla(s, "Identificador", 100, na);
            }
        }

        int tipo(char c)
        {
            if (c != '\n' && salto)
                salto = false;
            if (c != '\'' && (char.IsLetter(c) || c == '_' || constante))
            {
                return 0;
            }
            else
                if (char.IsDigit(c))
                {
                    return 1;
                }
                else
                    if (c == '#')
                    {
                        return 2;
                    }
                    else
                        if (c != ' ' && c != '\r' && c != '\t' && c != '\n')
                        {
                            if (c == '\'')
                                return 4;
                            NodoDiccionario nd = dicc.buscar(c.ToString(), dicc.getRaiz());
                            if (nd != null)
                                if (nd.tipo == "Delimitador" && c != '\'' || nd.tipo == "Operador" || nd.tipo == "Relacional")
                                    return 3;
                                else
                                    return 5;
                            return -1;
                        }
                        else
                            if (c == ' ' || c == '\r' || c == '\t')
                                return 5;
                            else
                                if (c == '\n')
                                {
                                    if (!salto)
                                        salto = true;
                                    else
                                        salto = false;
                                    return 5;
                                }
            return -1;
        }

        NodoToken InsTabla(string Tok, string Tip, int Val, int codigo_interno)
        {
            NodoToken nt = new NodoToken();
            nt.Línea = lin < 10 ? "0" + lin.ToString() : lin.ToString();
            nt.nombreToken = Tok;
            nt.tipo = Tip;
            nt.codigo = Val;
            nt.codigo_interno = codigo_interno;
            if (nt_inicio == null)
            {
                nt.numero = 1;
                nt_inicio = nt_fin = nt;
            }
            else
            {
                nt.numero = nt_fin.numero + 1;
                nt_fin.sig = nt;
                nt_fin = nt;
            }
            return nt;
        }

        NodoToken InsTabla(string Tok, string Tip, int Val, NodoAlfanumerico na)
        {
            NodoToken nt = new NodoToken();
            nt.Línea = lin < 10 ? "0" + lin.ToString() : lin.ToString();
            nt.nombreToken = Tok;
            nt.tipo = Tip;
            nt.codigo = Val;
            nt.codigo_interno = na.codigo;
            nt.ligaNodoAlfanumerico = na;
            if (nt_inicio == null)
            {
                nt.numero = 1;
                nt_inicio = nt_fin = nt;
            }
            else
            {
                nt.numero = nt_fin.numero + 1;
                nt_fin.sig = nt;
                nt_fin = nt;
            }
            return nt;
        }

        public NodoToken ant(NodoToken nd)
        {
            NodoToken nt = nt_inicio;
            while (nt.sig != null)
            {
                if (nt.sig == nd)
                    return nt;
                nt = nt.sig;
            }
            return null;
        }

        void ini_Analizador_léxico()
        {
            constante = false;
            lin = 1;
            f = c = fant = index_letraActual = 0;
        }

        public NodoToken getToken()
        {
            try
            {
                NodoToken nt = null;
                string s = "";
                int k = index_letraActual;
                while (k < Entrada.Length)
                {
                    c = tipo(Entrada[k]);
                    if (c == -1)
                    {
                        errores.Add("Error 1.101 Línea " + lin.ToString() + " Símbolo desconocido '" + Entrada[k].ToString() + "'");
                        return null;
                    }
                    fant = f;
                    f = autom[f, c];
                    if (f == -1)
                    {
                        errores.Add("Error 1.101 Línea " + lin.ToString() + " Símbolo desconocido '" + Entrada[k].ToString() + "'");
                        return null;
                    }
                    if (f == 0)
                    {
                        if (s != "")
                        {
                            nt = InsDat(s, k);
                            if (index_letraActual != k)
                                k = index_letraActual;
                            k--;
                        }
                    }
                    else
                        s = s + Entrada[k];
                    index_letraActual = k = k + 1;
                    if (salto)
                        lin++;
                    if (nt != null)
                        return nt;
                }
                if (s != "")
                    return InsDat(s, Entrada.Length - 1);
                return InsTabla("$", "Fin de lectura", 199, 199);
            }
            catch (Exception ms)
            {
                errores.Add("Error fatal léxico:\nLínea: " + nt_fin.Línea + " \nOcurrió una excepción dentro del escaner, de seguimiento al proceso durante la depuración");
                return null;
            }
        }

        public void Llenar_Tokens(DataGridView tokens)
        {
            if (nt_inicio != null)
            {
                NodoToken nt = nt_inicio;
                def_Tokens(tokens);
                tokens.RowCount = 1;
                tokens.RowCount = nt_fin.numero + 1;
                int cont = 0;
                while (nt != null)
                {
                    cont++;
                    tokens[0, cont].Value = nt.numero;
                    tokens[1, cont].Value = nt.Línea;
                    tokens[2, cont].Value = nt.nombreToken;
                    tokens[3, cont].Value = nt.tipo;
                    tokens[4, cont].Value = nt.codigo;
                    nt = nt.sig;
                }
            }
        }

        public void def_Tokens(DataGridView tokens)
        {
            tokens.ColumnCount = 5;
            tokens.RowCount = 1;
            tokens[0, 0].Value = "No.";
            tokens[1, 0].Value = "Línea";
            tokens[2, 0].Value = "Token";
            tokens[3, 0].Value = "Tipo";
            tokens[4, 0].Value = "Código";
        }
    }
}
