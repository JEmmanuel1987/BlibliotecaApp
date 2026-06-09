using BibliotecaApp.BLL;
using BibliotecaApp.Data;
using BibliotecaApp.Data.Repositories;

namespace BibliotecaApp.UI.Forms
{
    // ─── Formulario Principal / Dashboard ─────────────────────────────────────
    public class FrmPrincipal : Form
    {
        private readonly BibliotecaContext _context;
        private readonly PrestamoService _prestamoService;

        public FrmPrincipal()
        {
            _context = new BibliotecaContext();
            var repoP = new PrestamoRepository(_context);
            var repoL = new LibroRepository(_context);
            var repoS = new SocioRepository(_context);
            _prestamoService = new PrestamoService(repoP, repoL, repoS);

            InicializarComponentes();
            CargarResumen();
        }

        private void InicializarComponentes()
        {
            Text            = "Sistema de Gestión de Biblioteca";
            Size            = new Size(960, 600);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 247, 249);
            Font            = new Font("Segoe UI", 9f);

            // ── Header ─────────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = Color.FromArgb(45, 62, 80)
            };
            new Label
            {
                Text      = "📚  Sistema de Gestión de Biblioteca",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 15f, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(20, 18),
                Parent    = pnlHeader
            };
            new Label
            {
                Text      = "Universidad Tecnológica Boliviana · UTB",
                ForeColor = Color.FromArgb(149, 165, 166),
                Font      = new Font("Segoe UI", 9f),
                AutoSize  = true,
                Location  = new Point(22, 44),
                Parent    = pnlHeader
            };

            // ── Menú lateral ───────────────────────────────────────────────────
            var pnlMenu = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 220,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding   = new Padding(0, 20, 0, 0)
            };

            var opciones = new (string texto, Action accion)[]
            {
                ("  📊  Dashboard",              CargarResumen),
                ("  📚  Gestión de Libros",      () => AbrirForm(new FrmLibros())),
                ("  👥  Gestión de Socios",      () => AbrirForm(new FrmSocios())),
                ("  📖  Préstamos",              () => AbrirForm(new FrmPrestamos())),
            };

            int yMenu = 20;
            foreach (var (texto, accion) in opciones)
            {
                var btn = new Button
                {
                    Text      = texto,
                    Location  = new Point(0, yMenu),
                    Width     = 220, Height = 45,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(189, 195, 199),
                    FlatStyle = FlatStyle.Flat,
                    Font      = new Font("Segoe UI", 10f),
                    Cursor    = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize    = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(44, 62, 80);
                var captura = accion;
                btn.Click += (s, e) => captura();
                pnlMenu.Controls.Add(btn);
                yMenu += 45;
            }

            // ── Panel central (dashboard) ──────────────────────────────────────
            var pnlCentro = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(30)
            };
            pnlCentro.Name = "pnlCentro";

            Controls.Add(pnlCentro);
            Controls.Add(pnlMenu);
            Controls.Add(pnlHeader);
        }

        // ─── Dashboard con tarjetas de resumen ─────────────────────────────────
        private void CargarResumen()
        {
            var pnlCentro = Controls.Find("pnlCentro", true).FirstOrDefault() as Panel;
            if (pnlCentro == null) return;
            pnlCentro.Controls.Clear();

            var resumen = _prestamoService.ObtenerResumen();

            var tarjetas = new (string titulo, string valor, Color color)[]
            {
                ("Préstamos activos",   resumen.TotalActivos.ToString(),            Color.FromArgb(52, 152, 219)),
                ("Préstamos vencidos",  resumen.TotalVencidos.ToString(),           Color.FromArgb(231, 76, 60)),
                ("Total devueltos",     resumen.TotalDevueltos.ToString(),           Color.FromArgb(46, 204, 113)),
                ("Multas pendientes",   $"Bs. {resumen.MultasPendientes:F2}",       Color.FromArgb(230, 126, 34))
            };

            int x = 0, y = 0, i = 0;
            foreach (var (titulo, valor, color) in tarjetas)
            {
                x = (i % 2) * 230;
                y = (i / 2) * 130 + 10;

                var card = new Panel
                {
                    Location  = new Point(x, y),
                    Size      = new Size(210, 110),
                    BackColor = Color.White,
                    Padding   = new Padding(15)
                };
                card.Paint += (s, e) =>
                {
                    e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, 6, 110));
                };

                new Label
                {
                    Text      = valor,
                    Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
                    ForeColor = color,
                    Location  = new Point(18, 15),
                    AutoSize  = true,
                    Parent    = card
                };
                new Label
                {
                    Text      = titulo,
                    Font      = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(127, 140, 141),
                    Location  = new Point(18, 65),
                    AutoSize  = true,
                    Parent    = card
                };

                pnlCentro.Controls.Add(card);
                i++;
            }

            // Título del dashboard
            var lblTitulo = new Label
            {
                Text      = "Resumen del sistema",
                Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location  = new Point(0, 295),
                AutoSize  = true
            };
            pnlCentro.Controls.Add(lblTitulo);
        }

        private void AbrirForm(Form form)
        {
            form.MdiParent = null;
            form.ShowDialog();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _context.Dispose();
            base.OnFormClosed(e);
        }
    }
}
