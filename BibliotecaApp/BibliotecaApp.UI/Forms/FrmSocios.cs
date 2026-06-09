using BibliotecaApp.BLL;
using BibliotecaApp.Data;
using BibliotecaApp.Data.Repositories;
using BibliotecaApp.Models;
using BibliotecaApp.UI.Helpers;

namespace BibliotecaApp.UI.Forms
{
    public class FrmSocios : Form
    {
        private readonly SocioService _service;
        private readonly BibliotecaContext _context;

        private DataGridView dgvSocios   = new();
        private TextBox      txtBuscar   = new();
        private Button       btnNuevo    = new();
        private Button       btnEditar   = new();
        private Button       btnEliminar = new();
        private Button       btnPagarDeuda = new();
        private Panel        pnlDetalle  = new();

        private TextBox    txtCarnet    = new();
        private TextBox    txtNombre    = new();
        private TextBox    txtApellido  = new();
        private TextBox    txtTelefono  = new();
        private TextBox    txtEmail     = new();
        private Label      lblModoForm  = new();
        private Button     btnGuardar   = new();

        private int? _idEdicion = null;

        public FrmSocios()
        {
            _context = new BibliotecaContext();
            _service = new SocioService(new SocioRepository(_context));
            InicializarComponentes();
            CargarSocios();
        }

        private void InicializarComponentes()
        {
            Text          = "Gestión de Socios";
            Size          = new Size(1100, 660);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor     = Color.FromArgb(245, 247, 249);
            Font          = new Font("Segoe UI", 9f);

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top, Height = 55,
                BackColor = Color.FromArgb(45, 62, 80), Padding = new Padding(12, 10, 12, 10)
            };
            new Label { Text = "👥  Gestión de Socios", ForeColor = Color.White, Font = new Font("Segoe UI", 13f, FontStyle.Bold), AutoSize = true, Location = new Point(12, 13), Parent = pnlTop };

