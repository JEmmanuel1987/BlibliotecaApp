using BibliotecaApp.Data.Repositories;
using BibliotecaApp.Models;

namespace BibliotecaApp.BLL
{
    
    public class LibroService
    {
        private readonly LibroRepository _repo;

        public LibroService(LibroRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Libro> ObtenerTodos() => _repo.ObtenerTodos();
        public IEnumerable<Libro> Buscar(string termino) => _repo.Buscar(termino);
        public IEnumerable<Libro> ObtenerDisponibles() => _repo.ObtenerDisponibles();
        public Libro? ObtenerPorId(int id) => _repo.ObtenerPorId(id);

        public (bool exito, string mensaje) Registrar(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo))
                return (false, "El título es obligatorio.");

            if (string.IsNullOrWhiteSpace(libro.ISBN))
                return (false, "El ISBN es obligatorio.");

            if (_repo.ObtenerPorISBN(libro.ISBN) != null)
                return (false, $"Ya existe un libro con ISBN '{libro.ISBN}'.");

            if (libro.StockTotal <= 0)
                return (false, "El stock debe ser mayor a cero.");

            libro.StockDisponible = libro.StockTotal;

            _repo.Agregar(libro);
            _repo.GuardarCambios();
            return (true, "Libro registrado correctamente.");
        }

        public (bool exito, string mensaje) Actualizar(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo))
                return (false, "El título es obligatorio.");

            var existente = _repo.ObtenerPorISBN(libro.ISBN);
            if (existente != null && existente.LibroId != libro.LibroId)
                return (false, $"Otro libro ya usa el ISBN '{libro.ISBN}'.");

            _repo.Actualizar(libro);
            _repo.GuardarCambios();
            return (true, "Libro actualizado correctamente.");
        }

        public (bool exito, string mensaje) Eliminar(int id)
        {
            var libro = _repo.ObtenerPorId(id);
            if (libro == null)
                return (false, "Libro no encontrado.");

            if (libro.StockDisponible < libro.StockTotal)
                return (false, "No se puede eliminar: hay ejemplares prestados actualmente.");

            _repo.Eliminar(id);
            _repo.GuardarCambios();
            return (true, "Libro desactivado correctamente.");
        }
    }
}
