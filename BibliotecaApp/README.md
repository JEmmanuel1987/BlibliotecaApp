# Sistema de Gestión de Biblioteca
## .NET 8 · Windows Forms · Entity Framework Core · SQL Server

---

## Estructura del proyecto

```
BibliotecaApp/
├── BibliotecaApp.Models/          ← Entidades del dominio
│   ├── Libro.cs
│   ├── Socio.cs
│   ├── Prestamo.cs
│   └── Categoria.cs
│
├── BibliotecaApp.Data/            ← Capa de acceso a datos
│   ├── BibliotecaContext.cs
│   └── Repositories/
│       ├── IRepository.cs
│       ├── LibroRepository.cs
│       ├── SocioRepository.cs
│       └── PrestamoRepository.cs
│
├── BibliotecaApp.BLL/             ← Lógica de negocio
│   ├── LibroService.cs
│   ├── SocioService.cs
│   └── PrestamoService.cs         ← Incluye cálculo de multas
│
└── BibliotecaApp.UI/              ← Interfaz de usuario
    ├── Forms/
    │   ├── FrmPrincipal.cs        ← Menú principal / Dashboard
    │   ├── FrmLibros.cs           ← CRUD libros
    │   ├── FrmSocios.cs           ← CRUD socios
    │   └── FrmPrestamos.cs        ← Préstamos y devoluciones
    └── Helpers/
        └── UIHelper.cs


