function inicializarFiltros() {
    const boton = document.getElementById("toggleFiltros");
    const panel = document.getElementById("panelFiltros");
    if (!boton || !panel) return;
    boton.addEventListener("click", () => panel.classList.toggle("d-none"));
}

function inicializarSliderImagenes() {
    const items = document.querySelectorAll("[data-slider-item]");
    if (items.length === 0) return;
    let indice = 0;
    setInterval(() => {
        items.forEach((el, idx) => el.classList.toggle("d-none", idx !== indice));
        indice = (indice + 1) % items.length;
    }, 3500);
}

function confirmarEliminacion(mensaje) {
    return window.confirm(mensaje || "Confirma esta accion.");
}

async function toggleFavorita(propiedadId) {
    const meta = document.querySelector('meta[name="request-verification-token"]');
    const token = meta ? meta.getAttribute("content") : null;
    const headers = { "RequestVerificationToken": token || "" };
    const response = await fetch(`/Cliente/ToggleFavorita?propiedadId=${propiedadId}`, { method: "POST", headers });
    return response.ok ? await response.json() : null;
}

document.addEventListener("DOMContentLoaded", () => {
    inicializarFiltros();
    inicializarSliderImagenes();
});
