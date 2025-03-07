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
using Org.BouncyCastle.Tls;
using static CadastroProdutos.View.FrmBase;

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

            DesenharListView();
            
            lviewProdutos.MouseWheel += lviewProdutos_MouseWheel;

            lblConsulta.Visible = false;
            progressBar1.Visible = false;

        }
        private void DesenharListView()
        {
            lviewProdutos.View = System.Windows.Forms.View.Details;
            lviewProdutos.GridLines = true;
            lviewProdutos.Clear();

            lviewProdutos.Columns.Add("Código", 60);
            lviewProdutos.Columns.Add("Nome", 180);
            lviewProdutos.Columns.Add("Preço", 60);
            lviewProdutos.Columns.Add("Foto", 150);
            lviewProdutos.Columns.Add("Custo", 80);
            lviewProdutos.Width = lviewProdutos.Columns[0].Width + lviewProdutos.Columns[1].Width +
                lviewProdutos.Columns[2].Width + lviewProdutos.Columns[3].Width +
                lviewProdutos.Columns[4].Width;
        }
        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            Pesquisar();
        }
        protected virtual void Pesquisar()
        {
            lviewProdutos.Items.Clear();
            lblConsulta.Text = string.Empty;
            lblConsulta.Visible = true;
            string pesquisa = txtBoxPesquisa.Text.Trim();

            List<Produtos> produtosEncontrados = new List<Produtos>();

            // Verifica se o texto é um número inteiro válido para buscar por código
            if (int.TryParse(pesquisa, out int codigo))
            {
                // Busca por código
                Produtos produto = produtoDao.BuscarPorCodigo(codigo);
                if (produto != null)
                    produtosEncontrados.Add(produto);
            }
            else
                // Busca por nome
                produtosEncontrados = produtoDao.BuscarPorNome(pesquisa);
            ExibirResultados(produtosEncontrados);
        }
        // Método para preencher a ListView e exibir a mensagem apropriada
        private void ExibirResultados(List<Produtos> produtos)
        {
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
                lblConsulta.Text = produtos.Count == 1 ? "1 produto encontrado." : $"{produtos.Count} produtos encontrados.";
            }
            else
                lblConsulta.Text = "Nenhum produto encontrado!";
        }
        private async void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            offset = 0;
            produtosCache.Clear();
            lviewProdutos.Items.Clear();
            progressBar1.Visible = true;
            await PreencherListView();
            int totalRegistros = await Task.Run(() => produtoDao.BuscarTotalRegistros());
            lblConsulta.Text = totalRegistros.ToString() + " itens encontrados";
            lblConsulta.Visible = true;
        }
        private async Task PreencherListView()
        {
            progressBar1.Visible = true;
            List<Produtos> produtosNovos = await Task.Run(() => produtoDao.ConsultarTodosPaginado(offset, limit));
            progressBar1.Visible = false;

            if (produtosNovos.Count == 0)
                return; // Não há mais produtos para carregar
            foreach (Produtos produto in produtosNovos)
            {
                ListViewItem item = new ListViewItem(produto.Codigo.ToString());
                item.SubItems.Add(produto.Nome);
                item.SubItems.Add(produto.Preco.ToString());
                item.SubItems.Add(produto.Foto);
                item.SubItems.Add(produto.Custo.ToString());

                lviewProdutos.Items.Add(item);
            }
            offset += limit;
        }
        private async void lviewProdutos_MouseWheel(object sender, MouseEventArgs e)
        {
            // Verifica se o usuário rolou para baixo
            if (e.Delta < 0)  // Se o valor de Delta for negativo, significa rolagem para baixo
            {
                // Verifica se a ListView atingiu o fim
                if (lviewProdutos.Items.Count > 0 &&
                    lviewProdutos.Items[lviewProdutos.Items.Count - 1].Bounds.Bottom <= lviewProdutos.ClientSize.Height)
                    await PreencherListView();
            }
        }
        protected virtual void Incluir()
        {
            FrmBase frmBase = new FrmBase(FrmBase.Acao.Incluir);
            frmBase.ShowDialog();

        }
        protected virtual void Alterar()
        {
            FrmBase frmBase = new FrmBase(FrmBase.Acao.Alterar);
            frmBase.ShowDialog();
        }
        protected virtual void Excluir()
        {
            FrmBase frmBase = new FrmBase(FrmBase.Acao.Excluir);
            frmBase.ShowDialog();
        }
        private void btnIncluir_Click(object sender, EventArgs e)
            =>  Incluir();
        private void btnAlterar_Click(object sender, EventArgs e)
            => Alterar();
        private void btnExcluir_Click(object sender, EventArgs e)
            => Excluir();
    }
}
