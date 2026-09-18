using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class PlaylistDAO
    {
        public class Resultado
        {
            public bool Ok;
            public string Erro;
            public int IdGerado;
        }

        public static Resultado Inserir(string nome, string descricao, int idUsuario, bool publica = false)
        {
            nome = (nome ?? "").Trim();
            descricao = (descricao ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nome))
                return new Resultado { Erro = "Informe o nome da playlist." };

            if (nome.Length > 150)
                return new Resultado { Erro = "O nome deve ter no maximo 150 caracteres." };

            if (descricao.Length > 500)
                return new Resultado { Erro = "A descricao deve ter no maximo 500 caracteres." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(1) FROM Usuarios WHERE Id = @ID_USUARIO", con))
                    {
                        cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                            return new Resultado { Erro = "Usuario nao encontrado (id " + idUsuario + ")." };
                    }

                    string sql = @"INSERT INTO Playlists (UsuarioId, Nome, Descricao, Publica)
                                   OUTPUT INSERTED.Id
                                   VALUES (@ID_USUARIO, @NOME, @DESCRICAO, @PUBLICA)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.Int).Value = idUsuario;
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 150).Value = nome;
                        cmd.Parameters.Add("@DESCRICAO", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrEmpty(descricao) ? (object)DBNull.Value : descricao;
                        cmd.Parameters.Add("@PUBLICA", SqlDbType.Bit).Value = publica;

                        return new Resultado { Ok = true, IdGerado = Convert.ToInt32(cmd.ExecuteScalar()) };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao inserir playlist: " + ex.Message };
            }
        }

        public static DataTable ListarPorUsuario(int idUsuario)
        {
            var tabela = new DataTable();

            using (SqlConnection con = Banco.ObterConexao())
            {
                con.Open();

                string sql = @"SELECT Id, Nome, Descricao
                               FROM Playlists
                               WHERE UsuarioId = @ID_USUARIO
                               ORDER BY Nome";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(tabela);
                    }
                }
            }

            return tabela;
        }

        // Lista as playlists do usuario com a quantidade de musicas e a capa da
        // primeira musica de cada uma (para montar os cards do painel lateral).
        public static DataTable ListarComResumo(int idUsuario)
        {
            var tabela = new DataTable();

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    string sql = @"SELECT p.Id, p.Nome,
                                          (SELECT COUNT(*) FROM PlaylistMusicas pm
                                           WHERE pm.PlaylistId = p.Id) AS QuantidadeMusicas,
                                          (SELECT TOP 1 m.CapaUrl FROM PlaylistMusicas pm2
                                           INNER JOIN Musicas m ON m.Id = pm2.MusicaId
                                           WHERE pm2.PlaylistId = p.Id
                                           ORDER BY pm2.Ordem) AS CapaUrl
                                   FROM Playlists p
                                   WHERE p.UsuarioId = @ID_USUARIO
                                   ORDER BY p.Nome";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                        using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(tabela);
                        }
                    }
                }
            }
            catch
            {
            }

            return tabela;
        }

        public static Resultado Renomear(int idPlaylist, string nome)
        {
            nome = (nome ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nome))
                return new Resultado { Erro = "Informe o novo nome da playlist." };

            if (nome.Length > 150)
                return new Resultado { Erro = "O nome deve ter no maximo 150 caracteres." };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Playlists SET Nome = @NOME WHERE Id = @ID", con))
                    {
                        cmd.Parameters.Add("@NOME", SqlDbType.NVarChar, 150).Value = nome;
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = idPlaylist;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao renomear playlist: " + ex.Message };
            }
        }

        public static Resultado AdicionarMusica(int idPlaylist, int idMusica)
        {
            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    int proximaOrdem;
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT ISNULL(MAX(Ordem), -1) + 1 FROM PlaylistMusicas WHERE PlaylistId = @PID", con))
                    {
                        cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;
                        proximaOrdem = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        @"IF NOT EXISTS (SELECT 1 FROM PlaylistMusicas
                                         WHERE PlaylistId = @PID AND MusicaId = @MID)
                          BEGIN
                              INSERT INTO PlaylistMusicas (PlaylistId, MusicaId, Ordem)
                              VALUES (@PID, @MID, @ORDEM);
                          END", con))
                    {
                        cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;
                        cmd.Parameters.Add("@MID", SqlDbType.Int).Value = idMusica;
                        cmd.Parameters.Add("@ORDEM", SqlDbType.Int).Value = proximaOrdem;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao adicionar musica na playlist: " + ex.Message };
            }
        }

        public static Resultado RemoverMusica(int idPlaylist, int idMusica)
        {
            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM PlaylistMusicas WHERE PlaylistId = @PID AND MusicaId = @MID", con))
                    {
                        cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;
                        cmd.Parameters.Add("@MID", SqlDbType.Int).Value = idMusica;
                        cmd.ExecuteNonQuery();
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao remover musica da playlist: " + ex.Message };
            }
        }

        public static List<SpotifyService.Faixa> ListarMusicas(int idPlaylist)
        {
            var lista = new List<SpotifyService.Faixa>();

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();

                    string sql = @"SELECT m.Id AS MusicaId, m.Titulo, m.Album, m.DuracaoSegundos,
                                          m.CapaUrl, m.ArquivoUrl, a.Nome AS Artista
                                   FROM PlaylistMusicas pm
                                   INNER JOIN Musicas m ON m.Id = pm.MusicaId
                                   INNER JOIN Artistas a ON a.Id = m.ArtistaId
                                   WHERE pm.PlaylistId = @PID
                                   ORDER BY pm.Ordem";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;

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

        public static Resultado Reordenar(int idPlaylist, List<int> idsMusicasNaOrdem)
        {
            if (idsMusicasNaOrdem == null || idsMusicasNaOrdem.Count == 0)
                return new Resultado { Ok = true };

            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < idsMusicasNaOrdem.Count; i++)
                            {
                                using (SqlCommand cmd = new SqlCommand(
                                    "UPDATE PlaylistMusicas SET Ordem = @ORDEM " +
                                    "WHERE PlaylistId = @PID AND MusicaId = @MID", con, trans))
                                {
                                    cmd.Parameters.Add("@ORDEM", SqlDbType.Int).Value = i;
                                    cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;
                                    cmd.Parameters.Add("@MID", SqlDbType.Int).Value = idsMusicasNaOrdem[i];
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao reordenar playlist: " + ex.Message };
            }
        }

        public static Resultado Excluir(int idPlaylist)
        {
            try
            {
                using (SqlConnection con = Banco.ObterConexao())
                {
                    con.Open();
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "DELETE FROM PlaylistMusicas WHERE PlaylistId = @PID", con, trans))
                            {
                                cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                "DELETE FROM Playlists WHERE Id = @PID", con, trans))
                            {
                                cmd.Parameters.Add("@PID", SqlDbType.Int).Value = idPlaylist;
                                cmd.ExecuteNonQuery();
                            }

                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }

                return new Resultado { Ok = true };
            }
            catch (Exception ex)
            {
                return new Resultado { Erro = "Erro ao excluir playlist: " + ex.Message };
            }
        }
    }
}
