//* -------------------- Validações (podem ser usadas em outras telas, como as de cadastro, por exemplo) -------------------- */
function validarCPF(cpf) {
    cpf = cpf.replace(/[.-]/g, ''); // Remove os pontos (.) e o hífen (-), mantendo apenas números para realizar as operações de validação.

    if (cpf == "00000000000") {
        return false;
    }

    if (cpf.length !== 11 || /^(.)\1+$/.test(cpf)) { // Verifica se o CPF tem 11 dígitos e se não é uma sequência repetida qualquer.
        return false;
    }

    var soma;
    var resto;
    soma = 0;

    for (i = 1; i <= 9; i++) {
        soma = soma + parseInt(cpf.substring(i - 1, i)) * (11 - i);
    }

    resto = (soma * 10) % 11;
    if ((resto == 10) || (resto == 11)) {
        resto = 0;
    }
    if (resto != parseInt(cpf.substring(9, 10))) {
        return false;
    }

    soma = 0;
    for (i = 1; i <= 10; i++) {
        soma = soma + parseInt(cpf.substring(i - 1, i)) * (12 - i);
    }

    resto = (soma * 10) % 11;
    if ((resto == 10) || (resto == 11)) {
        resto = 0;
    }
    if (resto != parseInt(cpf.substring(10, 11))) {
        return false;
    }

    return true;
}

function validarEmail(email) {
    return /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$/.test(email);
}

function validarData(data) {
    const regex = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;
    if (!regex.test(data)) {
        return false;
    }

    const partes = data.split('/');

    const dia = parseInt(partes[0], 10);
    const mes = parseInt(partes[1], 10);
    const ano = parseInt(partes[2], 10);

    const dataObj = new Date(ano, mes - 1, dia);

    return dataObj.getFullYear() === ano && dataObj.getMonth() === mes - 1 && dataObj.getDate() === dia;
}

function validarSelect(selector) {
    var select = $(selector);
    var selectedValue = select.val();

    return selectedValue !== 'Selecione' && selectedValue !== '';
}

function validarCheckboxes(id_container_checks) {
    var container_checks = document.getElementById(id_container_checks);
    var checkboxes = container_checks.querySelectorAll('input[type="checkbox"]');

    var algumMarcado = Array.from(checkboxes).some(cb => cb.checked);

    return algumMarcado;
}

function validarRG(rg) {
    // Remove pontos, hífens e outros caracteres especiais, mantendo apenas letras e números.
    rg = rg.replace(/[^a-zA-Z0-9]/g, '');

    // Verifica se o comprimento é válido (9 caracteres).
    if (rg.length !== 9) {
        return false;
    }

    // Verifica se contém apenas letras e números.
    if (!/^[a-zA-Z0-9]+$/.test(rg)) {
        return false;
    }

    // Verifica se todos os caracteres são iguais (ex: "AAAAAAAAA" ou "111111111").
    if (/^([a-zA-Z0-9])\1+$/.test(rg)) {
        return false;
    }

    return true;
}

function validarNumeroObrigatorio(input) {
    if (input.value === '' || isNaN(input.value)) {
        return false;
    }

    return true;
}

function validarFormulario(campos) {
    var _auxValido = true;

    campos.forEach((campo) => {
        var feedback = document.getElementById(`feedbackInvalido_${campo.id}`);
        var campoComponente = document.getElementById(campo.id);

        if (!campo.valido) {
            _auxValido = false;
            exibirFeedback(feedback, campo.msg, campoComponente);
        } else {
            limparFeedback(feedback, campoComponente);
        }
    });

    return _auxValido;
}

function validarLattes(id_campo, id_div_feedback) {
    var _auxValido = true;

    if (id_campo && id_div_feedback) {
        var feedback = document.getElementById(id_div_feedback);
        var campoComponente = document.getElementById(id_campo);

        $.ajax({
            url: '/Especialista/VerificarLattes',
            method: 'POST',
            data: { urlLattes: campoComponente.value }
        }).done(function (resultado) {
            if (!resultado.valido) {
                exibirFeedback(feedback, resultado.msg, campoComponente);
            } else {
                limparFeedback(feedback, campoComponente);
            }

            _auxValido = resultado.valido;
        });
    }

    return _auxValido;
}

// Mínimo de 8 dígitos, sendo uma letra maíuscula e uma minúscula, números e caracteres especiais.
function validarCriteriosSenha(senha, div_feedback) {
    if (senha.value) {
        const regex = /^(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8}$/;

        if (!regex.test(senha.value)) {
            exibirFeedback(div_feedback, 'A senha deve ser composta por 8 dígitos sendo ao menos 1 letra maiúscula, 1 número e 1 caractere especial.', senha);
            return false;
        }

        limparFeedback(div_feedback, senha);
        return true;
    }
}

function verificarMatchSenhas(campoSenha, campoSenhaConfirmacao, div_feedback) {
    if (campoSenhaConfirmacao.value) {
        var div_feedback = document.getElementById("feedbackInvalido");

        if (campoSenha.value !== campoSenhaConfirmacao.value) {
            exibirFeedback(div_feedback, 'As senhas informadas não são iguais.', campoSenhaConfirmacao);
            return false;
        } else {
            limparFeedback(div_feedback, campoSenha);
            limparFeedback(div_feedback, campoSenhaConfirmacao);
            return true;
        }
    }
}

function verificarMatchEmail(campoEmail, campoEmailConfirmacao) {
    if (campoEmailConfirmacao.value) {
        if (campoEmail.value !== campoEmailConfirmacao.value) {
            return false;
        } else {
            return true;
        }
    }
}

//* -------------------- Feedbacks -------------------- */
function exibirFeedback(div_feedback, mensagem, campo) {
    div_feedback.innerHTML = `<strong>${mensagem}</strong>`;
    div_feedback.style.display = 'block';

    setTimeout(() => {
        div_feedback.style.opacity = '1';
    }, 10);

    // Adiciona a classe de vibração ao campo.
    campo.classList.add('vibrate');
    campo.classList.add('campo-invalido');

    // Remove a classe de vibração após a animação para que possa ser reaplicada no futuro
    setTimeout(() => {
        campo.classList.remove('vibrate');
    }, 250);
}

function limparFeedback(div_feedback, campo) {
    if (div_feedback !== null && campo !== null) {
        div_feedback.style.opacity = '0';
        campo.classList.remove('campo-invalido');

        setTimeout(() => {
            div_feedback.innerHTML = '';
        }, 500);
    }
}