            txtBuscar = new TextBox { PlaceholderText = "Buscar por nombre o carnet…", Width = 280, Height = 28, Location = new Point(680, 13), Parent = pnlTop };
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) CargarSocios(txtBuscar.Text); };
            var btnB = new Button { Text = "Buscar", Width = 75, Height = 28, Location = new Point(968, 13), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Parent = pnlTop };
            btnB.FlatAppearance.BorderSize = 0;
            btnB.Click += (s, e) => CargarSocios(txtBuscar.Text);

            var pnlBotones = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.White, Padding = new Padding(10, 7, 10, 7) };
            btnNuevo    = Btn("＋ Nuevo",       Color.FromArgb(46, 204, 113),  0);
            btnEditar   = Btn("✎ Editar",       Color.FromArgb(52, 152, 219),  110);
            btnEliminar = Btn("✕ Eliminar",     Color.FromArgb(231, 76, 60),   220);
            btnPagarDeuda = Btn("💰 Pagar deuda", Color.FromArgb(230, 126, 34), 330);

            btnNuevo.Click    += (s, e) => { _idEdicion = null; UIHelper.LimpiarFormulario(pnlDetalle); lblModoForm.Text = "Nuevo Socio"; pnlDetalle.Visible = true; };
            btnEditar.Click   += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnPagarDeuda.Click += BtnPagarDeuda_Click;
            pnlBotones.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnEliminar, btnPagarDeuda });

            dgvSocios = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.EstilizarGrid(dgvSocios);
            dgvSocios.DataBindingComplete += ColorearDeudores;

            pnlDetalle = new Panel { Width = 330, Dock = DockStyle.Right, BackColor = Color.White, Padding = new Padding(16), Visible = false };
            ConstruirDetalle();

            var pnlC = new Panel { Dock = DockStyle.Fill };
            pnlC.Controls.Add(dgvSocios);
            pnlC.Controls.Add(pnlDetalle);
            Controls.Add(pnlC);
            Controls.Add(pnlBotones);
            Controls.Add(pnlTop);
        }

        private void ConstruirDetalle()
        {
            int y = 16;
            lblModoForm = L("Nuevo Socio", ref y, bold: true, size: 12f);
            y += 4;
            Campo("Nro. Carnet *", txtCarnet, ref y);
            Campo("Nombre *", txtNombre, ref y);
            Campo("Apellido *", txtApellido, ref y);
            Campo("Teléfono", txtTelefono, ref y);
            Campo("Email", txtEmail, ref y);
            y += 8;

            btnGuardar = new Button { Text = "Guardar", BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.SetBounds(16, y, 140, 35);
            btnGuardar.Click += BtnGuardar_Click;

            var btnC = new Button { Text = "Cancelar", BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnC.FlatAppearance.BorderSize = 0;
            btnC.SetBounds(166, y, 130, 35);
            btnC.Click += (s, e) => pnlDetalle.Visible = false;
            pnlDetalle.Controls.AddRange(new Control[] { btnGuardar, btnC });
        }

        private void Campo(string label, TextBox txt, ref int y)
        {
            pnlDetalle.Controls.Add(L(label, ref y));
            txt.SetBounds(16, y, 290, 28);
            txt.Font = new Font("Segoe UI", 9f);
            pnlDetalle.Controls.Add(txt);
            y += 40;
        }

        private Label L(string text, ref int y, bool bold = false, float size = 9f)
        {
            var lbl = new Label { Text = text, Location = new Point(16, y), AutoSize = true, Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular), ForeColor = Color.FromArgb(52, 73, 94) };
            y += 20;
            return lbl;
        }

        private Button Btn(string text, Color color, int x)
        {
            var b = new Button { Text = text, Location = new Point(x, 7), Width = 110, Height = 30, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f) };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void CargarSocios(string? busqueda = null)
        {
            var socios = string.IsNullOrWhiteSpace(busqueda)
                ? _service.ObtenerTodos()
                : _service.Buscar(busqueda);

            dgvSocios.DataSource = socios.Select(s => new
            {
                Id           = s.SocioId,
                Carnet       = s.NroCarnet,
                Nombre       = s.NombreCompleto,
                Teléfono     = s.Telefono ?? "—",
                Email        = s.Email ?? "—",
                Registrado   = s.FechaRegistro.ToString("dd/MM/yyyy"),
                DeudaBs      = s.DeudaTotal > 0 ? $"Bs. {s.DeudaTotal:F2}" : "—",
                _tieneDeuda  = s.TieneDeuda
            }).ToList();

            if (dgvSocios.Columns.Contains("_tieneDeuda"))
                dgvSocios.Columns["_tieneDeuda"]!.Visible = false;
        }

        private void ColorearDeudores(object? s, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvSocios.Rows)
            {
                bool deuda = row.Cells["_tieneDeuda"]?.Value is true;
                if (deuda) row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
            }
        }

        private void BtnEditar_Click(object? s, EventArgs e)
        {
            var id = UIHelper.ObtenerIdSeleccionado(dgvSocios);
            if (id == null) { UIHelper.Error("Selecciona un socio."); return; }
            var socio = _service.ObtenerPorId(id.Value);
            if (socio == null) return;
            _idEdicion = socio.SocioId;
            txtCarnet.Text   = socio.NroCarnet;
            txtNombre.Text   = socio.Nombre;
            txtApellido.Text = socio.Apellido;
            txtTelefono.Text = socio.Telefono ?? "";
            txtEmail.Text    = socio.Email ?? "";
            lblModoForm.Text = "Editar Socio";
            pnlDetalle.Visible = true;
        }

        private void BtnGuardar_Click(object? s, EventArgs e)
        {
            var socio = new Socio
            {
                SocioId  = _idEdicion ?? 0,
                NroCarnet = txtCarnet.Text.Trim(),
                Nombre   = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Email    = txtEmail.Text.Trim()
            };
            var (exito, msg) = _idEdicion == null ? _service.Registrar(socio) : _service.Actualizar(socio);
            if (exito) { UIHelper.Exito(msg); pnlDetalle.Visible = false; CargarSocios(); }
            else UIHelper.Error(msg);
        }

        private void BtnEliminar_Click(object? s, EventArgs e)
        {
            var id = UIHelper.ObtenerIdSeleccionado(dgvSocios);
            if (id == null) { UIHelper.Error("Selecciona un socio."); return; }
            if (!UIHelper.Confirmar("¿Desactivar este socio?")) return;
            var (exito, msg) = _service.Eliminar(id.Value);
            if (exito) { UIHelper.Exito(msg); CargarSocios(); } else UIHelper.Error(msg);
        }

        private void BtnPagarDeuda_Click(object? s, EventArgs e)
        {
            var id = UIHelper.ObtenerIdSeleccionado(dgvSocios);
            if (id == null) { UIHelper.Error("Selecciona un socio."); return; }
            if (!UIHelper.Confirmar("¿Registrar pago total de la deuda del socio?")) return;
            var (exito, msg) = _service.PagarDeuda(id.Value);
            if (exito) { UIHelper.Exito(msg); CargarSocios(); } else UIHelper.Error(msg);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { _context.Dispose(); base.OnFormClosed(e); }
    }
}
