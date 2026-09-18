using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApp1
{
    public static class UsuarioService
    {
        public class Resultado
        {
            public bool Ok;
            public string Erro;
        }

        public class UsuarioLogado
        {
            public int Id;
            public string Nome;
        }

        private static string Hash(string senha)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha + "TecfyFeira2026"));
                return Convert.ToBase64String(bytes);
            }
        }

        private static bool EmailExiste(SqlConnection con, string email)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Usuarios WHERE Email = @EMAIL", con))
            {
                cmd.Parameters.AddWithValue("@EMAIL", email);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static Resultado Cadastrar(string nome, string email, string senha, string confirmar)
        {
            nome = (nome ?? "").Trim();
            email = (email ?? "").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(nome))
                return new Resultado { Erro = "Informe o seu nome." };

            if (!email.Contains("@") || !email.Contains("."))
                return new Resultado { Erro = "Digite um email valido." };

            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 4)
                return new Resultado { Erro = "A senha deve ter pelo menos 4 caracteres." };

            if (senha != confirmar)
                return new Resultado { Erro = "As senhas nao conferem." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    if (EmailExiste(con, email))
                        return new Resultado { Erro = "Este email ja esta cadastrado." };

                    string sql = @"INSERT INTO Usuarios (Nome, Email, SenhaHash)
                                   VALUES (@NOME, @EMAIL, @SENHA)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 100).Value = nome;
                        cmd.Parameters.Add("@EMAIL", SqlDbType.NVarChar, 150).Value = email;
                        cmd.Parameters.Add("@SENHA", SqlDbType.NVarChar, 255).Value = Hash(senha);
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao cadastrar: " + ex.Message };
            }
        }

        public static UsuarioLogado Autenticar(string email, string senha)
        {
            email = (email ?? "").Trim().ToLowerInvariant();

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT Id, Nome FROM Usuarios WHERE Email = @EMAIL AND SenhaHash = @SENHA", con))
                    {
                        cmd.Parameters.Add("@EMAIL", SqlDbType.NVarChar, 150).Value = email;
                        cmd.Parameters.Add("@SENHA", SqlDbType.NVarChar, 255).Value = Hash(senha);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;

                            return new UsuarioLogado
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = Convert.ToString(reader["Nome"])
                            };
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static Resultado RedefinirSenha(string email, string novaSenha, string confirmar)
        {
            email = (email ?? "").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 4)
                return new Resultado { Erro = "A nova senha deve ter pelo menos 4 caracteres." };

            if (novaSenha != confirmar)
                return new Resultado { Erro = "As senhas nao conferem." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    if (!EmailExiste(con, email))
                        return new Resultado { Erro = "Email nao cadastrado." };

                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Usuarios SET SenhaHash = @SENHA WHERE Email = @EMAIL", con))
                    {
                        cmd.Parameters.Add("@SENHA", SqlDbType.NVarChar, 255).Value = Hash(novaSenha);
                        cmd.Parameters.Add("@EMAIL", SqlDbType.NVarChar, 150).Value = email;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao redefinir: " + ex.Message };
            }
        }
    }
}
