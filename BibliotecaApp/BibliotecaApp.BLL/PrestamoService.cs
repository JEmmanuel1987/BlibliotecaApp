using BibliotecaApp.Data.Repositories;
using BibliotecaApp.Models;

namespace BibliotecaApp.BLL
{
   
    public class PrestamoService
    {
        private readonly PrestamoRepository _repoP;
        private readonly LibroRepository _repoL;
        private readonly SocioRepository _repoS;

        private const int    DIAS_PRESTAMO    = 15;
        private const decimal MULTA_POR_DIA   = 2.50m; // Bolivianos

        public PrestamoService(
            PrestamoRepository repoP,
            LibroRepository repoL,
            SocioRepository repoS)
        {
            _repoP = repoP;
            _repoL = repoL;
            _repoS = repoS;
        }

        // ─── Consultas ──────────────────────────────────────────────────────────
        public IEnumerable<Prestamo> ObtenerTodos()    => _repoP.ObtenerTodos();
        public IEnumerable<Prestamo> ObtenerActivos()  => _repoP.ObtenerActivos();
        public IEnumerable<Prestamo> ObtenerVencidos() => _repoP.ObtenerVencidos();
        public Prestamo? ObtenerPorId(int id)          => _repoP.ObtenerPorId(id);

        public IEnumerable<Prestamo> ObtenerPorSocio(int socioId)
            => _repoP.ObtenerPorSocio(socioId);

        // ─── Calcular multa de un préstamo ─────────────────────────────────────
        public decimal CalcularMulta(Prestamo prestamo)
        {
            if (prestamo.Estado == EstadoPrestamo.Devuelto)
                return prestamo.MultaGenerada;

            var diasRetraso = prestamo.DiasRetraso;
            return diasRetraso > 0 ? diasRetraso * MULTA_POR_DIA : 0;
        }

        // ─── Registrar un nuevo préstamo ────────────────────────────────────────
        public (bool exito, string mensaje) RegistrarPrestamo(int socioId, int libroId, string? observaciones = null)
        {
            // Validar socio
            var socio = _repoS.ObtenerPorId(socioId);
            if (socio == null)
                return (false, "Socio no encontrado.");

            if (!socio.Activo)
                return (false, "El socio está desactivado.");

            if (socio.TieneDeuda)
                return (false, $"El socio tiene una deuda de Bs. {socio.DeudaTotal:F2}. Debe saldarla antes de realizar un nuevo préstamo.");

            // Validar libro
            var libro = _repoL.ObtenerPorId(libroId);
            if (libro == null)
                return (false, "Libro no encontrado.");

            if (!libro.Activo)
                return (false, "El libro está desactivado.");

            if (libro.StockDisponible <= 0)
                return (false, $"No hay ejemplares disponibles de \"{libro.Titulo}\".");

            // Validar que no tenga el mismo libro prestado
            if (_repoP.SocioTieneLibroPrestado(socioId, libroId))
                return (false, $"El socio ya tiene prestado \"{libro.Titulo}\".");

            // Crear préstamo
            var prestamo = new Prestamo
            {
                SocioId                  = socioId,
                LibroId                  = libroId,
                FechaPrestamo            = DateTime.Now,
                FechaDevolucionEsperada  = DateTime.Now.AddDays(DIAS_PRESTAMO),
                Estado                   = EstadoPrestamo.Activo,
                Observaciones            = observaciones
            };

            // Descontar stock
            libro.StockDisponible--;
            _repoL.Actualizar(libro);

            _repoP.Agregar(prestamo);
            _repoP.GuardarCambios();

            return (true, $"Préstamo registrado. Devolución esperada: {prestamo.FechaDevolucionEsperada:dd/MM/yyyy}");
        }

        // ─── Registrar devolución ───────────────────────────────────────────────
        public (bool exito, string mensaje, decimal multa) RegistrarDevolucion(int prestamoId)
        {
            var prestamo = _repoP.ObtenerPorId(prestamoId);
            if (prestamo == null)
                return (false, "Préstamo no encontrado.", 0);

            if (prestamo.Estado == EstadoPrestamo.Devuelto)
                return (false, "Este préstamo ya fue devuelto.", 0);

            // Calcular multa al momento de devolución
            decimal multa = CalcularMulta(prestamo);
            prestamo.MultaGenerada     = multa;
            prestamo.FechaDevolucionReal = DateTime.Now;
            prestamo.Estado            = EstadoPrestamo.Devuelto;

            // Restaurar stock del libro
            var libro = _repoL.ObtenerPorId(prestamo.LibroId);
            if (libro != null)
            {
                libro.StockDisponible++;
                _repoL.Actualizar(libro);
            }

            // Acumular deuda al socio si hay multa
            if (multa > 0)
            {
                var socio = _repoS.ObtenerPorId(prestamo.SocioId);
                if (socio != null)
                {
                    socio.DeudaTotal += multa;
                    _repoS.Actualizar(socio);
                }
            }

            _repoP.Actualizar(prestamo);
            _repoP.GuardarCambios();

            string mensaje = multa > 0
                ? $"Devolución registrada con multa de Bs. {multa:F2} ({prestamo.DiasRetraso} días de retraso)."
                : "Devolución registrada a tiempo. ¡Sin multa!";

            return (true, mensaje, multa);
        }

        // ─── Actualizar estados vencidos (llamar al inicio del día) ─────────────
        public int ActualizarEstadosVencidos()
        {
            var vencidos = _repoP.ObtenerVencidos().ToList();
            foreach (var p in vencidos)
            {
                p.Estado = EstadoPrestamo.Vencido;
                _repoP.Actualizar(p);
            }
            if (vencidos.Any()) _repoP.GuardarCambios();
            return vencidos.Count;
        }

        // ─── Resumen para el dashboard ──────────────────────────────────────────
        public ResumenPrestamos ObtenerResumen()
        {
            var todos = _repoP.ObtenerTodos().ToList();
            return new ResumenPrestamos
            {
                TotalActivos   = todos.Count(p => p.Estado == EstadoPrestamo.Activo),
                TotalVencidos  = todos.Count(p => p.EstaVencido),
                TotalDevueltos = todos.Count(p => p.Estado == EstadoPrestamo.Devuelto),
                MultasPendientes = todos
                    .Where(p => p.MultaGenerada > 0 && !p.MultaPagada)
                    .Sum(p => p.MultaGenerada)
            };
        }
    }

    // ─── DTO para el Dashboard ─────────────────────────────────────────────────
    public class ResumenPrestamos
    {
        public int TotalActivos { get; set; }
        public int TotalVencidos { get; set; }
        public int TotalDevueltos { get; set; }
        public decimal MultasPendientes { get; set; }
    }
}
