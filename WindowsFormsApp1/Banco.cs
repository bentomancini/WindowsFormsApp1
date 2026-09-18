using System.Configuration;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class Banco
    {
        public const string StringConexao =
            @"Data Source=Lab3-7; Initial Catalog=Tecfy; Integrated Security=True";

        public static SqlConnection ObterConexao()
        {
            return new SqlConnection(ObterStringConexao());
        }

        public static string ObterStringConexao()
        {
            var config = ConfigurationManager.ConnectionStrings["Tecfy"];
            if (config != null && !string.IsNullOrWhiteSpace(config.ConnectionString))
                return config.ConnectionString;
            return StringConexao;
        }
    }
}
