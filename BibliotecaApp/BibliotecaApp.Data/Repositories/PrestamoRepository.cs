using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Data.Repositories
{
    public class PrestamoRepository : IRepository<Prestamo>
    {
        private readonly BibliotecaContext _context;

        public PrestamoRepository(BibliotecaContext context)
        {
            _context = context;
        }

        public IEnumerable<Prestamo> ObtenerTodos()
        {
            return _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Socio)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToList();
        }

        public Prestamo? ObtenerPorId(int id)
        {
            return _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Socio)
                .FirstOrDefault(p => p.PrestamoId == id);
        }

        public IEnumerable<Prestamo> ObtenerActivos()
        {
            return _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Socio)
                .Where(p => p.Estado == EstadoPrestamo.Activo)
                .OrderBy(p => p.FechaDevolucionEsperada)
                .ToList();
        }

        public IEnumerable<Prestamo> ObtenerVencidos()
        {
            var hoy = DateTime.Now.Date;
            return _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Socio)
                .Where(p => p.Estado == EstadoPrestamo.Activo
                         && p.FechaDevolucionEsperada.Date < hoy)
                .OrderBy(p => p.FechaDevolucionEsperada)
                .ToList();
        }

        public IEnumerable<Prestamo> ObtenerPorSocio(int socioId)
        {
            return _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.SocioId == socioId)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToList();
        }

        // ¿El socio ya tiene este libro prestado y no lo devolvió?
        public bool SocioTieneLibroPrestado(int socioId, int libroId)
        {
            return _context.Prestamos.Any(p =>
                p.SocioId == socioId &&
                p.LibroId == libroId &&
                p.Estado == EstadoPrestamo.Activo);
        }

        public void Agregar(Prestamo prestamo) => _context.Prestamos.Add(prestamo);

        public void Actualizar(Prestamo prestamo) => _context.Prestamos.Update(prestamo);

        public void Eliminar(int id) { /* Los préstamos no se eliminan, solo se devuelven */ }

        public void GuardarCambios() => _context.SaveChanges();
    }
}
