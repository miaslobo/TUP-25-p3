export function mostrarToast(mensaje) {
    const elMensaje = document.getElementById('toastMensaje');
    const elToast = document.getElementById('toastNotificacion');

    if (!elMensaje || !elToast) return;

    elMensaje.innerText = mensaje;

    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(elToast);
    toastBootstrap.show();
}