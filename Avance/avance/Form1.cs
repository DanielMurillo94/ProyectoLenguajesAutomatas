using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Avance
{
    public partial class Form1 : Form
    {
        Compilar comp = new Compilar();

        public Form1()
        {
            InitializeComponent();
            comp.CrearTS();
            comp.Def_Tablas(tokens, procesoSintáctico);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comp.Mensdef(2, mensajes);
            comp.Com_Iniciar(Entrada.Text);
            comp.Llenar_Errores(mensajes);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (Entrada.Text == "")
                comp.Mensdef(0, mensajes);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Entrada.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comp.limpiar();
            comp.Mensdef(0, mensajes);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comp.mostrarTablas();
        }
    }
}
