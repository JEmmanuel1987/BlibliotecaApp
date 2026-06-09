using BibliotecaApp.Data;
using BibliotecaApp.UI.Forms;
using Microsoft.EntityFrameworkCore;

// ─── Punto de entrada de la aplicación ────────────────────────────────────────
ApplicationConfiguration.Initialize();


try
{
    using var context = new BibliotecaContext();
    context.Database.Migrate(); // Crea la BD si no existe y aplica todas las migraciones
}
catch (Exception ex)
{
    MessageBox.Show(
        $"Error al conectar con la base de datos:\n\n{ex.Message}\n\nVerifica la cadena de conexión en BibliotecaContext.cs",
        "Error de conexión",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    return;
}

Application.Run(new FrmPrincipal());
