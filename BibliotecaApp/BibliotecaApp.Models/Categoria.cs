namespace BibliotecaApp.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        // Navegación
        public ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}
