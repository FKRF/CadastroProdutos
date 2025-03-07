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
        public Produtos BuscarPorCodigo(int codigo)
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
                //string query = @"SELECT codigo, nome, preco, foto, custo FROM produtos";
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

        public void IncluirProduto(Produtos produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto), "O produto não pode ser nulo.");

            // Validação de dados (exemplo, adicione conforme necessário)
            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("O nome do produto não pode ser vazio.");

            // Usando 'using' para garantir que a conexão será fechada corretamente
            using (MySqlConnection conexao = _conexao.AbrirConexao())
            {
                try
                {
                    string query = @"INSERT INTO produtos (nome, preco, foto, custo) 
                             VALUES (@nome, @preco, @foto, @custo)";

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    {
                        // Adicionando os parâmetros de forma segura
                        comando.Parameters.AddWithValue("@nome", produto.Nome);
                        comando.Parameters.AddWithValue("@preco", produto.Preco);
                        comando.Parameters.AddWithValue("@foto", produto.Foto);
                        comando.Parameters.AddWithValue("@custo", produto.Custo);

                        // Executa a query de inserção
                        comando.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // Adiciona mais contexto no erro
                    throw new Exception("Erro ao incluir produto no banco de dados. Detalhes: " + ex.Message, ex);
                }
            }
        }
        public void AlterarProduto(Produtos produto)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto), "O produto não pode ser nulo.");

            if (produto.Codigo <= 0)
                throw new ArgumentException("Código do produto inválido.");

            if (string.IsNullOrWhiteSpace(produto.Nome))
                throw new ArgumentException("O nome do produto não pode ser vazio.");

            using (MySqlConnection conexao = _conexao.AbrirConexao())
            {
                try
                {
                    string query = @"UPDATE produtos 
                             SET nome = @nome, preco = @preco, foto = @foto, custo = @custo 
                             WHERE codigo = @codigo";

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@codigo", produto.Codigo);
                        comando.Parameters.AddWithValue("@nome", produto.Nome);
                        comando.Parameters.AddWithValue("@preco", produto.Preco);
                        comando.Parameters.AddWithValue("@foto", produto.Foto);
                        comando.Parameters.AddWithValue("@custo", produto.Custo);

                        int linhasAfetadas = comando.ExecuteNonQuery();
                        if (linhasAfetadas == 0)
                            throw new Exception("Nenhum produto foi atualizado. Verifique se o código informado está correto.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao alterar produto no banco de dados. Detalhes: " + ex.Message, ex);
                }
            }
        }

        public void ExcluirProduto(int codigo)
        {
            if (codigo <= 0)
                throw new ArgumentException("Código do produto inválido.");

            using (MySqlConnection conexao = _conexao.AbrirConexao())
            {
                try
                {
                    string query = "DELETE FROM produtos WHERE codigo = @codigo";

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@codigo", codigo);

                        int linhasAfetadas = Convert.ToInt32(comando.ExecuteNonQuery());

                        if (linhasAfetadas == 0)
                        {
                            MessageBox.Show("Nenhum produto foi excluído. O código informado pode não existir.",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao excluir produto. Detalhes: " + ex.Message, ex);
                }
            }
        }


    }
}
