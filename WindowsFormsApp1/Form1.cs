using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private PictureBox _btnOlho;
        private Label _lblUltimoLogin;

        private static readonly string CaminhoLembrar =
            System.IO.Path.Combine(Application.StartupPath, "lembrar_login.txt");

        private static readonly string CaminhoUltimoLogin =
            System.IO.Path.Combine(Application.StartupPath, "ultimo_login.txt");

        public Form1()
        {
            InitializeComponent();
            btnEntrar.Click += btnEntrar_Click;
            llabelCriar.LinkClicked += llabelCriar_LinkClicked;
            llabelEsqueceu.LinkClicked += llabelEsqueceu_LinkClicked;
            ConfigurarOlhoSenha();
            CriarTextoUltimoLogin();
            AcceptButton = btnEntrar;
            Resize += (s, e) => ReposicionarResponsivo();
        }

        private void ConfigurarOlhoSenha()
        {
            txtSenha.UseSystemPasswordChar = true;

            var btnOlho = new PictureBox
            {
                Size = new Size(24, 24),
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(13, 7, 20),
                Image = DesenharOlho(false),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(
                    txtSenha.Right - txtSenha.Height + 4,
                    txtSenha.Top + (txtSenha.Height - 24) / 2)
            };

            btnOlho.Click += (s, e) =>
            {
                bool mostrar = txtSenha.UseSystemPasswordChar;
                txtSenha.UseSystemPasswordChar = !mostrar;
                btnOlho.Image = DesenharOlho(!mostrar);
                txtSenha.Focus();
                txtSenha.SelectionStart = txtSenha.Text.Length;
            };

            _btnOlho = btnOlho;
            Controls.Add(btnOlho);
            btnOlho.BringToFront();
        }

        private Bitmap DesenharOlho(bool aberto)
        {
            var bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Pen caneta = new Pen(Color.FromArgb(168, 85, 247), 1.6f);
                caneta.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                caneta.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                if (aberto)
                {
                    g.DrawEllipse(caneta, 3, 6, 18, 12);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(168, 85, 247)), 9, 10, 6, 4);
                }
                else
                {
                    g.DrawEllipse(caneta, 4, 6, 16, 12);
                    g.DrawLine(caneta, 3, 3, 21, 21);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(168, 85, 247)), 9, 10, 4, 4);
                }
            }
            return bmp;
        }

        private void CriarTextoUltimoLogin()
        {
            _lblUltimoLogin = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(120, 100, 160),
                Font = new Font("Segoe UI", 8F),
                BackColor = Color.Transparent
            };
            Controls.Add(_lblUltimoLogin);
            _lblUltimoLogin.BringToFront();
            AtualizarTextoUltimoLogin();
        }

        private void AtualizarTextoUltimoLogin()
        {
            try
            {
                if (System.IO.File.Exists(CaminhoUltimoLogin))
                {
                    string[] partes = System.IO.File.ReadAllText(CaminhoUltimoLogin)
                        .Split(new[] { '|' }, 3);
                    if (partes.Length == 3)
                    {
                        _lblUltimoLogin.Text = string.Format(
                            "Ultimo acesso: {0} ({1}) em {2}",
                            partes[0], partes[1], partes[2]);
                        return;
                    }
                }
            }
            catch { }

            _lblUltimoLogin.Text = "";
        }

        private void SalvarUltimoLogin(string nome, string email)
        {
            try
            {
                string data = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                System.IO.File.WriteAllText(CaminhoUltimoLogin,
                    nome + "|" + email + "|" + data);
                AtualizarTextoUltimoLogin();
            }
            catch { }
        }

        private void ReposicionarResponsivo()
        {
            int w = ClientSize.Width, h = ClientSize.Height;
            if (w < 10 || h < 10) return;

            const int baseW = 742, baseH = 450;
            double esc = Math.Min(w / (double)baseW, h / (double)baseH);
            esc = Math.Max(0.8, Math.Min(esc, 2.2));

            int offX = (w - (int)(baseW * esc)) / 2;
            int offY = (h - (int)(baseH * esc)) / 2;

            Mover(webView21, 12, 25, 372, 156, esc, offX, offY);
            Mover(txtEmail, 99, 187, 200, 36, esc, offX, offY);
            Mover(txtSenha, 99, 246, 200, 36, esc, offX, offY);
            MoverPos(cboxLembrar, 99, 288, esc, offX, offY);
            MoverPos(llabelEsqueceu, 205, 289, esc, offX, offY);
            Mover(btnEntrar, 99, 329, 198, 39, esc, offX, offY);
            MoverPos(label1, 96, 371, esc, offX, offY);
            MoverPos(llabelCriar, 236, 371, esc, offX, offY);
            Mover(guna2CirclePictureBox1, 390, 128, 319, 209, esc, offX, offY);

            if (_btnOlho != null)
                MoverPos(_btnOlho, txtSenha.Right - txtSenha.Height + 4,
                    txtSenha.Top + (txtSenha.Height - 24) / 2, esc, offX, offY);

            if (_lblUltimoLogin != null)
                _lblUltimoLogin.Location = new Point(
                    (w - _lblUltimoLogin.Width) / 2,
                    h - (int)(32 * esc));
        }

        private static void Mover(Control c, int bx, int by, int bw, int bh,
            double esc, int offX, int offY)
        {
            c.Location = new Point((int)(bx * esc) + offX, (int)(by * esc) + offY);
            c.Size = new Size((int)(bw * esc), (int)(bh * esc));
        }

        private static void MoverPos(Control c, int bx, int by,
            double esc, int offX, int offY)
        {
            c.Location = new Point((int)(bx * esc) + offX, (int)(by * esc) + offY);
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string senha = txtSenha.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Preencha email e senha.", "Atencao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UsuarioService.UsuarioLogado usuario = UsuarioService.Autenticar(email, senha);

            if (usuario == null)
            {
                MessageBox.Show("Email ou senha incorretos.", "Atencao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SalvarLembrar(email);
            SalvarUltimoLogin(usuario.Nome, email);

            MessageBox.Show("Bem-vindo, " + usuario.Nome + "!", "Tecfy",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            using (var form2 = new Form2(usuario.Id, usuario.Nome))
            {
                this.Hide();
                form2.ShowDialog(this);
                this.Show();
                AtualizarTextoUltimoLogin();
            }
        }

        private void SalvarLembrar(string email)
        {
            try
            {
                if (cboxLembrar.Checked)
                    System.IO.File.WriteAllText(CaminhoLembrar, email);
                else if (System.IO.File.Exists(CaminhoLembrar))
                    System.IO.File.Delete(CaminhoLembrar);
            }
            catch { }
        }

        private void CarregarLembrar()
        {
            try
            {
                if (System.IO.File.Exists(CaminhoLembrar))
                {
                    string email = System.IO.File.ReadAllText(CaminhoLembrar).Trim();
                    if (!string.IsNullOrEmpty(email))
                    {
                        txtEmail.Text = email;
                        cboxLembrar.Checked = true;
                    }
                }
            }
            catch { }
        }

        private void llabelCriar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var form = new CriarContaForm())
                form.ShowDialog(this);
        }

        private void llabelEsqueceu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var form = new EsqueciSenhaForm())
                form.ShowDialog(this);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            CarregarLembrar();
            AtualizarTextoUltimoLogin();
            ReposicionarResponsivo();

            try
            {
                await webView21.EnsureCoreWebView2Async();
                string caminho = System.IO.Path.Combine(
                    Application.StartupPath, "assets", "index.html");
                webView21.Source = new Uri(caminho);
            }
            catch
            {
                webView21.Visible = false;
            }
        }

        private void webView21_Click_1(object sender, EventArgs e) { }
    }
}
