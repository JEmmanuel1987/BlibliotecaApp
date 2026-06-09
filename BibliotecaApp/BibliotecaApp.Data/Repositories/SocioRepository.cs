using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Data.Repositories
{
    public class SocioRepository : IRepository<Socio>
    {
        private readonly BibliotecaContext _context;

        public SocioRepository(BibliotecaContext context)
        {
            _context = context;
        }

        public IEnumerable<Socio> ObtenerTodos()
        {
            return _context.Socios
                .Where(s => s.Activo)
                .OrderBy(s => s.Apellido)
                .ToList();
        }

        public Socio? ObtenerPorId(int id)
        {
            return _context.Socios
                .Include(s => s.Prestamos)
                    .ThenInclude(p => p.Libro)
                .FirstOrDefault(s => s.SocioId == id);
        }

        public Socio? ObtenerPorCarnet(string nroCarnet)
        {
            return _context.Socios
                .FirstOrDefault(s => s.NroCarnet == nroCarnet);
        }

        public IEnumerable<Socio> Buscar(string termino)
        {
            var t = termino.ToLower();
            return _context.Socios
                .Where(s => s.Activo &&
                    (s.Nombre.ToLower().Contains(t) ||
                     s.Apellido.ToLower().Contains(t) ||
                     s.NroCarnet.Contains(t)))
                .OrderBy(s => s.Apellido)
                .ToList();
        }

        public IEnumerable<Socio> ObtenerConDeuda()
        {
            return _context.Socios
                .Where(s => s.Activo && s.DeudaTotal > 0)
                .OrderByDescending(s => s.DeudaTotal)
                .ToList();
        }

        public void Agregar(Socio socio) => _context.Socios.Add(socio);

        public void Actualizar(Socio socio) => _context.Socios.Update(socio);

        public void Eliminar(int id)
        {
            var socio = ObtenerPorId(id);
            if (socio != null)
            {
                socio.Activo = false;
                _context.Socios.Update(socio);
            }
        }

        public void GuardarCambios() => _context.SaveChanges();
    }
}
