using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class MusicaDAO
    {
        public class Resultado
        {
            public bool Ok;
            public string Erro;
            public int IdGerado;
        }

        public static Resultado Inserir(string titulo, int duracaoSegundos, int idArtista,
                                        string album = null, string capaUrl = null, string arquivoUrl = null)
        {
            titulo = (titulo ?? "").Trim();
            album = (album ?? "").Trim();
            capaUrl = (capaUrl ?? "").Trim();
            arquivoUrl = (arquivoUrl ?? "").Trim();

            if (string.IsNullOrWhiteSpace(titulo))
                return new Resultado { Erro = "Informe o titulo da musica." };

            if (titulo.Length > 200)
                return new Resultado { Erro = "O titulo deve ter no maximo 200 caracteres." };

            if (duracaoSegundos <= 0)
                return new Resultado { Erro = "A duracao deve ser maior que zero segundos." };

            if (string.IsNullOrWhiteSpace(arquivoUrl))
                return new Resultado { Erro = "Informe o arquivo da musica." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    if (!RegistroExiste(con, "Artistas", "Id", idArtista))
                        return new Resultado { Erro = "Artista nao encontrado (id " + idArtista + ")." };

                    string sql = @"INSERT INTO Musicas (ArtistaId, Titulo, Album, DuracaoSegundos, CapaUrl, ArquivoUrl)
                                   OUTPUT INSERTED.Id
                                   VALUES (@ID_ARTISTA, @TITULO, @ALBUM, @DURACAO, @CAPA, @ARQUIVO)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@ID_ARTISTA", SqlDbType.Int).Value = idArtista;
                        cmd.Parameters.Add("@TITULO", SqlDbType.NVarChar, 200).Value = titulo;
                        cmd.Parameters.Add("@ALBUM", SqlDbType.NVarChar, 200).Value =
                            string.IsNullOrEmpty(album) ? (object)DBNull.Value : album;
                        cmd.Parameters.Add("@DURACAO", SqlDbType.Int).Value = duracaoSegundos;
                        cmd.Parameters.Add("@CAPA", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrEmpty(capaUrl) ? (object)DBNull.Value : capaUrl;
                        cmd.Parameters.Add("@ARQUIVO", SqlDbType.NVarChar, 500).Value = arquivoUrl;

                        return new Resultado { Ok = true, IdGerado = Convert.ToInt32(cmd.ExecuteScalar()) };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao inserir musica: " + ex.Message };
            }
        }

        private static bool RegistroExiste(SqlConnection con, string tabela, string colunaId, int id)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(1) FROM " + tabela + " WHERE " + colunaId + " = @ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static int ObterOuInserir(SpotifyService.Faixa faixa, int idArtista)
        {
            if (faixa == null || string.IsNullOrWhiteSpace(faixa.Nome))
                return -1;

            string titulo = (faixa.Nome ?? "").Trim();
            string album = (faixa.Album ?? "").Trim();
            string capa = (faixa.ImagemUrl ?? "").Trim();
            string preview = (faixa.PreviewUrl ?? "").Trim();

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT TOP 1 Id FROM Musicas WHERE ArtistaId = @ID_ARTISTA AND Titulo = @TITULO", con))
                    {
                        cmd.Parameters.Add("@ID_ARTISTA", SqlDbType.Int).Value = idArtista;
                        cmd.Parameters.Add("@TITULO", SqlDbType.NVarChar, 200).Value = titulo;

                        object existente = cmd.ExecuteScalar();
                        if (existente != null && !(existente is DBNull))
                            return Convert.ToInt32(existente);
                    }

                    string sql = @"INSERT INTO Musicas (ArtistaId, Titulo, Album, DuracaoSegundos, CapaUrl, ArquivoUrl)
                                   OUTPUT INSERTED.Id
                                   VALUES (@ID_ARTISTA, @TITULO, @ALBUM, @DURACAO, @CAPA, @ARQUIVO)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@ID_ARTISTA", SqlDbType.Int).Value = idArtista;
                        cmd.Parameters.Add("@TITULO", SqlDbType.NVarChar, 200).Value = titulo;
                        cmd.Parameters.Add("@ALBUM", SqlDbType.NVarChar, 200).Value =
                            string.IsNullOrEmpty(album) ? (object)DBNull.Value : album;
                        cmd.Parameters.Add("@DURACAO", SqlDbType.Int).Value = faixa.DuracaoSegundos;
                        cmd.Parameters.Add("@CAPA", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrEmpty(capa) ? (object)DBNull.Value : capa;
                        cmd.Parameters.Add("@ARQUIVO", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrEmpty(preview) ? "" : preview;

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        public static DataTable ListarTodas()
        {
            var tabela = new DataTable();

            using (SqlConnection con = Banco.ObterConexao())
            {
                con.Open();

                string sql = @"SELECT m.Id AS id_musica, m.Titulo, m.DuracaoSegundos,
                                      a.Nome AS artista
                               FROM Musicas m
                               JOIN Artistas a ON a.Id = m.ArtistaId";

                using (SqlDataAdapter ad = new SqlDataAdapter(sql, con))
                {
                    ad.Fill(tabela);
                }
            }

            return tabela;
        }

        public static List<SpotifyService.Faixa> ListarFaixas()
        {
            var lista = new List<SpotifyService.Faixa>();

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    string sql = @"SELECT m.Id AS MusicaId, m.Titulo, m.Album, m.DuracaoSegundos,
                                          m.CapaUrl, m.ArquivoUrl, a.Nome AS Artista
                                   FROM Musicas m
                                   JOIN Artistas a ON a.Id = m.ArtistaId";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new SpotifyService.Faixa
                                {
                                    MusicaId = Convert.ToInt32(reader["MusicaId"]),
                                    Nome = reader["Titulo"] == DBNull.Value
                                        ? "" : Convert.ToString(reader["Titulo"]),
                                    Artistas = reader["Artista"] == DBNull.Value
                                        ? "" : Convert.ToString(reader["Artista"]),
                                    Album = reader["Album"] == DBNull.Value
                                        ? null : Convert.ToString(reader["Album"]),
                                    ImagemUrl = reader["CapaUrl"] == DBNull.Value
                                        ? null : Convert.ToString(reader["CapaUrl"]),
                                    PreviewUrl = reader["ArquivoUrl"] == DBNull.Value
                                        ? null : Convert.ToString(reader["ArquivoUrl"]),
                                    DuracaoSegundos = reader["DuracaoSegundos"] == DBNull.Value
                                        ? 0 : Convert.ToInt32(reader["DuracaoSegundos"])
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
