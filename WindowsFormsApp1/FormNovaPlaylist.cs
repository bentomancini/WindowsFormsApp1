using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class FormNovaPlaylist : Form
    {
        private readonly int _usuarioId;
        private TextBox txtNome;
        private Label lblStatus;

        public string NomeCriado { get; private set; }

        public FormNovaPlaylist(int usuarioId)
        {
            _usuarioId = usuarioId;
            Inicializar();
        }

        private void Inicializar()
        {
            Text = "Nova playlist";
            ClientSize = new Size(360, 200);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(13, 7, 20);
            Font = new Font("Segoe UI", 9F);

            var lblNome = new Label
            {
                Text = "Nome da playlist:",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(25, 30)
            };

            txtNome = new TextBox
            {
                Location = new Point(25, 60),
                Size = new Size(310, 30),
                BackColor = Color.FromArgb(28, 16, 42),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                MaxLength = 150
            };

            var btnCriar = new Button
            {
                Text = "Criar",
                Location = new Point(110, 115),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(124, 58, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            btnCriar.FlatAppearance.BorderSize = 0;
            btnCriar.Click += BtnCriar_Click;
            Tema.Arredondar(btnCriar, 17);

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(220, 115),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(45, 20, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Tema.Arredondar(btnCancelar, 17);

            lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.HotPink,
                AutoSize = true,
                Location = new Point(25, 160)
            };

            Controls.Add(lblNome);
            Controls.Add(txtNome);
            Controls.Add(btnCriar);
            Controls.Add(btnCancelar);
            Controls.Add(lblStatus);
        }

        private void BtnCriar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                lblStatus.Text = "Informe um nome.";
                return;
            }

            var resultado = PlaylistDAO.Inserir(txtNome.Text, "", _usuarioId);

            if (resultado.Ok)
            {
                NomeCriado = txtNome.Text.Trim();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblStatus.Text = resultado.Erro;
            }
        }
    }
}
