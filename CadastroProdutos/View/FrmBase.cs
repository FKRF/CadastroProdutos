using CadastroProdutos.Dao;
using CadastroProdutos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CadastroProdutos.View
{
    public partial class FrmBase : CadastroProdutos.Frm
    {
        Produtos produtoAtual = null;
        public enum Acao
        {
            Incluir,
            Alterar,
            Excluir
        }
        private Acao _acao;
        private ProdutoDao produtoDao = new ProdutoDao();
        private Produtos produto;
        public FrmBase(Acao acao)
        {
            InitializeComponent();
            _acao = acao;
            ConfigurarFormulario();
            txtCodigo.KeyDown -= txtCodigo_KeyDown;
            txtCodigo.KeyDown += txtCodigo_KeyDown;

            //MessageBox.Show("Evento btnSalvar.Click registrado");
        }
        private void ConfigurarFormulario()
        {
            txtCodigo.Enabled = (_acao == Acao.Alterar || _acao == Acao.Excluir);
            txtNome.Enabled = (_acao == Acao.Incluir);
            txtPreco.Enabled = (_acao == Acao.Incluir);
            txtCusto.Enabled = (_acao == Acao.Incluir);

            if (_acao == Acao.Alterar || _acao == Acao.Excluir)
            {
                txtNome.Enabled = false;
                txtPreco.Enabled = false;
                txtCusto.Enabled = false;
                txtCodigo.Leave -= BuscarProduto;

                txtCodigo.Leave += BuscarProduto;  // Ao perder o foco, busca o produto
            }
            btnSalvar.Click -= btnSalvar_Click;
            btnSalvar.Click += btnSalvar_Click;

            MessageBox.Show("btnSalvar_Click chamado");

        }
        private void BuscarProduto(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                    return; // Evita erro caso o usuário apenas clique e saia sem digitar

                if (!int.TryParse(txtCodigo.Text, out int codigo))
                {
                    MessageBox.Show("Código inválido! Digite um número.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                produtoAtual = produtoDao.BuscarPorCodigo(codigo);

                if (produtoAtual != null)
                {
                    txtNome.Text = produtoAtual.Nome;
                    txtPreco.Text = produtoAtual.Preco.ToString("F2");
                    txtCusto.Text = produtoAtual.Custo.ToString("F2");

                    if (_acao == Acao.Alterar)
                    {
                        txtNome.Enabled = true;
                        txtPreco.Enabled = true;
                        txtCusto.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Produto não encontrado!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao buscar o produto: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public virtual void Salvar()
        {
            if (_acao == Acao.Incluir)
            {
                Produtos novoProduto = new Produtos
                {
                    Nome = txtNome.Text,
                    Preco = Convert.ToDecimal(txtPreco.Text),
                    Custo = Convert.ToDecimal(txtCusto.Text)
                };
                produtoDao.IncluirProduto(novoProduto);
            }
            else if (_acao == Acao.Alterar && produtoAtual != null)
            {
                produtoAtual.Nome = txtNome.Text;
                produtoAtual.Preco = Convert.ToDecimal(txtPreco.Text);
                produtoAtual.Custo = Convert.ToDecimal(txtCusto.Text);
                produtoDao.AlterarProduto(produtoAtual);
            }
            else if (_acao == Acao.Excluir && produtoAtual != null)
            {
                produtoDao.ExcluirProduto(produtoAtual.Codigo);
            }
            this.Close();
        }
        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscarProduto(sender, e);
                e.SuppressKeyPress = true; // Evita o "bip" padrão do Windows ao pressionar Enter
            }
        }
        public void btnSalvar_Click(object sender, EventArgs e)
        {
            

            Salvar();
        }
        protected override void Sair()
        {
            this.Close();
        }
    }
}
