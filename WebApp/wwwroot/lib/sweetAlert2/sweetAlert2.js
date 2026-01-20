// Popups
const popupSwal = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-lg btn-light custom-button-popup',
        cancelButton: 'btn btn-lg btn-danger custom-button-popup',
        popup: 'custom-popup'
    },
    buttonsStyling: false
});

// Toasts
const toastSwal = Swal.mixin({
    toast: true,
    position: 'top-end',
    iconColor: 'white',
    customClass: {
        popup: 'colored-toast',
    },
    showConfirmButton: false,
    timer: 7000,
    timerProgressBar: true,
});

// Caso 'popup' seja passado como 'true', será exibido um POPUP. Caso 'false', será exibido um TOAST.
function notificar(popup, titulo, mensagem, icone, caminho) {
    if (popup) {
        popupSwal.fire({
            title: `${titulo}`
            , text: `${mensagem}`
            , icon: `${icone}`
        }).then(() => {
            if (caminho) { window.location.href = `${caminho}`; }
        })
    } else {
        toastSwal.fire({
            title: `${titulo}`
            , text: `${mensagem}`
            , icon: `${icone}`
        });

        if (caminho) {
            const toastEl = Swal.getPopup();

            if (toastEl) {
                toastEl.style.cursor = "pointer";

                toastEl.addEventListener("click", () => {
                    window.location.href = `${caminho}`;
                }, { once: true });
            }
        }
    }
}