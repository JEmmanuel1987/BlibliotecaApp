namespace BibliotecaApp.Models
{
    public class Libro
    {
        public int LibroId { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string Editorial { get; set; } = string.Empty;
        public int StockTotal { get; set; }
        public int StockDisponible { get; set; }
        public bool Activo { get; set; } = true;

        // Clave foránea
        public int CategoriaId { get; set; }

        // Navegación
        public Categoria? Categoria { get; set; }
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
