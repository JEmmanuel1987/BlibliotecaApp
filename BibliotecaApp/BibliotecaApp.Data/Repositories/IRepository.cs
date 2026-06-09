namespace BibliotecaApp.Data.Repositories
{
    // Contrato genérico: cualquier repositorio concreto debe implementar estas operaciones CRUD
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> ObtenerTodos();
        T? ObtenerPorId(int id);
        void Agregar(T entidad);
        void Actualizar(T entidad);
        void Eliminar(int id);
        void GuardarCambios();
    }
}
