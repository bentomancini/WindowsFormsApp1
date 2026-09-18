using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnEntrar.Click += btnEntrar_Click;
            llabelCriar.LinkClicked += llabelCriar_LinkClicked;
            llabelEsqueceu.LinkClicked += llabelEsqueceu_LinkClicked;
            ConfigurarOlhoSenha();
            AcceptButton = btnEntrar;
        }

        // Adiciona o botao de olho na txtSenha para mostrar/esconder a senha.
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
                    // Olho aberto: cantho do olho (elipse) + pupila.
                    g.DrawEllipse(caneta, 3, 6, 18, 12);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(168, 85, 247)), 9, 10, 6, 4);
                }
                else
                {
                    // Olho fechado (corte): linha diagonal riscando um olho desenhado.
                    g.DrawEllipse(caneta, 4, 6, 16, 12);
                    g.DrawLine(caneta, 3, 3, 21, 21);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(168, 85, 247)), 9, 10, 4, 4);
                }
            }
            return bmp;
        }

        private static readonly string CaminhoLembrar = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ".",
            "lembrar_login.txt");

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

            MessageBox.Show("Bem-vindo, " + usuario.Nome + "!", "Tecfy",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            using (var form2 = new Form2(usuario.Id, usuario.Nome))
            {
                this.Hide();
                form2.ShowDialog(this);
                this.Show();
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
            catch
            {
            }
        }

        private void llabelCriar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var form = new CriarContaForm())
            {
                form.ShowDialog(this);
            }
        }

        private void llabelEsqueceu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var form = new EsqueciSenhaForm())
            {
                form.ShowDialog(this);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            CarregarLembrar();

            try
            {
                await webView21.EnsureCoreWebView2Async();

                string caminho = System.IO.Path.Combine(
                    Application.StartupPath,
                    "assets",
                    "index.html"
                );

                webView21.Source = new Uri(caminho);
            }
            catch (Exception)
            {
                // Se o WebView2 nao estiver disponivel (runtime ausente, processo
                // abortado etc.), apenas esconde o controle e segue com o login.
                webView21.Visible = false;
            }
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
            catch
            {
            }
        }

        private void webView21_Click_1(object sender, EventArgs e)
        {

        }
    }
}

