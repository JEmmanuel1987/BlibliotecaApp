using BibliotecaApp.BLL;
using BibliotecaApp.Data;
using BibliotecaApp.Data.Repositories;
using BibliotecaApp.Models;
using BibliotecaApp.UI.Helpers;

namespace BibliotecaApp.UI.Forms
{
    // ─── Formulario de Préstamos y Devoluciones ───────────────────────────────
    public class FrmPrestamos : Form
    {
        private readonly PrestamoService _service;
        private readonly BibliotecaContext _context;

        private DataGridView dgvPrestamos  = new();
        private ComboBox     cmbFiltro     = new();
        private Button       btnPrestar    = new();
        private Button       btnDevolver   = new();
        private Button       btnActualizar = new();

        // Panel de nuevo préstamo
        private Panel        pnlNuevo      = new();
        private ComboBox     cmbSocio      = new();
        private ComboBox     cmbLibro      = new();
        private TextBox      txtObs        = new();
        private Button       btnConfirmar  = new();
        private Button       btnCancelar   = new();

        public FrmPrestamos()
        {
            _context = new BibliotecaContext();
            var repoP = new PrestamoRepository(_context);
            var repoL = new LibroRepository(_context);
            var repoS = new SocioRepository(_context);
            _service  = new PrestamoService(repoP, repoL, repoS);

            InicializarComponentes();
            CargarPrestamos();
        }

        private void InicializarComponentes()
        {
            Text          = "Préstamos y Devoluciones";
            Size          = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor     = Color.FromArgb(245, 247, 249);
            Font          = new Font("Segoe UI", 9f);

            // ── Barra superior ─────────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 55,
                BackColor = Color.FromArgb(45, 62, 80),
                Padding   = new Padding(12, 10, 12, 10)
            };

            new Label
            {
                Text      = "📖  Préstamos y Devoluciones",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(12, 13),
                Parent    = pnlTop
            };

            // Combo filtro
            var lblFiltro = new Label { Text = "Ver:", ForeColor = Color.White, Location = new Point(580, 17), AutoSize = true, Parent = pnlTop };
            cmbFiltro = new ComboBox
            {
                Location      = new Point(610, 13),
                Width         = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Parent        = pnlTop
            };
            cmbFiltro.Items.AddRange(new object[] { "Todos", "Activos", "Vencidos", "Devueltos" });
            cmbFiltro.SelectedIndex = 0;
            cmbFiltro.SelectedIndexChanged += (s, e) => CargarPrestamos();

            // ── Botones ────────────────────────────────────────────────────────
            var pnlBotones = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 45,
                BackColor = Color.White,
                Padding   = new Padding(10, 7, 10, 7)
            };

            btnPrestar = CrearBtn("＋ Nuevo préstamo", Color.FromArgb(46, 204, 113), 0);
            btnPrestar.Click += (s, e) => MostrarPanelNuevo();

            btnDevolver = CrearBtn("↩ Registrar devolución", Color.FromArgb(52, 152, 219), 155);
            btnDevolver.Click += BtnDevolver_Click;

            btnActualizar = CrearBtn("⟳ Actualizar estados", Color.FromArgb(155, 89, 182), 335);
            btnActualizar.Click += BtnActualizar_Click;

            pnlBotones.Controls.AddRange(new Control[] { btnPrestar, btnDevolver, btnActualizar });

            // ── Grid ───────────────────────────────────────────────────────────
            dgvPrestamos = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.EstilizarGrid(dgvPrestamos);
            dgvPrestamos.DataBindingComplete += ColorizarFilas;

            // ── Panel lateral: nuevo préstamo ──────────────────────────────────
            pnlNuevo = new Panel
            {
                Width     = 360,
                Dock      = DockStyle.Right,
                BackColor = Color.White,
                Padding   = new Padding(16),
                Visible   = false
            };
            ConstruirPanelNuevoPrestamo();

