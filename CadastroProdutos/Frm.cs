using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadastroProdutos
{
    public partial class Frm : Form
    {
        public Frm()
        {
            InitializeComponent();
        }

        protected virtual void Sair()
        {
            Application.Exit();
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            Sair();
        }
    }
}
