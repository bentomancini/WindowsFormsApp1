using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class ControleInicio : UserControl
    {
        private readonly int _usuarioId;
        private FlowLayoutPanel flpScroll;
        private Label lblCarregando;
        private Label lblBemVindo;
        private bool _carregado;

        // Eventos disparados ao clicar num card de artista ou de album.
        public event Action<SpotifyService.Artista> ArtistaSolicitado;
        public event Action<string> AlbumSolicitado;

        public ControleInicio(int usuarioId, string nomeUsuario)
        {
            _usuarioId = usuarioId;
            Inicializar(nomeUsuario);
        }

        private void Inicializar(string nomeUsuario)
        {
            BackColor = Color.FromArgb(13, 7, 20);
            Font = new Font("Segoe UI", 9F);

            // Saudacao no topo.
            lblBemVindo = new Label
            {
                Text = ObterSaudacao() + ", "
                    + (string.IsNullOrWhiteSpace(nomeUsuario) ? "Usuario" : nomeUsuario) + "!",
                ForeColor = Color.FromArgb(168, 85, 247),
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 8, 0, 14),
                BackColor = Color.Transparent
            };

            // Container rolavel com as secoes de destaque.
            flpScroll = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.Transparent,
                Padding = new Padding(12, 0, 12, 12)
            };

            // Mensagem de carregamento.
            lblCarregando = new Label
            {
                Text = "Carregando destaques...",
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Padding = new Padding(0, 20, 0, 0)
            };
            flpScroll.Controls.Add(lblCarregando);

            Controls.Add(flpScroll);

            Resize += (s, e) => ReposicionarSessoes();
        }

        public void Atualizar()
        {
            if (!_carregado)
            {
                _carregado = true;
                CarregarDestaquesAsync();
            }
        }

        public void AtualizarSaudacao(string nomeUsuario)
        {
            if (lblBemVindo == null)
                return;
            lblBemVindo.Text = ObterSaudacao() + ", "
                + (string.IsNullOrWhiteSpace(nomeUsuario) ? "Usuario" : nomeUsuario) + "!";
        }

        private async void CarregarDestaquesAsync()
        {
            List<SpotifyService.Artista> artistas = null;
            List<SpotifyService.Faixa> albuns = null;

            try
            {
                var t1 = SpotifyService.BuscarArtistasDestaqueAsync(8);
                var t2 = SpotifyService.BuscarAlbunsDestaqueAsync(6);
                await Task.WhenAll(t1, t2);
                artistas = t1.Result;
                albuns = t2.Result;
            }
            catch
            {
            }

            flpScroll.SuspendLayout();
            flpScroll.Controls.Clear();

            // Sessao: Artistas em destaque.
            if (artistas != null && artistas.Count > 0)
            {
                var lblSecArtistas = CriarTituloSecao("Artistas em destaque");
                flpScroll.Controls.Add(lblSecArtistas);

                var flpArtistas = CriarGradeDestaque();
                foreach (var artista in artistas)
                {
                    flpArtistas.Controls.Add(
                        CriarCardDestaque(
                            artista.ImagemUrl,
                            artista.Nome,
                            null,
                            (s, e) => ArtistaSolicitado?.Invoke(artista)));
                }
                flpScroll.Controls.Add(flpArtistas);
            }

            // Sessao: Albuns em destaque.
            if (albuns != null && albuns.Count > 0)
            {
                var lblSecAlbuns = CriarTituloSecao("Albuns em destaque");
                flpScroll.Controls.Add(lblSecAlbuns);

                var flpAlbuns = CriarGradeDestaque();
                foreach (var album in albuns)
                {
                    flpAlbuns.Controls.Add(
                        CriarCardDestaque(
                            album.ImagemUrl,
                            album.Nome,
                            album.Artistas,
                            (s, e) => AlbumSolicitado?.Invoke(
                                album.Album + " " + album.Artistas)));
                }
                flpScroll.Controls.Add(flpAlbuns);
            }

            if ((artistas == null || artistas.Count == 0)
                && (albuns == null || albuns.Count == 0))
            {
                flpScroll.Controls.Add(new Label
                {
                    Text = "Nenhum destaque disponivel no momento.",
                    ForeColor = Color.Silver,
                    AutoSize = true,
                    Padding = new Padding(0, 20, 0, 0)
                });
            }

            flpScroll.ResumeLayout(true);
            ReposicionarSessoes();
            Tema.Aplicar(this);
        }

        private static Label CriarTituloSecao(string texto)
        {
            return new Label
            {
                Text = texto,
                ForeColor = Color.FromArgb(168, 85, 247),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 18, 0, 10),
                BackColor = Color.Transparent
            };
        }

        private FlowLayoutPanel CriarGradeDestaque()
        {
            return new FlowLayoutPanel
            {
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 4)
            };
        }

        private Control CriarCardDestaque(
            string imagemUrl, string titulo, string subtitulo, EventHandler clique)
        {
            var card = new Panel
            {
                Width = 150,
                Height = 190,
                Margin = new Padding(6),
                BackColor = Color.FromArgb(28, 16, 42),
                Padding = new Padding(6),
                Cursor = Cursors.Hand
            };

            var picCapa = new PictureBox
            {
                Location = new Point(15, 10),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(45, 20, 65),
                Cursor = Cursors.Hand
            };

            if (!string.IsNullOrWhiteSpace(imagemUrl))
            {
                try { picCapa.LoadAsync(imagemUrl); } catch { }
            }

            var lblTitulo = new Label
            {
                Text = titulo ?? "",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                MaximumSize = new Size(138, 18),
                AutoEllipsis = true,
                Location = new Point(6, 138),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            Control btnCtrl = card;

            card.Controls.Add(picCapa);
            card.Controls.Add(lblTitulo);

            if (!string.IsNullOrWhiteSpace(subtitulo))
            {
                var lblSub = new Label
                {
                    Text = subtitulo,
                    ForeColor = Color.Silver,
                    Font = new Font("Segoe UI", 8F),
                    AutoSize = true,
                    MaximumSize = new Size(138, 16),
                    AutoEllipsis = true,
                    Location = new Point(6, 160),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };
                card.Controls.Add(lblSub);
            }

            // Deixa todo o card clicavel.
            card.Click += clique;
            picCapa.Click += clique;
            lblTitulo.Click += clique;
            foreach (Control c in card.Controls)
                c.Click += clique;

            return card;
        }

        private void ReposicionarSessoes()
        {
            // Ajusta a largura dos containers de grade para preencher o flpScroll,
            // fazendo o wrap de cards reagir ao redimensionar.
            if (flpScroll == null)
                return;

            int largura = flpScroll.ClientSize.Width - flpScroll.Padding.Horizontal;

            foreach (Control ctrl in flpScroll.Controls)
            {
                if (ctrl is FlowLayoutPanel flp && flp.AutoSize)
                {
                    flp.Width = Math.Max(100, largura);
                }
            }
        }

        private static string ObterSaudacao()
        {
            int hora = DateTime.Now.Hour;
            if (hora >= 5 && hora < 12) return "Bom dia";
            if (hora >= 12 && hora < 18) return "Boa tarde";
            return "Boa noite";
        }
    }
}