            var pnlCentro = new Panel { Dock = DockStyle.Fill };
            pnlCentro.Controls.Add(dgvPrestamos);
            pnlCentro.Controls.Add(pnlNuevo);

            Controls.Add(pnlCentro);
            Controls.Add(pnlBotones);
            Controls.Add(pnlTop);
        }

        private void ConstruirPanelNuevoPrestamo()
        {
            int y = 16;

            Lbl("Nuevo Préstamo", 16, ref y, bold: true, size: 12f);
            y += 8;

            Lbl("Socio *", 16, ref y);
            cmbSocio.SetBounds(16, y, 310, 28);
            cmbSocio.DropDownStyle = ComboBoxStyle.DropDownList;
            y += 40;

            Lbl("Libro disponible *", 16, ref y);
            cmbLibro.SetBounds(16, y, 310, 28);
            cmbLibro.DropDownStyle = ComboBoxStyle.DropDownList;
            y += 40;

            Lbl("Observaciones", 16, ref y);
            txtObs.SetBounds(16, y, 310, 55);
            txtObs.Multiline = true;
            y += 65;

            // Info sobre plazo
            var lblInfo = new Label
            {
                Text      = "ℹ  Plazo: 15 días · Multa: Bs. 2.50/día de retraso",
                Location  = new Point(16, y),
                Width     = 310,
                AutoSize  = false,
                Height    = 36,
                ForeColor = Color.FromArgb(41, 128, 185),
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic)
            };
            y += 50;

            btnConfirmar = new Button
            {
                Text      = "Confirmar préstamo",
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f)
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.SetBounds(16, y, 310, 35);
            btnConfirmar.Click += BtnConfirmarPrestamo_Click;

            y += 44;
            btnCancelar = new Button
            {
                Text      = "Cancelar",
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.SetBounds(16, y, 310, 30);
            btnCancelar.Click += (s, e) => pnlNuevo.Visible = false;

            pnlNuevo.Controls.AddRange(new Control[]
            {
                cmbSocio, cmbLibro, txtObs, lblInfo, btnConfirmar, btnCancelar
            });
        }

        // ─── Carga de datos ────────────────────────────────────────────────────
        private void CargarPrestamos()
        {
            IEnumerable<Prestamo> prestamos = cmbFiltro.SelectedItem?.ToString() switch
            {
                "Activos"   => _service.ObtenerActivos(),
                "Vencidos"  => _service.ObtenerVencidos(),
                "Devueltos" => _service.ObtenerTodos().Where(p => p.Estado == EstadoPrestamo.Devuelto),
                _           => _service.ObtenerTodos()
            };

            dgvPrestamos.DataSource = prestamos.Select(p => new
            {
                Id               = p.PrestamoId,
                Socio            = p.Socio?.NombreCompleto ?? "-",
                Libro            = p.Libro?.Titulo ?? "-",
                FechaPréstamo    = p.FechaPrestamo.ToString("dd/MM/yyyy"),
                FechaVencimiento = p.FechaDevolucionEsperada.ToString("dd/MM/yyyy"),
                FechaDevolucion  = p.FechaDevolucionReal?.ToString("dd/MM/yyyy") ?? "—",
                Estado           = p.Estado.ToString(),
                DiasRetraso      = p.DiasRetraso > 0 ? p.DiasRetraso.ToString() : "—",
                MultaBs          = p.MultaGenerada > 0 ? $"Bs. {p.MultaGenerada:F2}" : "—",
                _estaVencido     = p.EstaVencido  // columna oculta para colorear
            }).ToList();

            // Ocultar la columna auxiliar
            if (dgvPrestamos.Columns.Contains("_estaVencido"))
                dgvPrestamos.Columns["_estaVencido"]!.Visible = false;
        }

        private void ColorizarFilas(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvPrestamos.Rows)
            {
                var estado   = row.Cells["Estado"].Value?.ToString()     ?? "";
                var vencido  = row.Cells["_estaVencido"]?.Value is true;
                row.DefaultCellStyle.BackColor = UIHelper.ColorEstadoPrestamo(estado, vencido);
            }
        }

        // ─── Eventos ───────────────────────────────────────────────────────────
        private void MostrarPanelNuevo()
        {
            // Cargar socios y libros disponibles
            var socios = _context.Socios.Where(s => s.Activo).OrderBy(s => s.Apellido).ToList();
            cmbSocio.DataSource    = socios;
            cmbSocio.DisplayMember = "NombreCompleto";
            cmbSocio.ValueMember   = "SocioId";

            var libros = _context.Libros
                .Where(l => l.Activo && l.StockDisponible > 0)
                .OrderBy(l => l.Titulo).ToList();
            cmbLibro.DataSource    = libros;
            cmbLibro.DisplayMember = "Titulo";
            cmbLibro.ValueMember   = "LibroId";

            UIHelper.LimpiarFormulario(pnlNuevo);
            pnlNuevo.Visible = true;
        }

        private void BtnConfirmarPrestamo_Click(object? s, EventArgs e)
        {
            if (cmbSocio.SelectedValue == null || cmbLibro.SelectedValue == null)
            {
                UIHelper.Error("Selecciona un socio y un libro.");
                return;
            }

            int socioId = (int)cmbSocio.SelectedValue;
            int libroId = (int)cmbLibro.SelectedValue;

            var (exito, mensaje) = _service.RegistrarPrestamo(socioId, libroId, txtObs.Text);

            if (exito)
            {
                UIHelper.Exito(mensaje);
                pnlNuevo.Visible = false;
                CargarPrestamos();
            }
            else UIHelper.Error(mensaje);
        }

        private void BtnDevolver_Click(object? s, EventArgs e)
        {
            var id = UIHelper.ObtenerIdSeleccionado(dgvPrestamos);
            if (id == null) { UIHelper.Error("Selecciona un préstamo para registrar la devolución."); return; }

            var prestamo = _service.ObtenerPorId(id.Value);
            if (prestamo == null) return;

            if (prestamo.Estado == EstadoPrestamo.Devuelto)
            {
                UIHelper.Error("Este préstamo ya fue devuelto.");
                return;
            }

            // Mostrar multa esperada antes de confirmar
            decimal multaEstimada = _service.CalcularMulta(prestamo);
            string aviso = multaEstimada > 0
                ? $"⚠  Devolución con {prestamo.DiasRetraso} días de retraso.\nSe generará una multa de Bs. {multaEstimada:F2}.\n\n¿Confirmar devolución?"
                : "Devolución dentro del plazo. ¿Confirmar?";

            if (!UIHelper.Confirmar(aviso)) return;

            var (exito, mensaje, multa) = _service.RegistrarDevolucion(id.Value);

            if (exito)
            {
                UIHelper.Exito(mensaje);
                CargarPrestamos();
            }
            else UIHelper.Error(mensaje);
        }

        private void BtnActualizar_Click(object? s, EventArgs e)
        {
            int count = _service.ActualizarEstadosVencidos();
            UIHelper.Exito(count > 0
                ? $"Se actualizaron {count} préstamo(s) a estado 'Vencido'."
                : "No hay préstamos vencidos sin actualizar.");
            CargarPrestamos();
        }

        // ─── Helpers ───────────────────────────────────────────────────────────
        private Button CrearBtn(string texto, Color color, int x)
        {
            var btn = new Button
            {
                Text      = texto,
                Location  = new Point(x, 7),
                Width     = 165, Height = 30,
                BackColor = color, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void Lbl(string txt, int x, ref int y, bool bold = false, float size = 9f)
        {
            var l = new Label
            {
                Text     = txt,
                Location = new Point(x, y),
                AutoSize = true,
                Font     = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Parent   = pnlNuevo
            };
            y += 20;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _context.Dispose();
            base.OnFormClosed(e);
        }
    }
}
