using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class FormRenomearPlaylist : Form
    {
        private readonly int _idPlaylist;
        private TextBox txtNome;
        private Label lblStatus;

        public FormRenomearPlaylist(int idPlaylist, string nomeAtual)
        {
            _idPlaylist = idPlaylist;
            Inicializar(nomeAtual);
        }

        private void Inicializar(string nomeAtual)
        {
            Text = "Renomear playlist";
            ClientSize = new Size(360, 200);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(13, 7, 20);
            Font = new Font("Segoe UI", 9F);

            var lblNome = new Label
            {
                Text = "Novo nome da playlist:",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(25, 30)
            };

            txtNome = new TextBox
            {
                Text = nomeAtual ?? "",
                Location = new Point(25, 60),
                Size = new Size(310, 30),
                BackColor = Color.FromArgb(28, 16, 42),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                MaxLength = 150
            };

            var btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new Point(110, 115),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(124, 58, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += BtnSalvar_Click;
            Tema.Arredondar(btnSalvar, 17);

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
            Controls.Add(btnSalvar);
            Controls.Add(btnCancelar);
            Controls.Add(lblStatus);
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                lblStatus.Text = "Informe um nome.";
                return;
            }

            var resultado = PlaylistDAO.Renomear(_idPlaylist, txtNome.Text);

            if (resultado.Ok)
            {
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
