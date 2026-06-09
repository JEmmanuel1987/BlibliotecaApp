using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Data.Repositories
{
    public class LibroRepository : IRepository<Libro>
    {
        private readonly BibliotecaContext _context;

        public LibroRepository(BibliotecaContext context)
        {
            _context = context;
        }

        public IEnumerable<Libro> ObtenerTodos()
        {
            return _context.Libros
                .Include(l => l.Categoria)
                .Where(l => l.Activo)
                .OrderBy(l => l.Titulo)
                .ToList();
        }

        public Libro? ObtenerPorId(int id)
        {
            return _context.Libros
                .Include(l => l.Categoria)
                .FirstOrDefault(l => l.LibroId == id);
        }

        public Libro? ObtenerPorISBN(string isbn)
        {
            return _context.Libros
                .Include(l => l.Categoria)
                .FirstOrDefault(l => l.ISBN == isbn);
        }

        // Búsqueda por título, autor o ISBN (para el buscador del formulario)
        public IEnumerable<Libro> Buscar(string termino)
        {
            var t = termino.ToLower();
            return _context.Libros
                .Include(l => l.Categoria)
                .Where(l => l.Activo &&
                    (l.Titulo.ToLower().Contains(t) ||
                     l.Autor.ToLower().Contains(t) ||
                     l.ISBN.Contains(t)))
                .OrderBy(l => l.Titulo)
                .ToList();
        }

        public IEnumerable<Libro> ObtenerDisponibles()
        {
            return _context.Libros
                .Include(l => l.Categoria)
                .Where(l => l.Activo && l.StockDisponible > 0)
                .OrderBy(l => l.Titulo)
                .ToList();
        }

        public void Agregar(Libro libro)
        {
            _context.Libros.Add(libro);
        }

        public void Actualizar(Libro libro)
        {
            _context.Libros.Update(libro);
        }

        public void Eliminar(int id)
        {
            // Baja lógica: no se borra de la BD
            var libro = ObtenerPorId(id);
            if (libro != null)
            {
                libro.Activo = false;
                _context.Libros.Update(libro);
            }
        }

        public void GuardarCambios() => _context.SaveChanges();
    }
}
