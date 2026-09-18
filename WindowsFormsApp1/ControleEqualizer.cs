using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // Animacao de equalizador: barrinhas verticais que sobem e descem enquanto
    // uma musica esta tocando. Mostrada ao lado da musica atual no player.
    public class ControleEqualizer : Control
    {
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Random _rand = new Random();
        private readonly int[] _alturas;
        private readonly int[] _alvos;

        public ControleEqualizer()
        {
            _alturas = new int[5];
            _alvos = new int[5];
SetStyle(
    ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
    | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw
    | ControlStyles.SupportsTransparentBackColor,
    true);
            DoubleBuffered = true;

            for (int i = 0; i < _alturas.Length; i++)
            {
                _alturas[i] = 3 + _rand.Next(8);
                _alvos[i] = _alturas[i];
            }

            _timer = new System.Windows.Forms.Timer { Interval = 110 };
            _timer.Tick += Timer_Tick;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Transparent);

            int n = _alturas.Length;
            int larguraBarra = Math.Max(2, (Width - (n - 1) * 3) / n);
            using (var pincel = new SolidBrush(Tema.Roxo))
            {
                for (int i = 0; i < n; i++)
                {
                    int altura = Math.Max(3, Math.Min(Height - 2, _alturas[i]));
                    int x = i * (larguraBarra + 3);
                    int y = Height - altura;
                    e.Graphics.FillRectangle(pincel, x, y, larguraBarra, altura);
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < _alturas.Length; i++)
            {
                if (_alturas[i] == _alvos[i])
                    _alvos[i] = 3 + _rand.Next(Math.Max(4, Height - 3));

                if (_alturas[i] < _alvos[i])
                    _alturas[i] = Math.Min(_alvos[i], _alturas[i] + 2);
                else
                    _alturas[i] = Math.Max(3, _alturas[i] - 2);
            }
            Invalidate();
        }

        public void Iniciar()
        {
            _timer.Start();
        }

        public void Parar()
        {
            _timer.Stop();
            for (int i = 0; i < _alturas.Length; i++)
                _alturas[i] = 3;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    _timer?.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}