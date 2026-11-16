// ONDA Peluquería — módulo ES6 para JavaScript Interop (Fase 7.4).
// La API de portapapeles requiere HTTPS o localhost, y puede solicitar permisos.
export async function copiarCodigo(codigo) {
    if (typeof codigo !== 'string' || !codigo.trim()) {
        throw new Error('No hay un código de reserva para copiar.');
    }

    if (!window.isSecureContext || !navigator.clipboard?.writeText) {
        throw new Error('El navegador no admite el portapapeles seguro en este contexto.');
    }

    await navigator.clipboard.writeText(codigo);
}
