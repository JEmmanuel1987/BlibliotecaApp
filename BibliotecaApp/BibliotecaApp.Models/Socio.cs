namespace BibliotecaApp.Models
{
    public class Socio
    {
        public int SocioId { get; set; }
        public string NroCarnet { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;
        public decimal DeudaTotal { get; set; } = 0;

        // Navegación
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

        // Propiedad calculada (no mapeada)
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public bool TieneDeuda => DeudaTotal > 0;
    }
}
