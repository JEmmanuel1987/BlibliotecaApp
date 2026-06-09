using BibliotecaApp.BLL;
using BibliotecaApp.Data;
using BibliotecaApp.Data.Repositories;
using BibliotecaApp.Models;
using BibliotecaApp.UI.Helpers;

namespace BibliotecaApp.UI.Forms
{
    
    public class FrmLibros : Form
    {
        // ── Servicios ──────────────────────────────────────────────────────────
        private readonly LibroService _service;
        private readonly BibliotecaContext _context;

        // ── Controles principales ──────────────────────────────────────────────
        private DataGridView dgvLibros   = new();
        private TextBox      txtBuscar   = new();
        private Button       btnBuscar   = new();
        private Button       btnNuevo    = new();
        private Button       btnEditar   = new();
        private Button       btnEliminar = new();
        private Panel        pnlDetalle  = new();

        // Campos del formulario de detalle
        private TextBox      txtISBN       = new();
        private TextBox      txtTitulo     = new();
        private TextBox      txtAutor      = new();
        private TextBox      txtEditorial  = new();
        private NumericUpDown nudAnio      = new();
        private NumericUpDown nudStock     = new();
        private ComboBox     cmbCategoria  = new();
        private Button       btnGuardar    = new();
        private Button       btnCancelar   = new();
        private Label        lblModoForm   = new();

        private int? _idEdicion = null; // null = nuevo registro

        public FrmLibros()
        {
            _context = new BibliotecaContext();
            _service = new LibroService(new LibroRepository(_context));
            InicializarComponentes();
            CargarCategorias();
            CargarLibros();
        }

        // ─── Construcción del UI ───────────────────────────────────────────────
        private void InicializarComponentes()
        {
            Text            = "Gestión de Libros";
            Size            = new Size(1100, 680);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 247, 249);
            Font            = new Font("Segoe UI", 9f);
            MinimumSize     = new Size(900, 580);

            // ── Barra superior ─────────────────────────────────────────────────
            var pnlTop = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 55,
                BackColor = Color.FromArgb(45, 62, 80),
                Padding   = new Padding(12, 10, 12, 10)
            };

            var lblTitulo = new Label
            {
                Text      = "📚  Gestión de Libros",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(12, 13)
            };

