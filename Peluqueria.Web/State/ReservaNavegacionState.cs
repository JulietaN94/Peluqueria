namespace Peluqueria.Web.State;

/// <summary>
/// Borrador temporal de reservas para un circuito de Blazor Server.
/// No reemplaza el backend, no guarda datos personales y no persiste en SQL Server.
/// </summary>
public sealed class ReservaNavegacionState
{
    public event Action? Cambio;

    public int? ServicioId { get; private set; }
    public string? ServicioNombre { get; private set; }
    public int? ProfesionalId { get; private set; }
    public bool ProfesionalDefinido { get; private set; }
    public DateTime? FechaSeleccionada { get; private set; }
    public DateTime? HoraSeleccionada { get; private set; }
    public string? UltimoCodigoConfirmado { get; private set; }

    public bool TieneReservaEnCurso => ServicioId.HasValue;

    public void SeleccionarServicio(int id, string nombre)
    {
        ServicioId = id;
        ServicioNombre = nombre;
        ProfesionalId = null;
        ProfesionalDefinido = false;
        FechaSeleccionada = null;
        HoraSeleccionada = null;
        UltimoCodigoConfirmado = null;
        Cambio?.Invoke();
    }

    // null es una opción válida: "Cualquier profesional".
    public void SeleccionarProfesional(int? id)
    {
        ProfesionalId = id;
        ProfesionalDefinido = true;
        HoraSeleccionada = null;
        Cambio?.Invoke();
    }

    public void SeleccionarFecha(DateTime fecha)
    {
        FechaSeleccionada = fecha.Date;
        HoraSeleccionada = null;
        Cambio?.Invoke();
    }

    public void SeleccionarHora(DateTime hora)
    {
        HoraSeleccionada = hora;
        Cambio?.Invoke();
    }

    public void LimpiarHora()
    {
        if (HoraSeleccionada is null) return;
        HoraSeleccionada = null;
        Cambio?.Invoke();
    }

    public void ConfirmarReserva(string codigo)
    {
        LimpiarBorradorInterno();
        UltimoCodigoConfirmado = codigo;
        Cambio?.Invoke();
    }

    public void Reiniciar()
    {
        LimpiarBorradorInterno();
        UltimoCodigoConfirmado = null;
        Cambio?.Invoke();
    }

    private void LimpiarBorradorInterno()
    {
        ServicioId = null;
        ServicioNombre = null;
        ProfesionalId = null;
        ProfesionalDefinido = false;
        FechaSeleccionada = null;
        HoraSeleccionada = null;
    }
}
