namespace BibliotecaApp.UI.Helpers
{
    
    public static class UIHelper
    {
        // ─── Diálogos ──────────────────────────────────────────────────────────
        public static void Exito(string mensaje, string titulo = "Operación exitosa")
            => MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void Error(string mensaje, string titulo = "Error")
            => MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static bool Confirmar(string mensaje, string titulo = "Confirmar acción")
            => MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
               == DialogResult.Yes;

        // ─── DataGridView estándar ─────────────────────────────────────────────
        public static void EstilizarGrid(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect         = false;
            grid.ReadOnly            = true;
            grid.AllowUserToAddRows  = false;
            grid.RowHeadersVisible   = false;
            grid.BackgroundColor     = Color.White;
            grid.BorderStyle         = BorderStyle.None;
            grid.GridColor           = Color.FromArgb(230, 230, 230);
            grid.Font                = new Font("Segoe UI", 9f);
            grid.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 62, 80);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            grid.DefaultCellStyle.SelectionBackColor    = Color.FromArgb(52, 152, 219);
            grid.DefaultCellStyle.SelectionForeColor    = Color.White;
        }

        // ─── Colores de estado para filas de préstamos ─────────────────────────
        public static Color ColorEstadoPrestamo(string estado, bool estaVencido)
        {
            if (estaVencido) return Color.FromArgb(255, 235, 235); // rojo suave
            return estado switch
            {
                "Devuelto" => Color.FromArgb(235, 255, 235),       // verde suave
                "Activo"   => Color.White,
                _          => Color.FromArgb(255, 248, 220)        // amarillo suave
            };
        }

        // ─── Obtener ID de fila seleccionada en un DataGridView ────────────────
        public static int? ObtenerIdSeleccionado(DataGridView grid, string columnaId = "Id")
        {
            if (grid.SelectedRows.Count == 0) return null;
            var valor = grid.SelectedRows[0].Cells[columnaId].Value;
            return valor != null ? (int)valor : null;
        }

        // ─── Limpiar todos los TextBox e ComboBox de un contenedor ────────────
        public static void LimpiarFormulario(Control contenedor)
        {
            foreach (Control ctrl in contenedor.Controls)
            {
                if (ctrl is TextBox tb) tb.Clear();
                else if (ctrl is ComboBox cb) cb.SelectedIndex = -1;
                else if (ctrl is NumericUpDown num) num.Value = num.Minimum;
                else if (ctrl is DateTimePicker dtp) dtp.Value = DateTime.Now;
                else if (ctrl is GroupBox || ctrl is Panel)
                    LimpiarFormulario(ctrl); // Recursivo para contenedores anidados
            }
        }
    }
}
