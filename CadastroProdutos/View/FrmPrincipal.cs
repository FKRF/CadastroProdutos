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
using System.Linq;

namespace CadastroProdutos.View
{
    public partial class FrmPrincipal : CadastroProdutos.Frm
    {
        private ProdutoDao produtoDao = new ProdutoDao();
        private int offset = 0;  // Controla o ponto de onde os dados serão carregados
        private const int limit = 50;  // Número de produtos por vez
        private List<Produtos> produtosCache = new List<Produtos>();
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
            lblConsulta.Visible = false;

            txtBoxPesquisa.Enabled = false;
            btnPesquisar.Enabled = false;
            btnMostrarTodos.Enabled = false;
            btnIncluir.Enabled = false;
            btnAlterar.Enabled = false;
            btnExcluir.Enabled = false;
        }
        
        private async void FrmPrincipal_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            
            produtosCache = await Task.Run(() => produtoDao.ConsultarTodosPaginado(offset, limit));
            
            progressBar1.Visible = false;

            txtBoxPesquisa.Enabled = true;
            btnPesquisar.Enabled = true;
            btnMostrarTodos.Enabled = true;
            btnIncluir.Enabled = true;
            btnAlterar.Enabled = true;
            btnExcluir.Enabled = true;
        }

        protected virtual void Pesquisar()
        {
            lviewProdutos.Items.Clear();  // Limpa os itens da ListView
            string pesquisa = txtBoxPesquisa.Text.Trim();  // Obtém o texto da pesquisa e remove espaços extras

            // Verifica se o texto é um número inteiro válido para buscar por código
            if (int.TryParse(pesquisa, out int codigo))
            {
                // Busca por código
                Produtos produto = produtoDao.BuscarPorCodigo(codigo);
                if (produto != null)
                {
                    ListViewItem item = new ListViewItem(produto.Codigo.ToString());
                    item.SubItems.Add(produto.Nome);
                    item.SubItems.Add(produto.Preco.ToString("C"));
                    item.SubItems.Add(produto.Foto);
                    item.SubItems.Add(produto.Custo.ToString("C"));
                    lviewProdutos.Items.Add(item);
                }
                else
                {
                    MessageBox.Show("Produto não encontrado pelo código!");
                }
            }
            else
            {
                // Busca por nome (se não for um número válido)
                List<Produtos> produtos = produtoDao.BuscarPorNome(pesquisa);
                if (produtos.Any())
                {
                    foreach (var produto in produtos)
                    {
                        ListViewItem item = new ListViewItem(produto.Codigo.ToString());
                        item.SubItems.Add(produto.Nome);
                        item.SubItems.Add(produto.Preco.ToString("C"));
                        item.SubItems.Add(produto.Foto);
                        item.SubItems.Add(produto.Custo.ToString("C"));
                        lviewProdutos.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum produto encontrado com esse nome!");
                }
            }
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
        private void PreencherListView()
        {
            foreach (Produtos produto in produtosCache)
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
        private async void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            lviewProdutos.Items.Clear();
            PreencherListView();
            int totalRegistros = await produtoDao.BuscarTotalRegistros();
            lblConsulta.Text = totalRegistros.ToString() + " itens encontrados";
            lblConsulta.Visible = true;
        }

        private void lviewProdutos_MouseWheel(object sender, MouseEventArgs e)
        {
            // Verifica se o usuário rolou para baixo
            if (e.Delta < 0)  // Se o valor de Delta for negativo, significa rolagem para baixo
            {
                // Verifica se a ListView atingiu o fim
                if (lviewProdutos.Items.Count > 0 &&
                    lviewProdutos.Items[lviewProdutos.Items.Count - 1].Bounds.Bottom <= lviewProdutos.ClientSize.Height)
                    btnMostrarTodos_Click(sender, e); // Chama o método para carregar mais produtos

            }
        }
    }
}
