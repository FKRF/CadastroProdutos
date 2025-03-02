using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CadastroProdutos.Models;
using CadastroProdutos.Dao;

namespace CadastroProdutos.View
{
    public partial class FrmPrincipal : CadastroProdutos.Frm
    {
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
        }
        protected virtual void Pesquisar()
        {
            ProdutoDao produtoDao = new ProdutoDao();

            List<Produtos> produtos = produtoDao.Consultar();
            foreach(Produtos produto in produtos)
            {
                ListViewItem item = new ListViewItem(produto.Codigo.ToString());
                item.SubItems.Add(produto.Nome);
                item.SubItems.Add(produto.Preco.ToString());
                item.SubItems.Add(produto.Foto);
                item.SubItems.Add(produto.Custo.ToString());
                
                lviewProdutos.Items.Add(item);
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
        
    }
}
