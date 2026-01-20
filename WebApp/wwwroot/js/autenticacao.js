//* -------------------- Validação do E-mail ou CPF -------------------- */
function validarCredenciaisAcesso(id_campo, id_feedback) {
    var campo = document.getElementById(id_campo);
    var div_feedback = document.getElementById(id_feedback);

    if (campo.value) {
        // Verifica se é um e-mail.
        if (campo.value.includes("@")) {
            if (!validarEmail(campo.value)) {
                exibirFeedback(div_feedback, 'Por favor, informe um e-mail válido.', campo);
            } else {
                limparFeedback(div_feedback, campo);
            }
        } else {
            // Se não for um e-mail, assume que é um CPF.
            if (!validarCPF(campo.value)) {
                exibirFeedback(div_feedback, 'Por favor, informe um CPF válido.', campo);
            } else {
                limparFeedback(div_feedback, campo);
            }
        }
    }

    return;
}

//* -------------------- Cadastro de Senha -------------------- */
function cadastrarSenha(event, hash_recuperacao = null) { // "hash_recuperacao" é um parâmetro que indica se será uma RECUPERAÇÃO de senha.
    event.preventDefault();

    var novaSenha = document.getElementById("txtNovaSenha");
    var confirmacao = document.getElementById("txtNovaSenhaConfirmacao");
    var div_feedback = document.getElementById("feedbackInvalido");
    var div_feedbackCriterios = document.getElementById("feedbackInvalidoCriterios");

    if (!novaSenha.value || !confirmacao.value) {
        exibirFeedback(div_feedback, '', novaSenha);
        exibirFeedback(div_feedback, 'Para recuperar a sua conta, é necessário informar uma nova senha e a confirmação da mesma.', confirmacao);

        return;
    }

    if (!validarCriteriosSenha(novaSenha, div_feedbackCriterios)) {
        return;
    }

    if (!verificarMatchSenhas(novaSenha, confirmacao, div_feedback)) {
        return;
    }

    var idUsuario = document.getElementById("hddIdUsuario").value;
    var email = document.getElementById("hddEmail").value;
    var senha = document.getElementById("txtNovaSenha").value;
    var tokenNovaSenha = document.getElementById("hddTokenNovaSenha").value;

    var formData = new FormData();
    formData.append('IdUsuario', idUsuario);
    formData.append('Email', email);
    formData.append('Senha', senha);
    formData.append('TokenNovaSenha', tokenNovaSenha);

    if (hash_recuperacao) {
        formData.append('HashRecuperacao', hash_recuperacao);
    }

    $.ajax({
        url: "/Autenticacao/NovaSenha",
        data: formData,
        method: 'post',
        processData: false,
        contentType: false,
        success: function (retorno) {
            notificar(true, retorno.titulo, retorno.mensagem, "success", "/Autenticacao/Login");
        },
        error: function (retorno) {
            var obj = retorno.responseJSON;

            console.error(obj.erro);
            notificar(true, obj.titulo, obj.mensagem, 'error', '');
        }
    });
}

//* -------------------- Login -------------------- */
function logar(event, id_campo_credencial, id_campo_senha) {
    event.preventDefault();

    var credencial = document.getElementById(id_campo_credencial).value;
    var senha = document.getElementById(id_campo_senha).value;

    if (!credencial || !senha) {
        notificar(false, "Autenticação", "Por favor, preencha todos os campos.", "warning", "");
        return;
    } else {
        if (credencial.includes("@")) {
            if (!validarEmail(credencial)) {
                notificar(false, "E-mail inválido", "Por favor, informe um e-mail válido e tente novamente.", "error", "");
                return;
            }
        } else {
            if (!validarCPF(credencial)) {
                notificar(false, "CPF inválido", "Por favor, informe um CPF válido e tente novamente.", "error", "");
                return;
            }
        }
    }

    var _data = {
        USUARIO: credencial,
        SENHA: senha
    }

    $.ajax({
        method: "POST",
        url: "/Autenticacao/Login",
        data: _data,
        dataType: "json",
        contentType: "application/x-www-form-urlencoded; charset=utf-8"
    }).done(function (response) {
        if (response.req === true) {
            if (!response.cond) {
                window.location.href = "../Home/Dashboard";
            } else {
                window.location.href = "../Home/DashboardEspecialista";
            }
        } else {
            notificar(false, "Autenticação", response.msg, "error", "");
        }
    }).fail(function () {
        notificar(false, "Erro", "Não foi possível autenticar o seu usuário.", "error", "");
    })
}

function recuperarSenha(event) {
    event.preventDefault();

    var _email = document.getElementById("txtEmailRecuperacao").value;

    if (!_email) {
        notificar(false, "Autenticação", "Por favor, preencha todos os campos.", "warning", "");
        return;
    } else {
        if (!validarEmail(_email)) {
            notificar(false, "E-mail inválido", "Por favor, informe um e-mail válido e tente novamente.", "error", "");
            return;
        }
    }

    var _data = {
        ID_USUARIO: 0,
        EMAIL: _email,
        NM_USUARIO: null
    }

    $.ajax({
        method: "POST",
        url: "/Autenticacao/RecuperacaoSenha",
        data: _data,
        dataType: "json",
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        success: function (retorno) {
            notificar(true, retorno.titulo, retorno.mensagem, "success", "/Autenticacao/Login");
        },
        error: function (retorno) {
            var response = retorno.responseJSON;

            console.error(response.erro);
            notificar(true, response.titulo, response.mensagem, 'error', '');
        }
    });
}