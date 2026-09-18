using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class FormMusicas : Form
    {
        public FormMusicas()
        {
            Inicializar();
        }

        private void Inicializar()
        {
            Text = "Musicas - Tecfy";
            ClientSize = new Size(560, 460);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(13, 7, 20);
            Font = new Font("Segoe UI", 9F);

            var lblTitulo = new Label
            {
                Text = "Musicas",
                ForeColor = Color.FromArgb(168, 85, 247),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 25)
            };

            var lblInfo = new Label
            {
                Text = "Todas as musicas salvas no banco Tecfy:",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 70)
            };

            var flp = new FlowLayoutPanel
            {
                Location = new Point(30, 100),
                Size = new Size(500, 320),
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.FromArgb(20, 11, 30)
            };

            CarregarMusicas(flp);

            Controls.Add(lblTitulo);
            Controls.Add(lblInfo);
            Controls.Add(flp);
        }

        private void CarregarMusicas(FlowLayoutPanel flp)
        {
            flp.Controls.Clear();

            var faixas = MusicaDAO.ListarFaixas();

            if (faixas == null || faixas.Count == 0)
            {
                flp.Controls.Add(Aviso("Nenhuma musica salva no banco ainda."));
                return;
            }

            foreach (var faixa in faixas)
            {
                flp.Controls.Add(CriarItem(faixa));
            }
        }

        private Control Aviso(string texto)
        {
            return new Label
            {
                Text = texto,
                ForeColor = Color.White,
                AutoSize = true,
                Padding = new Padding(5),
                Margin = new Padding(2)
            };
        }

        private Control CriarItem(SpotifyService.Faixa faixa)
        {
            var linha = new Panel
            {
                Size = new Size(480, 42),
                Margin = new Padding(2)
            };

            var lblNome = new Label
            {
                Text = faixa.Nome + "  -  " + faixa.Artistas,
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 12),
                Font = new Font("Segoe UI", 10F)
            };

            var lblDur = new Label
            {
                Text = faixa.DuracaoSegundos > 0
                    ? TimeSpan.FromSeconds(faixa.DuracaoSegundos).ToString(@"mm\:ss")
                    : "",
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(430, 12),
                Font = new Font("Segoe UI", 9F)
            };

            linha.Controls.Add(lblNome);
            linha.Controls.Add(lblDur);
            return linha;
        }
    }
}
