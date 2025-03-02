using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CadastroProdutos.Models;
using CadastroProdutos.Dao;
using System.Threading.Tasks;
using Mysqlx.Crud;

namespace CadastroProdutos.View
{
    public partial class FrmPrincipal : CadastroProdutos.Frm
    {
        private int offset = 0;  // Controla o ponto de onde os dados serão carregados
        private const int limit = 50;  // Número de produtos por vez
        public FrmPrincipal()
        {
            InitializeComponent();
            lviewProdutos.View = System.Windows.Forms.View.Details;
            lviewProdutos.GridLines = true;
            lviewProdutos.Clear();

            lviewProdutos.Columns.Add("Código", 60);
            lviewProdutos.Columns.Add("Nome", 300);
            lviewProdutos.Columns.Add("Preço", 60);
            lviewProdutos.Columns.Add("Foto", 60);
            lviewProdutos.Columns.Add("Custo", 60);

            lviewProdutos.MouseWheel += lviewProdutos_MouseWheel;
        }
        protected virtual void Pesquisar()
        {
            

        }
        protected virtual void Incluir()
        {

        }
        protected virtual void Alterar()
        {

        }
        protected virtual void Excluir()
        {

        }
        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            Incluir();
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            Alterar();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            Excluir();
        }

        private async void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            ProdutoDao produtoDao = new ProdutoDao();

            List<Produtos> produtos = await Task.Run(() => produtoDao.Consultar(offset, limit));
            foreach (Produtos produto in produtos)
            {
                ListViewItem item = new ListViewItem(produto.Codigo.ToString());
                item.SubItems.Add(produto.Nome);
                item.SubItems.Add(produto.Preco.ToString());
                item.SubItems.Add(produto.Foto);
                item.SubItems.Add(produto.Custo.ToString());

                lviewProdutos.Items.Add(item);
            }

            offset += limit;  // Atualiza o offset para carregar os próximos produtos
        }

        private void lviewProdutos_MouseWheel(object sender, MouseEventArgs e)
        {
            // Verifica se o usuário rolou para baixo
            if (e.Delta < 0)  // Se o valor de Delta for negativo, significa rolagem para baixo
            {
                // Verifica se a ListView atingiu o fim
                if (lviewProdutos.Items.Count > 0 &&
                    lviewProdutos.Items[lviewProdutos.Items.Count - 1].Bounds.Bottom <= lviewProdutos.ClientSize.Height)
                {
                    // Chama o método para carregar mais produtos
                    btnMostrarTodos_Click(sender, e);
                }
            }
        }


    }
}
