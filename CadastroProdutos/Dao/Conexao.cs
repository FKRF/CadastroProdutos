using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Prng;

namespace CadastroProdutos.Dao
{
    internal class Conexao
    {
        private string _stringConexao;
        public Conexao()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            // Obter a string de conexão do arquivo JSON
            _stringConexao = configuration.GetConnectionString("DefaultConnection");

            // Obter a senha do banco de dados a partir de uma variável de ambiente
            string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (string.IsNullOrEmpty(dbPassword))
            {
                MessageBox.Show("A senha do banco de dados não foi definida como variável de ambiente.");
                return;
            }

            // Substituir o placeholder pela senha real
            _stringConexao = _stringConexao.Replace("{DB_PASSWORD}", dbPassword);

        }
        public MySqlConnection AbrirConexao()
        {
            MySqlConnection sqlConnection = new MySqlConnection(_stringConexao);
            sqlConnection.Open();
            return sqlConnection;
        }
        public void FecharConexao(MySqlConnection sqlConnection)
        {
            if(sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();
        }
    }
}