            txtBuscar = new TextBox
            {
                PlaceholderText = "Buscar por título, autor o ISBN…",
                Width    = 280,
                Height   = 28,
                Location = new Point(680, 13),
                Font     = new Font("Segoe UI", 9f)
            };
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };

            btnBuscar = new Button
            {
                Text      = "Buscar",
                Width     = 75,
                Height    = 28,
                Location  = new Point(968, 13),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            pnlTop.Controls.AddRange(new Control[] { lblTitulo, txtBuscar, btnBuscar });

            // ── Barra de botones ───────────────────────────────────────────────
            var pnlBotones = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 45,
                BackColor = Color.White,
                Padding   = new Padding(10, 7, 10, 7)
            };

            btnNuevo = CrearBoton("＋ Nuevo", Color.FromArgb(46, 204, 113), 0);
            btnNuevo.Click += BtnNuevo_Click;

            btnEditar = CrearBoton("✎ Editar", Color.FromArgb(52, 152, 219), 110);
            btnEditar.Click += BtnEditar_Click;

            btnEliminar = CrearBoton("✕ Eliminar", Color.FromArgb(231, 76, 60), 220);
            btnEliminar.Click += BtnEliminar_Click;

            pnlBotones.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnEliminar });

            // ── Grid ───────────────────────────────────────────────────────────
            dgvLibros = new DataGridView
            {
                Dock = DockStyle.Fill
            };
            UIHelper.EstilizarGrid(dgvLibros);
            dgvLibros.CellDoubleClick += (s, e) => BtnEditar_Click(s, e);

            // ── Panel de detalle (lado derecho) ────────────────────────────────
            pnlDetalle = new Panel
            {
                Width     = 340,
                Dock      = DockStyle.Right,
                BackColor = Color.White,
                Padding   = new Padding(16),
                Visible   = false
            };
            ConstruirPanelDetalle();

            // ── Layout ─────────────────────────────────────────────────────────
            var pnlCentro = new Panel { Dock = DockStyle.Fill };
            pnlCentro.Controls.Add(dgvLibros);
            pnlCentro.Controls.Add(pnlDetalle);

            Controls.Add(pnlCentro);
            Controls.Add(pnlBotones);
            Controls.Add(pnlTop);
        }

        private void ConstruirPanelDetalle()
        {
            int y = 16;
            lblModoForm = Etiqueta("Nuevo Libro", 16, ref y, bold: true, fontSize: 12f);
            y += 4;

            AgregarCampo("ISBN *", txtISBN, ref y);
            AgregarCampo("Título *", txtTitulo, ref y);
            AgregarCampo("Autor *", txtAutor, ref y);
            AgregarCampo("Editorial", txtEditorial, ref y);

            // Año
            pnlDetalle.Controls.Add(Etiqueta("Año de publicación", 16, ref y));
            nudAnio.SetBounds(16, y, 290, 28); nudAnio.Minimum = 1800; nudAnio.Maximum = DateTime.Now.Year; nudAnio.Value = DateTime.Now.Year;
            pnlDetalle.Controls.Add(nudAnio); y += 40;

            // Stock
            pnlDetalle.Controls.Add(Etiqueta("Stock total *", 16, ref y));
            nudStock.SetBounds(16, y, 290, 28); nudStock.Minimum = 1; nudStock.Maximum = 9999; nudStock.Value = 1;
            pnlDetalle.Controls.Add(nudStock); y += 40;

            // Categoría
            pnlDetalle.Controls.Add(Etiqueta("Categoría *", 16, ref y));
            cmbCategoria.SetBounds(16, y, 290, 28); cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            pnlDetalle.Controls.Add(cmbCategoria); y += 50;

            // Botones guardar/cancelar
            btnGuardar = new Button
            {
                Text      = "Guardar",
                SetBounds_location = new Point(16, y),
                Width     = 135, Height = 35,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.SetBounds(16, y, 135, 35);
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text      = "Cancelar",
                Width     = 135, Height = 35,
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.SetBounds(171, y, 135, 35);
            btnCancelar.Click += (s, e) => OcultarDetalle();

            pnlDetalle.Controls.AddRange(new Control[] { btnGuardar, btnCancelar });
        }

        // ─── Helpers para construir el formulario ──────────────────────────────
        private void AgregarCampo(string etiqueta, TextBox campo, ref int y)
        {
            pnlDetalle.Controls.Add(Etiqueta(etiqueta, 16, ref y));
            campo.SetBounds(16, y, 290, 28);
            campo.Font = new Font("Segoe UI", 9f);
            pnlDetalle.Controls.Add(campo);
            y += 40;
        }

        private Label Etiqueta(string texto, int x, ref int y, bool bold = false, float fontSize = 9f)
        {
            var lbl = new Label
            {
                Text      = texto,
                Location  = new Point(x, y),
                AutoSize  = true,
                Font      = new Font("Segoe UI", fontSize, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            y += 20;
            return lbl;
        }

        private Button CrearBoton(string texto, Color color, int x)
        {
            var btn = new Button
            {
                Text      = texto,
                Location  = new Point(x, 7),
                Width     = 100, Height = 30,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ─── Carga de datos ────────────────────────────────────────────────────
        private void CargarLibros(string? busqueda = null)
        {
            var libros = string.IsNullOrWhiteSpace(busqueda)
                ? _service.ObtenerTodos()
                : _service.Buscar(busqueda);

            dgvLibros.DataSource = libros.Select(l => new
            {
                Id         = l.LibroId,
                ISBN       = l.ISBN,
                Título     = l.Titulo,
                Autor      = l.Autor,
                Categoría  = l.Categoria?.Nombre ?? "-",
                Año        = l.AnioPublicacion,
                Disponible = l.StockDisponible,
                Total      = l.StockTotal,
                Editorial  = l.Editorial
            }).ToList();
        }

        private void CargarCategorias()
        {
            var cats = _context.Categorias.OrderBy(c => c.Nombre).ToList();
            cmbCategoria.DataSource    = cats;
            cmbCategoria.DisplayMember = "Nombre";
            cmbCategoria.ValueMember   = "CategoriaId";
        }

        // ─── Eventos de botones ────────────────────────────────────────────────
        private void BtnBuscar_Click(object? s, EventArgs e)
            => CargarLibros(txtBuscar.Text.Trim());

        private void BtnNuevo_Click(object? s, EventArgs e)
        {
            _idEdicion = null;
            UIHelper.LimpiarFormulario(pnlDetalle);
            nudAnio.Value  = DateTime.Now.Year;
            nudStock.Value = 1;
            lblModoForm.Text = "Nuevo Libro";
            MostrarDetalle();
        }

        private void BtnEditar_Click(object? s, EventArgs e)
        {
            var id = UIHelper.ObtenerIdSeleccionado(dgvLibros);
            if (id == null) { UIHelper.Error("Selecciona un libro para editar."); return; }

            var libro = _service.ObtenerPorId(id.Value);
            if (libro == null) return;

            _idEdicion          = libro.LibroId;
            txtISBN.Text        = libro.ISBN;
            txtTitulo.Text      = libro.Titulo;
            txtAutor.Text       = libro.Autor;
            txtEditorial.Text   = libro.Editorial;
            nudAnio.Value       = libro.AnioPublicacion;
            nudStock.Value      = libro.StockTotal;
            cmbCategoria.SelectedValue = libro.CategoriaId;
            lblModoForm.Text    = "Editar Libro";
            MostrarDetalle();
        }

        private void BtnEliminar_Click(object? s, EventArgs e)
        {
            var id = UIHelper.ObtenerIdSeleccionado(dgvLibros);
            if (id == null) { UIHelper.Error("Selecciona un libro para eliminar."); return; }

            if (!UIHelper.Confirmar("¿Desactivar este libro? No se borrará de la base de datos.")) return;

            var (exito, mensaje) = _service.Eliminar(id.Value);
            if (exito) { UIHelper.Exito(mensaje); CargarLibros(); }
            else UIHelper.Error(mensaje);
        }

        private void BtnGuardar_Click(object? s, EventArgs e)
        {
            var libro = new Libro
            {
                LibroId         = _idEdicion ?? 0,
                ISBN            = txtISBN.Text.Trim(),
                Titulo          = txtTitulo.Text.Trim(),
                Autor           = txtAutor.Text.Trim(),
                Editorial       = txtEditorial.Text.Trim(),
                AnioPublicacion = (int)nudAnio.Value,
                StockTotal      = (int)nudStock.Value,
                CategoriaId     = (int)(cmbCategoria.SelectedValue ?? 0)
            };

            var (exito, mensaje) = _idEdicion == null
                ? _service.Registrar(libro)
                : _service.Actualizar(libro);

            if (exito)
            {
                UIHelper.Exito(mensaje);
                OcultarDetalle();
                CargarLibros();
            }
            else UIHelper.Error(mensaje);
        }

        private void MostrarDetalle()  => pnlDetalle.Visible = true;
        private void OcultarDetalle()  => pnlDetalle.Visible = false;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _context.Dispose();
            base.OnFormClosed(e);
        }
    }
}
