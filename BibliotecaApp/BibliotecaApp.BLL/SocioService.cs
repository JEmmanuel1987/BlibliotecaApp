using BibliotecaApp.Data.Repositories;
using BibliotecaApp.Models;

namespace BibliotecaApp.BLL
{
    public class SocioService
    {
        private readonly SocioRepository _repo;

        public SocioService(SocioRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Socio> ObtenerTodos() => _repo.ObtenerTodos();
        public IEnumerable<Socio> Buscar(string termino) => _repo.Buscar(termino);
        public IEnumerable<Socio> ObtenerConDeuda() => _repo.ObtenerConDeuda();
        public Socio? ObtenerPorId(int id) => _repo.ObtenerPorId(id);

        public (bool exito, string mensaje) Registrar(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.NroCarnet))
                return (false, "El número de carnet es obligatorio.");

            if (_repo.ObtenerPorCarnet(socio.NroCarnet) != null)
                return (false, $"Ya existe un socio con carnet '{socio.NroCarnet}'.");

            if (string.IsNullOrWhiteSpace(socio.Nombre) || string.IsNullOrWhiteSpace(socio.Apellido))
                return (false, "Nombre y apellido son obligatorios.");

            socio.FechaRegistro = DateTime.Now;
            _repo.Agregar(socio);
            _repo.GuardarCambios();
            return (true, "Socio registrado correctamente.");
        }

        public (bool exito, string mensaje) Actualizar(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.Nombre) || string.IsNullOrWhiteSpace(socio.Apellido))
                return (false, "Nombre y apellido son obligatorios.");

            var existente = _repo.ObtenerPorCarnet(socio.NroCarnet);
            if (existente != null && existente.SocioId != socio.SocioId)
                return (false, $"Otro socio ya usa el carnet '{socio.NroCarnet}'.");

            _repo.Actualizar(socio);
            _repo.GuardarCambios();
            return (true, "Socio actualizado correctamente.");
        }

        public (bool exito, string mensaje) PagarDeuda(int socioId)
        {
            var socio = _repo.ObtenerPorId(socioId);
            if (socio == null) return (false, "Socio no encontrado.");
            if (socio.DeudaTotal <= 0) return (false, "El socio no tiene deuda pendiente.");

            socio.DeudaTotal = 0;
            _repo.Actualizar(socio);
            _repo.GuardarCambios();
            return (true, "Deuda saldada correctamente.");
        }

        public (bool exito, string mensaje) Eliminar(int id)
        {
            var socio = _repo.ObtenerPorId(id);
            if (socio == null) return (false, "Socio no encontrado.");

            if (socio.TieneDeuda)
                return (false, "No se puede eliminar: el socio tiene deuda pendiente.");

            bool tienePrestamosActivos = socio.Prestamos
                .Any(p => p.Estado == Models.EstadoPrestamo.Activo);

            if (tienePrestamosActivos)
                return (false, "No se puede eliminar: el socio tiene préstamos activos.");

            _repo.Eliminar(id);
            _repo.GuardarCambios();
            return (true, "Socio desactivado correctamente.");
        }
    }
}
