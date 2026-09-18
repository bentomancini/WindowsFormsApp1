using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class FavoritoDAO
    {
        public class Resultado
        {
            public bool Ok;
            public string Erro;
        }

        public static Resultado Adicionar(int usuarioId, int musicaId)
        {
            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        @"IF NOT EXISTS (SELECT 1 FROM Favoritos WHERE UsuarioId = @UID AND MusicaId = @MID)
                          BEGIN
                              INSERT INTO Favoritos (UsuarioId, MusicaId) VALUES (@UID, @MID);
                          END", con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@MID", SqlDbType.Int).Value = musicaId;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao favoritar: " + ex.Message };
            }
        }

        public static Resultado Remover(int usuarioId, int musicaId)
        {
            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Favoritos WHERE UsuarioId = @UID AND MusicaId = @MID", con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;
                        cmd.Parameters.Add("@MID", SqlDbType.Int).Value = musicaId;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao remover favorito: " + ex.Message };
            }
        }

        public static List<SpotifyService.Faixa> ListarPorUsuario(int usuarioId)
        {
            var lista = new List<SpotifyService.Faixa>();

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    string sql = @"SELECT m.Id AS MusicaId, m.Titulo, m.Album, m.DuracaoSegundos,
                                          m.CapaUrl, m.ArquivoUrl, a.Nome AS Artista
                                   FROM Favoritos f
                                   INNER JOIN Musicas m ON m.Id = f.MusicaId
                                   INNER JOIN Artistas a ON a.Id = m.ArtistaId
                                   WHERE f.UsuarioId = @UID";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@UID", SqlDbType.Int).Value = usuarioId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new SpotifyService.Faixa
                                {
                                    MusicaId = Convert.ToInt32(reader["MusicaId"]),
                                    Nome = Convert.ToString(reader["Titulo"]),
                                    Artistas = Convert.ToString(reader["Artista"]),
                                    Album = reader["Album"] == DBNull.Value
                                        ? null
                                        : Convert.ToString(reader["Album"]),
                                    ImagemUrl = reader["CapaUrl"] == DBNull.Value
                                        ? null
                                        : Convert.ToString(reader["CapaUrl"]),
                                    PreviewUrl = reader["ArquivoUrl"] == DBNull.Value
                                        ? null
                                        : Convert.ToString(reader["ArquivoUrl"]),
                                    DuracaoSegundos = reader["DuracaoSegundos"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(reader["DuracaoSegundos"])
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
