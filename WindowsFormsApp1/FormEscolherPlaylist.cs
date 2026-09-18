using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class FormEscolherPlaylist : Form
    {
        private readonly int _usuarioId;
        private readonly string _nomeMusica;
        private ListBox lstPlaylists;
        private Button btnAdicionar;
        private Button btnNova;
        private Label lblStatus;

        public int PlaylistEscolhida { get; private set; }
        public string NomePlaylistEscolhida { get; private set; }

        public FormEscolherPlaylist(int usuarioId, string nomeMusica)
        {
            _usuarioId = usuarioId;
            _nomeMusica = nomeMusica ?? "";
            Inicializar();
        }

        private void Inicializar()
        {
            Text = "Adicionar a playlist";
            ClientSize = new Size(420, 400);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(13, 7, 20);
            Font = new Font("Segoe UI", 9F);

            var lblTitulo = new Label
            {
                Text = "Adicionar musica a playlist",
                ForeColor = Color.FromArgb(168, 85, 247),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(25, 20)
            };

            var lblMusica = new Label
            {
                Text = "Musica: " + _nomeMusica,
                ForeColor = Color.White,
                AutoSize = true,
                MaximumSize = new Size(370, 40),
                Location = new Point(25, 55)
            };

            var lblEscolha = new Label
            {
                Text = "Escolha uma playlist:",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(25, 100)
            };

            lstPlaylists = new ListBox
            {
                Location = new Point(25, 125),
                Size = new Size(370, 150),
                BackColor = Color.FromArgb(28, 16, 42),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            lstPlaylists.SelectedIndexChanged += LstPlaylists_SelectedIndexChanged;

            btnNova = CriarBotao("＋ Nova playlist", new Point(25, 290), Color.FromArgb(45, 20, 65));
            btnNova.Click += BtnNova_Click;

            btnAdicionar = CriarBotao("Adicionar", new Point(220, 340), Color.FromArgb(124, 58, 237));
            btnAdicionar.Click += BtnAdicionar_Click;
            btnAdicionar.Enabled = false;

            var btnCancelar = CriarBotao("Cancelar", new Point(320, 340), Color.FromArgb(45, 20, 65));
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.HotPink,
                AutoSize = true,
                Location = new Point(25, 330),
                MaximumSize = new Size(370, 30)
            };

            Controls.Add(lblTitulo);
            Controls.Add(lblMusica);
            Controls.Add(lblEscolha);
            Controls.Add(lstPlaylists);
            Controls.Add(btnNova);
            Controls.Add(btnAdicionar);
            Controls.Add(btnCancelar);
            Controls.Add(lblStatus);

            CarregarPlaylists();
        }

        private Button CriarBotao(string texto, Point local, Color cor)
        {
            var botao = new Button
            {
                Text = texto,
                Location = local,
                Size = new Size(90, 32),
                BackColor = cor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            Tema.Arredondar(botao, 16);
            return botao;
        }

        private void CarregarPlaylists()
        {
            lstPlaylists.Items.Clear();

            try
            {
                DataTable tabela = PlaylistDAO.ListarPorUsuario(_usuarioId);

                if (tabela.Rows.Count == 0)
                {
                    lstPlaylists.Items.Add("Voce ainda nao tem playlists.");
                    return;
                }

                foreach (DataRow linha in tabela.Rows)
                {
                    int id = Convert.ToInt32(linha["Id"]);
                    string nome = Convert.ToString(linha["Nome"]);
                    lstPlaylists.Items.Add(new ItemPlaylist(id, nome));
                }
            }
            catch (Exception ex)
            {
                lstPlaylists.Items.Add("Erro ao carregar playlists: " + ex.Message);
            }
        }

        private void LstPlaylists_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAdicionar.Enabled = lstPlaylists.SelectedItem is ItemPlaylist;
        }

        private void BtnNova_Click(object sender, EventArgs e)
        {
            using (var form = new FormNovaPlaylist(_usuarioId))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    CarregarPlaylists();
                    lblStatus.Text = "Playlist criada: " + form.NomeCriado;
                }
            }
        }

        private void BtnAdicionar_Click(object sender, EventArgs e)
        {
            if (!(lstPlaylists.SelectedItem is ItemPlaylist item))
            {
                lblStatus.Text = "Selecione uma playlist.";
                return;
            }

            PlaylistEscolhida = item.Id;
            NomePlaylistEscolhida = item.Nome;
            DialogResult = DialogResult.OK;
            Close();
        }

        private class ItemPlaylist
        {
            public int Id;
            public string Nome;

            public ItemPlaylist(int id, string nome)
            {
                Id = id;
                Nome = nome;
            }

            public override string ToString() => Nome;
        }
    }
}
