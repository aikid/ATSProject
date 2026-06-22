// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ---- Sidebar toggle ----
document.addEventListener('DOMContentLoaded', function () {
    var btn = document.getElementById('sidebarToggle');
    if (!btn) return;

    var sidebar = document.getElementById('sidebar');
    var wrapper = document.getElementById('mainWrapper');

    var collapsed = localStorage.getItem('ats_sidebar') === 'collapsed';
    if (collapsed) {
        sidebar.classList.add('collapsed');
        wrapper.classList.add('expanded');
    }

    btn.addEventListener('click', function () {
        sidebar.classList.toggle('collapsed');
        wrapper.classList.toggle('expanded');
        localStorage.setItem('ats_sidebar', sidebar.classList.contains('collapsed') ? 'collapsed' : 'open');
    });
});

// ---- Logout ----
function logout() {
    window.location.href = '/Autenticacao/Logout';
}

// ---- Visualizar/ocultar senha ----
function visualizarSenha(id) {
    var input = document.getElementById(id);
    var icon  = document.getElementById('iconOlhoSenha');
    if (!input) return;
    if (input.type === 'password') {
        input.type = 'text';
        if (icon) { icon.classList.remove('bi-eye-slash'); icon.classList.add('bi-eye'); }
    } else {
        input.type = 'password';
        if (icon) { icon.classList.remove('bi-eye'); icon.classList.add('bi-eye-slash'); }
    }
}

// ---- Notificação (SweetAlert2) ----
function notificar(redirecionar, titulo, mensagem, tipo, url) {
    Swal.fire({ title: titulo, text: mensagem, icon: tipo, confirmButtonColor: '#4f46e5' }).then(function () {
        if (redirecionar && url) window.location.href = url;
    });
}

