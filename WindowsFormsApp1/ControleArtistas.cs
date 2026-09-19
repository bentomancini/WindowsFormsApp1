using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class ControleArtistas : UserControl
    {
        public ControleArtistas(int usuarioId)
        {
            _usuarioId = usuarioId;
            Inicializar();
        }

        // Disparado quando o usuario quer tocar uma das musicas do artista.
        public event Action<SpotifyService.Faixa> MusicaSolicitada;

        private int _usuarioId;

        private TextBox txtBuscaArtista;
        private Button btnBuscar;
        private Button btnFavoritarSelecionado;
        private Button btnFavoritos;
        private ListView lstArtistas;
        private ImageList _imagens;

        // Painel de detalhe do artista.
        private ScrollableControl pnlDetalhe;
        private PictureBox picArtista;
        private Label lblNomeArtista;
        private Label lblInfoArtista;
        private FlowLayoutPanel flpTopMusicas;
        private Button btnVoltar;
        private Button btnFavorito;
        private SpotifyService.Artista _artistaAtual;
        private bool _exibindoDetalhe;

        private void Inicializar()
        {
            BackColor = Color.FromArgb(13, 7, 20);
            Font = new Font("Segoe UI", 9F);

            // ---- Barra superior (busca + botoes) ----
            var pnlTopo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(6, 8, 6, 4),
                BackColor = Color.FromArgb(20, 11, 30)
            };

            btnBuscar = NovoBotaoTopo("Buscar", Color.FromArgb(124, 58, 237), 86, BtnBuscar_Click);
            btnFavoritarSelecionado = NovoBotaoTopo("♡ Favoritar", Color.FromArgb(45, 20, 65), 110, BtnFavoritarSelecionado_Click);
            btnFavoritos = NovoBotaoTopo("Favoritos", Color.FromArgb(45, 20, 65), 96, BtnFavoritos_Click);

            txtBuscaArtista = new TextBox
            {
                Location = new Point(6, 8),
                Size = new Size(200, 26),
                BackColor = Color.FromArgb(28, 16, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };
            txtBuscaArtista.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    BuscarArtistas(txtBuscaArtista.Text.Trim());
            };

            pnlTopo.Controls.Add(txtBuscaArtista);
            pnlTopo.Controls.Add(btnBuscar);
            pnlTopo.Controls.Add(btnFavoritarSelecionado);
            pnlTopo.Controls.Add(btnFavoritos);
            ReposicionarBotoesTopo();

            Controls.Add(pnlTopo);
            Resize += (s, e) => ReposicionarBotoesTopo();

            // ---- Lista de artistas ----
            _imagens = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(56, 56)
            };

            lstArtistas = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(28, 16, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12.5F),
                SmallImageList = _imagens,
                HeaderStyle = ColumnHeaderStyle.None
            };
            lstArtistas.Columns.Add("Artista", 360);
            lstArtistas.DoubleClick += LstArtistas_DoubleClick;
            lstArtistas.SelectedIndexChanged += LstArtistas_SelectedIndexChanged;

            Controls.Add(lstArtistas);

            CriarPainelDetalhe();
            Controls.Add(pnlDetalhe);
            pnlDetalhe.Visible = false;

            Load += (s, e) => BuscarArtistas("Brasil");
        }

        private Button NovoBotaoTopo(string texto, Color cor, int largura, EventHandler clique)
        {
            var botao = new Button
            {
                Text = texto,
                Size = new Size(largura, 26),
                BackColor = cor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            botao.FlatAppearance.BorderSize = 0;
            Tema.Arredondar(botao, 13);
            botao.Click += clique;
            return botao;
        }

        private void ReposicionarBotoesTopo()
        {
            // Recalcula posicoes dos botoes/txtBusca da barra quando o tamanho muda.
            if (btnBuscar == null || btnFavoritos == null || btnFavoritarSelecionado == null)
                return;

            int direita = this.Width - 6;
            btnFavoritos.Location = new Point(direita - btnFavoritos.Width, 8);
            btnFavoritarSelecionado.Location = new Point(btnFavoritos.Left - btnFavoritarSelecionado.Width - 4, 8);
            btnBuscar.Location = new Point(btnFavoritarSelecionado.Left - btnBuscar.Width - 4, 8);
            txtBuscaArtista.Width = Math.Max(60, btnBuscar.Left - 16);
        }

        private void CriarPainelDetalhe()
        {
            pnlDetalhe = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 11, 30),
                Visible = false
            };

            // Barra superior com o botao voltar (sempre visivel).
            var pnlTopoDetalhe = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10, 6, 10, 4),
                BackColor = Color.FromArgb(20, 11, 30)
            };

            btnVoltar = new Button
            {
                Text = "< Voltar",
                Location = new Point(10, 6),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(45, 20, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnVoltar.FlatAppearance.BorderSize = 0;
            Tema.Arredondar(btnVoltar, 14);
            btnVoltar.Click += (s, e) => MostrarLista();
            pnlTopoDetalhe.Controls.Add(btnVoltar);
            pnlDetalhe.Controls.Add(pnlTopoDetalhe);

            // Usa um TableLayoutPanel (grid 2 colunas) para separar o card do artista
            // (esquerda) das musicas (direita), sem sobreposicao por dock.
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.FromArgb(20, 11, 30)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Coluna 0: card do artista (foto + nome + info + favoritar).
            var pnlEsquerda = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(28, 16, 42)
            };

            picArtista = new PictureBox
            {
                Location = new Point(60, 45),
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(28, 16, 42)
            };
            pnlEsquerda.Controls.Add(picArtista);

            lblNomeArtista = new Label
            {
                Location = new Point(5, 205),
                Size = new Size(260, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.FromArgb(28, 16, 42)
            };
            pnlEsquerda.Controls.Add(lblNomeArtista);

            lblInfoArtista = new Label
            {
                Location = new Point(5, 234),
                Size = new Size(260, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(28, 16, 42)
            };
            pnlEsquerda.Controls.Add(lblInfoArtista);

            btnFavorito = new Button
            {
                Location = new Point(52, 280),
                Size = new Size(166, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnFavorito.FlatAppearance.BorderSize = 0;
            Tema.Arredondar(btnFavorito, 15);
            btnFavorito.Click += BtnFavorito_Click;
            pnlEsquerda.Controls.Add(btnFavorito);

            grid.Controls.Add(pnlEsquerda, 0, 0);

            // Coluna 1: as 5 musicas.
            var pnlDireita = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 11, 30)
            };

            var lblMusicas = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Text = "10 musicas mais famosas:",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(168, 85, 247),
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(20, 11, 30)
            };

            flpTopMusicas = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.FromArgb(20, 11, 30)
            };

            // Acompanha o tamanho da janela: repassa a largura nova a cada linha.
            flpTopMusicas.Resize += (s, e) =>
            {
                if (flpTopMusicas.ClientSize.Width <= 0)
                    return;
                int larguraNova = Math.Max(280, flpTopMusicas.ClientSize.Width - 12);
                foreach (Control controle in flpTopMusicas.Controls)
                {
                    if (controle.Width != larguraNova)
                        controle.Width = larguraNova;
                }
            };

            // Ordem IMPORTANTE no WinForms: o dock e processado na ordem inversa
            // da adicao. Adiciona primeiro o Fill (flp) e DEPOIS o Top (lblMusicas),
            // para o label reservar o topo e o flp preencher abaixo dele.
            pnlDireita.Controls.Add(flpTopMusicas);
            pnlDireita.Controls.Add(lblMusicas);

            grid.Controls.Add(pnlDireita, 1, 0);

            pnlDetalhe.Controls.Add(grid);
        }

        private void MostrarLista()
        {
            _exibindoDetalhe = false;
            lstArtistas.Visible = true;
            pnlDetalhe.Visible = false;
        }

        // Permite abrir o detalhe de um artista diretamente (ex.: vindo da home).
        public void ExibirArtista(SpotifyService.Artista artista)
        {
            if (artista == null)
                return;
            MostrarDetalhe(artista);
        }

        private void MostrarDetalhe(SpotifyService.Artista artista)
        {
            _exibindoDetalhe = true;
            lstArtistas.Visible = false;
            pnlDetalhe.Visible = true;

            _artistaAtual = artista;
            lblNomeArtista.Text = artista.Nome;

            string generos = artista.Generos != null && artista.Generos.Count > 0
                ? string.Join(", ", artista.Generos.Take(3))
                : "";
            string seguidores = artista.Seguidores > 0
                ? artista.Seguidores.ToString("N0") + " seguidores"
                : "";
            lblInfoArtista.Text = string.Join(Environment.NewLine,
                new[]
                {
                    !string.IsNullOrWhiteSpace(generos) ? generos : "",
                    !string.IsNullOrWhiteSpace(seguidores) ? seguidores : ""
                }.Where(s => !string.IsNullOrWhiteSpace(s)));

            // Foto do artista.
            picArtista.Image = null;
            if (!string.IsNullOrWhiteSpace(artista.ImagemUrl))
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var dados = client.GetByteArrayAsync(artista.ImagemUrl).Result;
                        using (var ms = new System.IO.MemoryStream(dados))
                        {
                            picArtista.Image = new Bitmap(Image.FromStream(ms));
                        }
                    }
                }
                catch
                {
                }
            }

            CarregarTopMusicas(artista.Nome);

            AtualizarBotaoFavorito();
        }

        private async void CarregarTopMusicas(string nomeArtista)
        {
            flpTopMusicas.Controls.Clear();

            var top = await SpotifyService.BuscarTopMusicasArtistaAsync(nomeArtista, 10);

            if (top == null || top.Count == 0)
            {
                var msg = new Label
                {
                    Text = "Nao foi possivel carregar as musicas deste artista na iTunes.",
                    ForeColor = Color.Silver,
                    AutoSize = true,
                    Padding = new Padding(0, 6, 0, 0)
                };
                flpTopMusicas.Controls.Add(msg);
                Tema.Aplicar(flpTopMusicas);
                return;
            }

            int numero = 1;
            foreach (var faixa in top)
            {
                var faixaAtual = faixa;
                var item = CriarLinhaMusica(numero, faixa);
                item.Click += (s, e) => MusicaSolicitada?.Invoke(faixaAtual);
                flpTopMusicas.Controls.Add(item);
                numero++;
            }

            Tema.Aplicar(flpTopMusicas);
        }

        private Control CriarLinhaMusica(int numero, SpotifyService.Faixa faixa)
        {
            int larguraLinha = Math.Max(280, (flpTopMusicas != null ? flpTopMusicas.ClientSize.Width : 480) - 12);

var pnl = new Panel
            {
                Size = new Size(larguraLinha, 60),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var inicio = new Point(0, 6);
                var botoes = new PictureBox
                {
                    Text = "",
                    Size = new Size(46, 46),
                    Location = inicio,
                    Cursor = Cursors.Hand,
                    BackColor = Color.FromArgb(124, 58, 237),
                    Image = DesenharPlay(),
                    SizeMode = PictureBoxSizeMode.CenterImage
                };
                Tema.Arredondar(botoes, 23);
                pnl.Controls.Add(botoes);

                var capa = new PictureBox
                {
                    Size = new Size(46, 46),
                    Location = new Point(56, 6),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.FromArgb(28, 16, 42)
                };
                Tema.Arredondar(capa, 23);
            if (!string.IsNullOrWhiteSpace(faixa.ImagemUrl))
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var dados = client.GetByteArrayAsync(faixa.ImagemUrl).Result;
using (var ms = new System.IO.MemoryStream(dados))
                            {
                                capa.Image = new Bitmap(Image.FromStream(ms));
                            }
                    }
                }
                catch
                {
                }
            }
            pnl.Controls.Add(capa);

            var duracao = new Label
            {
                Text = FormatarDuracao(faixa.DuracaoSegundos),
                Location = new Point(larguraLinha - 60, 18),
                AutoSize = true,
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.Transparent
            };
            pnl.Controls.Add(duracao);

            var txt = new Label
            {
                Text = numero + ".  " + faixa.Nome + "  -  " + faixa.Album,
                Location = new Point(112, 15),
                Size = new Size(larguraLinha - 112 - 65, 34),
                AutoEllipsis = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F),
                BackColor = Color.Transparent
            };
            pnl.Controls.Add(txt);

            foreach (Control c in pnl.Controls)
            {
                c.Click += (s, e) => MusicaSolicitada?.Invoke(faixa);
            }

            // Mantem a linha e as posicoes internas ajustadas ao largar/esticar.
            pnl.Resize += (s, e) =>
            {
                int larguraNova = pnl.Width;
                duracao.Location = new Point(larguraNova - duracao.Width - 6, 18);
                txt.Size = new Size(Math.Max(60, larguraNova - 112 - 65), 34);
            };

            return pnl;
        }

        private void LstArtistas_DoubleClick(object sender, EventArgs e)
        {
            if (lstArtistas.SelectedItems.Count == 0)
                return;

            int idx = lstArtistas.SelectedItems[0].Index;
            if (idx < 0 || idx >= _resultadosBusca.Count)
                return;

            MostrarDetalhe(_resultadosBusca[idx]);
        }

        // Clique simples/teclado tambem abre o detalhe do artista selecionado.
        private void LstArtistas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_carregando || !lstArtistas.Visible || pnlDetalhe.Visible)
                return;
            if (lstArtistas.SelectedItems.Count == 0)
                return;

            int idx = lstArtistas.SelectedItems[0].Index;
            if (idx < 0 || idx >= _resultadosBusca.Count)
                return;

            MostrarDetalhe(_resultadosBusca[idx]);
        }

        private List<SpotifyService.Artista> _resultadosBusca = new List<SpotifyService.Artista>();
        private bool _carregando;

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            BuscarArtistas(txtBuscaArtista.Text.Trim());
        }

        private void AtualizarBotaoFavorito()
        {
            if (_artistaAtual == null)
            {
                btnFavorito.Visible = false;
                return;
            }

            bool ehFavorito = _usuarioId > 0 && ArtistaDAO.EhFavorito(_usuarioId, _artistaAtual.Nome);
            btnFavorito.Text = ehFavorito ? "♥ Desfavoritar" : "♡ Favoritar";
            btnFavorito.BackColor = ehFavorito ? Color.FromArgb(200, 40, 70) : Color.FromArgb(124, 58, 237);
            btnFavorito.ForeColor = Color.White;
            btnFavorito.Visible = _usuarioId > 0;
        }

        private void BtnFavorito_Click(object sender, EventArgs e)
        {
            if (_usuarioId <= 0 || _artistaAtual == null)
                return;

            bool ehFavorito = ArtistaDAO.EhFavorito(_usuarioId, _artistaAtual.Nome);
            var resultado = ehFavorito
                ? ArtistaDAO.RemoverFavorito(_usuarioId, _artistaAtual.Nome)
                : ArtistaDAO.Favoritar(_usuarioId, _artistaAtual);

            if (resultado.Ok)
            {
                AtualizarBotaoFavorito();
            }
            else
            {
                MessageBox.Show(resultado.Erro, "Artistas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnFavoritos_Click(object sender, EventArgs e)
        {
            MostrarFavoritos();
        }

        // Favorita o artista selecionado na lista, sem abrir o detalhe.
        private void BtnFavoritarSelecionado_Click(object sender, EventArgs e)
        {
            if (_usuarioId <= 0)
            {
                MessageBox.Show("Entre no app para favoritar artistas.",
                    "Artistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (lstArtistas.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione um artista na lista para favoritar.",
                    "Artistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idx = lstArtistas.SelectedItems[0].Index;
            if (idx < 0 || idx >= _resultadosBusca.Count)
                return;

            var artista = _resultadosBusca[idx];
            if (artista == null || string.IsNullOrWhiteSpace(artista.Nome))
                return;

            var resultado = ArtistaDAO.Favoritar(_usuarioId, artista);
            if (resultado.Ok)
            {
                MessageBox.Show("Artista adicionado aos favoritos: " + artista.Nome,
                    "Artistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resultado.Erro, "Artistas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void MostrarFavoritos()
        {
            if (_usuarioId <= 0)
            {
                MessageBox.Show("Entre no app para ver seus artistas favoritos.",
                    "Artistas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var favoritos = await System.Threading.Tasks.Task.Run(() => ArtistaDAO.ListarFavoritos(_usuarioId));

            _carregando = true;
            _imagens.Images.Clear();
            lstArtistas.Items.Clear();
            pnlDetalhe.Visible = false;
            lstArtistas.Visible = true;

            if (favoritos == null || favoritos.Count == 0)
            {
                AdicionarMensagem("Voce ainda nao tem artistas favoritos.");
                _carregando = false;
                return;
            }

            _resultadosBusca.Clear();
            _resultadosBusca.AddRange(favoritos);

            foreach (var artista in favoritos)
            {
                var item = new ListViewItem(artista.Nome)
                {
                    ImageIndex = -1
                };

                if (!string.IsNullOrWhiteSpace(artista.ImagemUrl))
                {
                    try
                    {
                        using (var client = new System.Net.Http.HttpClient())
                        {
                            var dados = await client.GetByteArrayAsync(artista.ImagemUrl);
using (var ms = new System.IO.MemoryStream(dados))
                                {
                                    _imagens.Images.Add(new Bitmap(Image.FromStream(ms)));
                                }
                        }
                    }
                    catch
                    {
                        item.ImageIndex = -1;
                    }
                }

                lstArtistas.Items.Add(item);
            }

            _carregando = false;
        }

        private void AdicionarMensagem(string mensagem)
        {
            _imagens.Images.Clear();
            lstArtistas.Items.Clear();
            lstArtistas.Items.Add(new ListViewItem(" " + mensagem) { ForeColor = Color.White });
        }

        private async void BuscarArtistas(string termo)
        {
            _exibindoDetalhe = false;

            if (string.IsNullOrWhiteSpace(termo))
            {
                AdicionarMensagem("Digite o nome de um artista para buscar.");
                return;
            }

            if (!SpotifyService.Configurado)
            {
                AdicionarMensagem("Credenciais do Spotify nao configuradas em SpotifyService.cs.");
                return;
            }

            AdicionarMensagem("Buscando artistas no Spotify...");

            try
            {
                var artistas = await SpotifyService.BuscarArtistasAsync(termo);
                _resultadosBusca.Clear();
                _resultadosBusca.AddRange(artistas ?? new List<SpotifyService.Artista>());

                _carregando = true;
                _imagens.Images.Clear();
                lstArtistas.Items.Clear();

                if (artistas == null || artistas.Count == 0)
                {
                    AdicionarMensagem("Nenhum artista encontrado para \"" + termo + "\".");
                    _carregando = false;
                    return;
                }

                foreach (var artista in artistas)
                {
                    string generos = artista.Generos != null && artista.Generos.Count > 0
                        ? string.Join(", ", artista.Generos.Take(3))
                        : "";
                    string seguidores = artista.Seguidores > 0
                        ? artista.Seguidores.ToString("N0") + " seguidores"
                        : "";
                    string info = string.Join("   .   ", new[] { generos, seguidores }
                        .Where(s => !string.IsNullOrWhiteSpace(s)));

                    var item = new ListViewItem(artista.Nome + (string.IsNullOrWhiteSpace(info) ? "" : "   |   " + info))
                    {
                        ImageIndex = -1
                    };

                    if (!string.IsNullOrWhiteSpace(artista.ImagemUrl))
                    {
                        try
                        {
                            using (var client = new System.Net.Http.HttpClient())
                            {
                                var dados = await client.GetByteArrayAsync(artista.ImagemUrl);
                                using (var ms = new System.IO.MemoryStream(dados))
                                {
                                    _imagens.Images.Add(Image.FromStream(ms));
                                    item.ImageIndex = _imagens.Images.Count - 1;
                                }
                            }
                        }
                        catch
                        {
                            item.ImageIndex = -1;
                        }
                    }

                    lstArtistas.Items.Add(item);
                }

                _carregando = false;
                if (!_exibindoDetalhe)
                {
                    lstArtistas.Visible = true;
                    pnlDetalhe.Visible = false;
                }
            }
            catch (Exception ex)
            {
                _carregando = false;
                _imagens.Images.Clear();
                lstArtistas.Items.Clear();
                AdicionarMensagem("Erro ao buscar artistas: " + ex.Message);
            }
        }

        private Bitmap DesenharPlay()
        {
            var bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var pontos = new[]
                {
                    new PointF(4.5f, 2.5f),
                    new PointF(13.5f, 8f),
                    new PointF(4.5f, 13.5f)
                };
                g.FillPolygon(new SolidBrush(Color.White), pontos);
            }
            return bmp;
        }

        private string FormatarDuracao(int segundos)
        {
            if (segundos <= 0)
                return "--:--";
            int min = segundos / 60;
            int sec = segundos % 60;
            return min + ":" + sec.ToString("00");
        }
    }
}