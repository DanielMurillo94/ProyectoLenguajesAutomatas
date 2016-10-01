using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace Avance
{
    class Compilar
    {
        public List<string> errores = new List<string>();
        public ArbolDiccionario dicc = new ArbolDiccionario();
        public ArbolTablas BD = new ArbolTablas();
        public ArbolAlfanumerico constantes = new ArbolAlfanumerico(1000);
        public ArbolAlfanumerico identificadores = new ArbolAlfanumerico(10000);
        public TablaSintáctica tabSint;

        StreamReader ArchLeer;

        public Analizador_sintáctico anSin = new Analizador_sintáctico();

        public Compilar()
        {}

        public void limpiar()
        {
            BD = new ArbolTablas();
            constantes = new ArbolAlfanumerico(1000);
            identificadores = new ArbolAlfanumerico(10000);
        }

        public void Com_Iniciar(string entrada)
        {
            BD.consulta_ejecutada_parcialmente = false;
            errores = new List<string>();
            anSin = new Analizador_sintáctico(tabSint, dicc, errores, entrada, constantes, identificadores);
            anSin.anSem.arbTab = BD;
            anSin.anSem.arbTab.errores = errores;
            anSin.Parser();
        }

        public void CrearTS()
        {
            tabSint = new TablaSintáctica();
            Cargar("Diccionario");
            Cargar("Tabla sintáctica");
        }

        public void Def_Tablas(DataGridView tokens, DataGridView sintáctico)
        {
            anSin.anLex.def_Tokens(tokens);
            anSin.def_Sint(sintáctico);
        }

        public void Llenar_Tablas(DataGridView tokens, DataGridView sintáctico)
        {
            if (anSin.anLex.nt_inicio != null && anSin.ns_inicio != null)
            {
                anSin.anLex.Llenar_Tokens(tokens);
                anSin.llenarParser(sintáctico);
            }
        }

        void LlenarCabeceras(string[] elementos, ref NodoCabezal nc)
        {
            for (int x = 0; x < elementos.Length; x++)
                tabSint.agregarCabezal(ref nc, int.Parse(elementos[x]));
        }

        void Cargar(string archivo)
        {
            if (ArchLeer != null)
                ArchLeer.Close();
            string ruta = AppDomain.CurrentDomain.BaseDirectory + "Documentos\\" + archivo + ".txt";
            ArchLeer = new StreamReader(ruta);
            if (archivo == "Tabla sintáctica")
            {
                string Línea;
                string[] elementos;
                int regla, terminal = 0;
                Línea = ArchLeer.ReadLine();
                elementos = Línea.Split(',');
                NodoCabezal nc = (NodoCabezal)tabSint.raiz2;
                LlenarCabeceras(elementos, ref nc);
                tabSint.raiz2 = nc;
                Línea = ArchLeer.ReadLine();
                elementos = Línea.Split(',');
                nc = (NodoCabezal)tabSint.raiz1;
                LlenarCabeceras(elementos, ref nc);
                tabSint.raiz1 = nc;
                while (!ArchLeer.EndOfStream)
                {
                    Línea = ArchLeer.ReadLine();
                    elementos = Línea.Split(',');
                    regla = int.Parse(elementos[0]);
                    bool producto = false;
                    for (int x = 1; x < elementos.Length; x++)
                    {
                        if (!producto)
                            terminal = int.Parse(elementos[x]);
                        else
                            tabSint.agregar(regla, terminal, elementos[x]);
                        producto = !producto;
                    }
                }
            }
            else
            {
                string Línea, s;
                string[] elementos;
                while (!ArchLeer.EndOfStream)
                {
                    Línea = ArchLeer.ReadLine();
                    elementos = Línea.Split('¬');
                    s = elementos[1].ToUpper();
                    dicc.agregar(elementos[0], s, int.Parse(elementos[2]));
                    dicc.insertarNodo(elementos[0], s, int.Parse(elementos[2]));
                }
            }
        }

        public void Llenar_Errores(RichTextBox mensajes)
        {
            string texto = "";
            for (int x = 0; x < errores.Count; x++)
                texto = texto + errores[x] + '\n';
            if (texto == "")
                Mensdef(1, mensajes);
            else
            {
                if (BD.consulta_ejecutada_parcialmente)
                {
                    texto = "Consulta ejecutada con errores.\n" + texto;
                    mensajes.ForeColor = Color.OrangeRed;
                }
                else
                    mensajes.ForeColor = Color.Red;
                mensajes.Text = texto;
            }
        }

        public void Mensdef(int espera, RichTextBox mensajes)
        {
            Color c = Color.Black;
            string txt = "Esperando...";
            switch (espera)
            {
                case 1:
                    c = Color.Green;
                    txt = "Libre de errores...";
                    break;
                case 2:
                    c = Color.Blue;
                    txt = "Procesando...";
                    break;
            }
            mensajes.ForeColor = c;
            mensajes.Text = txt;
            mensajes.Refresh();
        }

        public void mostrarTablas()
        {
            string mens = "";
            BD.mens(ref mens, BD.raiz);
            if (mens.Length == 0)
                MessageBox.Show("No hay tablas dentro de la BD", "No hay tablas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("La base de datos tiene las tablas: " + mens.Remove(0, 2), "Lista de tablas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
