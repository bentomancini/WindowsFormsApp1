using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace WindowsFormsApp1
{
    public class EsqueciSenhaForm : Form
    {
        private readonly Guna2TextBox txtEmail = new Guna2TextBox();
        private readonly Guna2TextBox txtNovaSenha = new Guna2TextBox();
        private readonly Guna2TextBox txtConfirmar = new Guna2TextBox();
        private readonly Guna2Button btnRedefinir = new Guna2Button();

        public EsqueciSenhaForm()
        {
            Inicializar();
        }

        private void EstilizarCampo(Guna2TextBox campo, string placeholder, int y)
        {
            campo.BorderColor = Color.BlueViolet;
            campo.BorderRadius = 7;
            campo.FillColor = Color.FromArgb(13, 7, 20);
            campo.ForeColor = Color.Azure;
            campo.PlaceholderForeColor = Color.White;
            campo.PlaceholderText = placeholder;
            campo.Font = new Font("Segoe UI", 9F);
            campo.Location = new Point(60, y);
            campo.Size = new Size(240, 36);
        }

        private void Inicializar()
        {
            Text = "Esqueceu a senha";
            ClientSize = new Size(360, 320);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(13, 7, 20);

            var lblTitulo = new Label
            {
                Text = "Redefinir senha",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(60, 20)
            };

            var lblAviso = new Label
            {
                Text = "Digite seu email e escolha uma nova senha.",
                ForeColor = Color.Silver,
                AutoSize = true,
                Location = new Point(60, 52)
            };

            EstilizarCampo(txtEmail, "Email", 85);
            EstilizarCampo(txtNovaSenha, "Nova senha", 135);
            txtNovaSenha.UseSystemPasswordChar = true;
            EstilizarCampo(txtConfirmar, "Confirmar nova senha", 185);
            txtConfirmar.UseSystemPasswordChar = true;

            btnRedefinir.Text = "Redefinir";
            btnRedefinir.BorderRadius = 9;
            btnRedefinir.FillColor = Color.FromArgb(124, 58, 237);
            btnRedefinir.ForeColor = Color.White;
            btnRedefinir.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnRedefinir.Location = new Point(60, 240);
            btnRedefinir.Size = new Size(240, 40);
            btnRedefinir.Click += btnRedefinir_Click;

            Controls.Add(lblTitulo);
            Controls.Add(lblAviso);
            Controls.Add(txtEmail);
            Controls.Add(txtNovaSenha);
            Controls.Add(txtConfirmar);
            Controls.Add(btnRedefinir);

            AcceptButton = btnRedefinir;
        }

        private void btnRedefinir_Click(object sender, EventArgs e)
        {
            var resultado = UsuarioService.RedefinirSenha(
                txtEmail.Text, txtNovaSenha.Text, txtConfirmar.Text);

            if (!resultado.Ok)
            {
                MessageBox.Show(resultado.Erro, "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Senha redefinida com sucesso!", "Tecfy",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
