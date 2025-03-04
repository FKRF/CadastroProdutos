using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CadastroProdutos.Models;
using MySql.Data.MySqlClient;

namespace CadastroProdutos.Dao
{
    internal class ProdutoDao
    {
        private Conexao _conexao;
        public ProdutoDao()
        {
            _conexao = new Conexao();
        }
        public Produtos BuscarPorCodigo(int  codigo)
        {
            Produtos produto = null;
            MySqlConnection conexao = _conexao.AbrirConexao();
            try
            {
                string query = @"SELECT codigo, nome, preco, foto, custo
                                 FROM produtos
                                 WHERE codigo = @codigo";
                MySqlCommand comando = new MySqlCommand(query, conexao);
                comando.Parameters.AddWithValue("@codigo", codigo);
                MySqlDataReader reader = comando.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    produto = new Produtos();
                    produto.Codigo = reader.GetInt32(0);
                    produto.Nome = reader.GetString(1);
                    produto.Preco = reader.GetDecimal(2);
                    produto.Foto = reader.GetString(3);
                    produto.Custo = reader.GetDecimal(4);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro no acesso ao banco de dados: " + ex.Message);
            }
            finally
            {
                conexao.Close();
            }
            return produto;
        }
        public List<Produtos> BuscarPorNome(string nome)
        {
            List<Produtos> produtos = new List<Produtos>();
            MySqlConnection conexao = _conexao.AbrirConexao();
            try
            {
                string query = @"SELECT codigo, nome, preco, foto, custo
                                 FROM produtos
                                 WHERE nome LIKE @nome";
                MySqlCommand comando = new MySqlCommand(query, conexao);
                comando.Parameters.AddWithValue("@nome", "%" + nome + "%");
                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Produtos produto = new Produtos();
                    produto = new Produtos();
                    produto.Codigo = reader.GetInt32(0);
                    produto.Nome = reader.GetString(1);
                    produto.Preco = reader.GetDecimal(2);
                    produto.Foto = reader.GetString(3);
                    produto.Custo = reader.GetDecimal(4);

                    produtos.Add(produto);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro no acesso ao banco de dados: " + ex.Message);
            }
            finally
            {
                conexao.Close();
            }
            return produtos;
        }
        public async Task<int> BuscarTotalRegistros()
        {
            int total = 0;
            MySqlConnection conexao = _conexao.AbrirConexao();
            try
            {
                string query = @"SELECT COUNT(*) FROM produtos";
                MySqlCommand comando = new MySqlCommand(query, conexao);
                var resultado = await comando.ExecuteScalarAsync();
                total = resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0; // Se resultado for DBNull, define como 0
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro no acesso ao banco de dados" + ex.Message);
                total = -1;
            }
            finally
            {
                conexao.Close();
            }
            return total;
        }
        public List<Produtos> ConsultarTodosPaginado(int offset, int limit)
        {
            List<Produtos> produtos = new List<Produtos>();

            MySqlConnection conexao = _conexao.AbrirConexao();
            try
            {
                string query = @"SELECT codigo, nome, preco, foto, custo FROM produtos LIMIT @limit OFFSET @offset";
                MySqlCommand comando = new MySqlCommand(query, conexao);

                comando.Parameters.AddWithValue("@limit", limit);
                comando.Parameters.AddWithValue("@offset", offset);

                MySqlDataReader reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    Produtos produto = new Produtos();
                    produto.Codigo = reader.GetInt32(0);
                    produto.Nome = reader.GetString(1);
                    produto.Preco = reader.GetDecimal(2);
                    produto.Foto = reader.GetString(3);
                    produto.Custo = reader.GetDecimal(4);

                    produtos.Add(produto);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar produto: " + ex.Message);
            }
            finally
            {
                _conexao.FecharConexao(conexao);
            }

            return produtos;
        }

        public void IncluirProduto(string nome, decimal preco, string foto, decimal custo)
        {
            

            MySqlConnection conexao = _conexao.AbrirConexao();
            try
            {
                string query = @"INSERT INTO produtos (nome, preco, foto, custo) 
                                VALUES (@nome, @preco, @foto, @custo";
                MySqlCommand comando = new MySqlCommand(query, conexao);
                //comando.Parameters.Add("")
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao incluir produto" + ex.Message);
            }
            finally
            {
                _conexao.FecharConexao(conexao);
            }
        }
    }
}
