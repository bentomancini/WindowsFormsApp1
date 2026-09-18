using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class ArtistaDAO
    {
        public class Resultado
        {
            public bool Ok;
            public string Erro;
            public int IdGerado;
        }

        public static Resultado Inserir(string nome, string bio)
        {
            nome = (nome ?? "").Trim();
            bio = (bio ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nome))
                return new Resultado { Erro = "Informe o nome do artista." };

            if (nome.Length > 150)
                return new Resultado { Erro = "O nome do artista deve ter no maximo 150 caracteres." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    string sql = @"INSERT INTO Artistas (Nome, Bio)
                                   OUTPUT INSERTED.Id
                                   VALUES (@NOME, @BIO)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 150).Value = nome;
                        cmd.Parameters.Add("@BIO", SqlDbType.NVarChar).Value =
                            string.IsNullOrEmpty(bio) ? (object)DBNull.Value : bio;

                        return new Resultado { Ok = true, IdGerado = Convert.ToInt32(cmd.ExecuteScalar()) };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao inserir artista: " + ex.Message };
            }
        }

        public static int ObterOuInserir(string nome)
        {
            nome = (nome ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nome))
                return -1;

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT TOP 1 Id FROM Artistas WHERE Nome = @NOME", con))
                    {
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 150).Value = nome;

                        object existente = cmd.ExecuteScalar();
                        if (existente != null && !(existente is DBNull))
                            return Convert.ToInt32(existente);
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO Artistas (Nome)
                          OUTPUT INSERTED.Id
                          VALUES (@NOME)", con))
                    {
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 150).Value = nome;
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        public static DataTable Listar()
        {
            var tabela = new DataTable();

            using (SqlConnection con = Banco.ObterConexao())
            {
                con.Open();

                string sql = @"SELECT Id, Nome, Bio FROM Artistas ORDER BY Nome";

                using (SqlDataAdapter ad = new SqlDataAdapter(sql, con))
                {
                    ad.Fill(tabela);
                }
            }

            return tabela;
        }

        // ---- Favoritos de artistas ----

        // Garante que a tabela ArtistasFavoritos exista (criada sob demanda).
        private static void GarantirTabelaFavoritos(SqlConnection con)
        {
            string sql = @"IF OBJECT_ID(N'dbo.ArtistasFavoritos', N'U') IS NULL
                           BEGIN
                               CREATE TABLE dbo.ArtistasFavoritos
                               (
                                   UsuarioId  INT NOT NULL,
                                   ArtistaId  INT NOT NULL,
                                   Nome       NVARCHAR(300) NOT NULL,
                                   ImagemUrl  NVARCHAR(500) NULL,
                                   DataFavorito DATETIME NOT NULL CONSTRAINT DF_ArtFav DEFAULT GETDATE(),
                                   CONSTRAINT PK_ArtistasFavoritos PRIMARY KEY (UsuarioId, ArtistaId)
                               );
                           END";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static Resultado Favoritar(int usuarioId, SpotifyService.Artista artista)
        {
            if (usuarioId <= 0)
                return new Resultado { Erro = "Usuario nao identificado." };
            if (artista == null || string.IsNullOrWhiteSpace(artista.Nome))
                return new Resultado { Erro = "Artista invalido." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();
                    GarantirTabelaFavoritos(con);

                    int idArtista = ObterOuInserir(artista.Nome);

                    using (SqlCommand cmd = new SqlCommand(
                        @"IF NOT EXISTS (SELECT 1 FROM ArtistasFavoritos WHERE UsuarioId = @UID AND ArtistaId = @AID)
                          BEGIN
                              INSERT INTO ArtistasFavoritos (UsuarioId, ArtistaId, Nome, ImagemUrl)
                              VALUES (@UID, @AID, @NOME, @IMG)
                          END", con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@AID", SqlDbType.Int).Value = idArtista;
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 300).Value = artista.Nome;
                        cmd.Parameters.Add("@IMG", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrWhiteSpace(artista.ImagemUrl) ? (object)DBNull.Value : artista.ImagemUrl;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao favoritar artista: " + ex.Message };
            }
        }

        public static Resultado RemoverFavorito(int usuarioId, string nomeArtista)
        {
            if (usuarioId <= 0)
                return new Resultado { Erro = "Usuario nao identificado." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        @"IF OBJECT_ID(N'dbo.ArtistasFavoritos', N'U') IS NOT NULL
                          BEGIN
                              DELETE FROM ArtistasFavoritos
                              WHERE UsuarioId = @UID AND Nome = @NOME
                          END", con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 300).Value = nomeArtista;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao remover artista favorito: " + ex.Message };
            }
        }

        public static bool EhFavorito(int usuarioId, string nomeArtista)
        {
            if (usuarioId <= 0)
                return false;

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        @"IF OBJECT_ID(N'dbo.ArtistasFavoritos', N'U') IS NOT NULL
                          BEGIN
                              SELECT 1 FROM ArtistasFavoritos
                              WHERE UsuarioId = @UID AND Nome = @NOME
                          END", con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 300).Value = nomeArtista;
                        return cmd.ExecuteScalar() != null;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static List<SpotifyService.Artista> ListarFavoritos(int usuarioId)
        {
            var lista = new List<SpotifyService.Artista>();

            if (usuarioId <= 0)
                return lista;

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        @"IF OBJECT_ID(N'dbo.ArtistasFavoritos', N'U') IS NOT NULL
                          BEGIN
                              SELECT Nome, ImagemUrl FROM ArtistasFavoritos
                              WHERE UsuarioId = @UID ORDER BY DataFavorito DESC
                          END", con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nome = Convert.ToString(reader["Nome"]);
                                if (string.IsNullOrWhiteSpace(nome))
                                    continue;

                                lista.Add(new SpotifyService.Artista
                                {
                                    Nome = nome,
                                    ImagemUrl = reader["ImagemUrl"] == DBNull.Value
                                        ? null
                                        : Convert.ToString(reader["ImagemUrl"])
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return lista;
        }
    }
}
