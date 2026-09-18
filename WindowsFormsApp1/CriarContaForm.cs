using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace WindowsFormsApp1
{
    public class CriarContaForm : Form
    {
        private readonly Guna2TextBox txtNome = new Guna2TextBox();
        private readonly Guna2TextBox txtEmail = new Guna2TextBox();
        private readonly Guna2TextBox txtSenha = new Guna2TextBox();
        private readonly Guna2TextBox txtConfirmar = new Guna2TextBox();
        private readonly Guna2Button btnCriar = new Guna2Button();

        public CriarContaForm()
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
            Text = "Criar conta";
            ClientSize = new Size(360, 390);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(13, 7, 20);

            var lblTitulo = new Label
            {
                Text = "Crie sua conta",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(60, 20)
            };

            EstilizarCampo(txtNome, "Nome", 70);
            EstilizarCampo(txtEmail, "Email", 120);
            EstilizarCampo(txtSenha, "Senha", 170);
            txtSenha.UseSystemPasswordChar = true;
            EstilizarCampo(txtConfirmar, "Confirmar senha", 220);
            txtConfirmar.UseSystemPasswordChar = true;

            btnCriar.Text = "Criar conta";
            btnCriar.BorderRadius = 9;
            btnCriar.FillColor = Color.FromArgb(124, 58, 237);
            btnCriar.ForeColor = Color.White;
            btnCriar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnCriar.Location = new Point(60, 280);
            btnCriar.Size = new Size(240, 40);
            btnCriar.Click += btnCriar_Click;

            Controls.Add(lblTitulo);
            Controls.Add(txtNome);
            Controls.Add(txtEmail);
            Controls.Add(txtSenha);
            Controls.Add(txtConfirmar);
            Controls.Add(btnCriar);

            AcceptButton = btnCriar;
        }

        private void btnCriar_Click(object sender, EventArgs e)
        {
            var resultado = UsuarioService.Cadastrar(
                txtNome.Text, txtEmail.Text, txtSenha.Text, txtConfirmar.Text);

            if (!resultado.Ok)
            {
                MessageBox.Show(resultado.Erro, "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Conta criada com sucesso!", "Tecfy",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
