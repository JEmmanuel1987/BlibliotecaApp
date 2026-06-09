namespace BibliotecaApp.Models
{
    public enum EstadoPrestamo
    {
        Activo,
        Devuelto,
        Vencido
    }

    public class Prestamo
    {
        public int PrestamoId { get; set; }
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Activo;
        public decimal MultaGenerada { get; set; } = 0;
        public bool MultaPagada { get; set; } = false;
        public string? Observaciones { get; set; }

        // Claves foráneas
        public int LibroId { get; set; }
        public int SocioId { get; set; }

        // Navegación
        public Libro? Libro { get; set; }
        public Socio? Socio { get; set; }

        // Propiedad calculada
        public int DiasRetraso
        {
            get
            {
                if (Estado == EstadoPrestamo.Devuelto) return 0;
                var hoy = DateTime.Now.Date;
                var vencimiento = FechaDevolucionEsperada.Date;
                return hoy > vencimiento ? (int)(hoy - vencimiento).TotalDays : 0;
            }
        }

        public bool EstaVencido => DateTime.Now.Date > FechaDevolucionEsperada.Date
                                   && Estado == EstadoPrestamo.Activo;
    }
}
