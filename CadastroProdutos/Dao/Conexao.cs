using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CadastroProdutos.Dao
{
    internal class Conexao
    {
        private string _stringConexao;
        public Conexao()
        {
            var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).
                AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
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
