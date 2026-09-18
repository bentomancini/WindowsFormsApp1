using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // Troca o tema da interface entre escuro (padrao) e claro.
    // Percorre a arvore de controles substituindo as cores escuras da paleta
    // atual pelas equivalentes claras, e vice-versa.
    public static class Tema
    {
        public static bool Claro { get; private set; }

        public static Color Fundo;
        public static Color FundoElevado;
        public static Color FundoCard;
        public static Color FundoPainel;
        public static Color Texto;
        public static Color TextoSecundario;
        public static Color Destaque;
        public static Color Roxo;

        // Paleta escura original da aplicacao.
        private static readonly Color C_Fundo = Color.FromArgb(13, 7, 20);
        private static readonly Color C_FundoElevado = Color.FromArgb(28, 16, 42);
        private static readonly Color C_FundoCard = Color.FromArgb(45, 20, 65);
        private static readonly Color C_FundoPainel = Color.FromArgb(20, 11, 30);
        private static readonly Color C_Texto = Color.White;
        private static readonly Color C_TextoSecundario = Color.Silver;
        private static readonly Color C_Destaque = Color.FromArgb(168, 85, 247);
        private static readonly Color C_Roxo = Color.FromArgb(124, 58, 237);

        static Tema()
        {
            AplicarEscuro();
        }

        public static void Alternar()
        {
            if (Claro)
                AplicarEscuro();
            else
                AplicarClaro();
        }

        private static void AplicarEscuro()
        {
            Claro = false;
            Fundo = C_Fundo;
            FundoElevado = C_FundoElevado;
            FundoCard = C_FundoCard;
            FundoPainel = C_FundoPainel;
            Texto = C_Texto;
            TextoSecundario = C_TextoSecundario;
            Destaque = C_Destaque;
            Roxo = C_Roxo;
        }

        private static void AplicarClaro()
        {
            Claro = true;
            Fundo = Color.FromArgb(247, 243, 250);
            FundoElevado = Color.FromArgb(255, 255, 255);
            FundoCard = Color.FromArgb(233, 216, 244);
            FundoPainel = Color.FromArgb(252, 250, 254);
            Texto = Color.FromArgb(35, 25, 45);
            TextoSecundario = Color.FromArgb(100, 90, 115);
            Destaque = Color.FromArgb(109, 40, 217);
            Roxo = C_Roxo;
        }

        public static Color TrocarFundo(Color cor)
        {
            if (Claro)
            {
                if (cor == C_Fundo) return Fundo;
                if (cor == C_FundoElevado) return FundoElevado;
                if (cor == C_FundoCard) return FundoCard;
                if (cor == C_FundoPainel) return FundoPainel;
                return cor;
            }
            else
            {
                if (cor == Fundo) return C_Fundo;
                if (cor == FundoElevado) return C_FundoElevado;
                if (cor == FundoCard) return C_FundoCard;
                if (cor == FundoPainel) return C_FundoPainel;
                return cor;
            }
        }

        public static Color TrocarFonte(Color cor)
        {
            if (Claro)
            {
                if (cor == C_Texto) return Texto;
                if (cor == C_TextoSecundario) return TextoSecundario;
                if (cor == C_Destaque) return Destaque;
                if (cor == Color.Gray) return TextoSecundario;
                return cor;
            }
            else
            {
                if (cor == Texto) return C_Texto;
                if (cor == TextoSecundario) return C_TextoSecundario;
                if (cor == Destaque) return C_Destaque;
                if (cor == Color.Gray) return C_TextoSecundario;
                return cor;
            }
        }

        // Aplica cantos arredondados a qualquer controle (Button, Panel,
        // PictureBox, etc.) via Region. raio = 0 usa pilula (metade da altura).
        // Reaplicar sempre que o controle mudar de tamanho.
        public static void Arredondar(Control c, int raio = 0)
        {
            if (c == null || c.Width <= 1 || c.Height <= 1)
                return;

            int max = Math.Max(1, Math.Min(c.Width, c.Height) / 2);
            if (raio <= 0)
                raio = max;
            raio = Math.Max(1, Math.Min(raio, max));

            int d = raio * 2;
            using (var caminho = new System.Drawing.Drawing2D.GraphicsPath())
            {
                caminho.AddArc(0, 0, d, d, 180, 90);
                caminho.AddArc(c.Width - d - 1, 0, d, d, 270, 90);
                caminho.AddArc(c.Width - d - 1, c.Height - d - 1, d, d, 0, 90);
                caminho.AddArc(0, c.Height - d - 1, d, d, 90, 90);
                caminho.CloseFigure();
                c.Region = new Region(caminho);
            }
        }

        // Aplica o tema atual em toda a arvore de controles a partir de raiz.
        public static void Aplicar(Control raiz)
        {
            if (raiz == null)
                return;

            AplicarNoControle(raiz);
            foreach (Control filho in raiz.Controls)
                Aplicar(filho);
        }

        private static void AplicarNoControle(Control c)
        {
            if (c is Guna.UI2.WinForms.Guna2TextBox txtBox)
            {
                txtBox.FillColor = Fundo;
                txtBox.ForeColor = Texto;
                txtBox.PlaceholderForeColor = TextoSecundario;
                return;
            }

            if (c is Guna.UI2.WinForms.Guna2Button btnGuna)
            {
                if (btnGuna.FillColor == Color.Transparent)
                    btnGuna.ForeColor = Texto;
                else
                    btnGuna.ForeColor = Color.White;
                return;
            }

            if (c is Guna.UI2.WinForms.Guna2Panel panelGuna)
            {
                if (panelGuna.FillColor != Color.Transparent && panelGuna.FillColor != Color.Empty)
                    panelGuna.FillColor = TrocarFundo(panelGuna.FillColor);
                return;
            }

            if (c is Label lbl)
            {
                if (lbl.BackColor != Color.Transparent)
                    lbl.BackColor = TrocarFundo(lbl.BackColor);
                lbl.ForeColor = TrocarFonte(lbl.ForeColor);
                return;
            }

            if (c is Button botao)
            {
                Color antes = botao.BackColor;
                Color depois = TrocarFundo(antes);
                botao.BackColor = depois;
                if (depois != antes)
                    botao.ForeColor = Texto;
                return;
            }

            if (c is TrackBar trk)
            {
                trk.BackColor = TrocarFundo(trk.BackColor);
                trk.ForeColor = TrocarFonte(trk.ForeColor);
                return;
            }

            if (c is ListBox lista)
            {
                lista.BackColor = TrocarFundo(lista.BackColor);
                lista.ForeColor = TrocarFonte(lista.ForeColor);
                return;
            }

            if (c is PictureBox pic)
            {
                if (pic.Image == null && pic.BackgroundImage == null)
                    pic.BackColor = TrocarFundo(pic.BackColor);
                return;
            }

            if (c is DataGridView || c is TextBoxBase)
                return;

            c.BackColor = TrocarFundo(c.BackColor);
        }
    }
}