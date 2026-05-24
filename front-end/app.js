// ==========================================================================
// CLIENTE JAVASCRIPT - ESTANTE INTELIGENTE SPA
// INTEGRAÇÃO ASSÍNCRONA COM A API FASTAPI
// ==========================================================================

const API_BASE_URL = "https://localhost:7280";

// Estado Global da Aplicação
const state = {
    autores: [],
    editoras: [],
    livros: [],
    leitores: [],
    emprestimos: [],
    estanteUsuarioAtivo: null // ID do usuário cuja estante está sendo visualizada
};

// Inicialização da Página
document.addEventListener("DOMContentLoaded", () => {
    inicializarAbas();
    inicializarIcones();
    inicializarEstanteSelectHeader();
    
    // Carga inicial de dados da primeira aba (Livros)
    carregarDadosAbaAtiva("livros-tab");
});

// Inicializa a biblioteca de ícones Lucide
function inicializarIcones() {
    if (window.lucide) {
        window.lucide.createIcons();
    }
}

// ==========================================================================
// CONTROLE DE ABAS (SPA NAVEGAÇÃO INSTANTÂNEA)
// ==========================================================================
function inicializarAbas() {
    const navButtons = document.querySelectorAll(".nav-btn");
    const sections = document.querySelectorAll(".tab-section");

    navButtons.forEach(btn => {
        btn.addEventListener("click", () => {
            const targetTab = btn.getAttribute("data-tab");

            // Alterna classe active nos botões
            navButtons.forEach(b => b.classList.remove("active"));
            btn.classList.add("active");

            // Alterna visibilidade das seções com fade
            sections.forEach(sec => sec.classList.remove("active"));
            document.getElementById(targetTab).classList.add("active");

            // Carrega dinamicamente os dados da aba que acabou de ser ativada
            carregarDadosAbaAtiva(targetTab);
        });
    });
}

// Carrega os dados sob demanda conforme o usuário navega
function carregarDadosAbaAtiva(tabId) {
    switch (tabId) {
        case "livros-tab":
            carregarLivros();
            break;
        case "leitores-tab":
            carregarLeitores();
            break;
        case "emprestimos-tab":
            carregarEmprestimos();
            break;
        case "estante-tab":
            atualizarVisualizacaoEstante();
            break;
    }
}

// Injeta um seletor amigável no topo da seção estante para alternar leitores
function inicializarEstanteSelectHeader() {
    const estanteHeader = document.querySelector("#estante-tab .section-header");
    
    // Cria uma barra de seleção e insere logo após o cabeçalho
    const filterDiv = document.createElement("div");
    filterDiv.className = "filter-bar";
    filterDiv.style.marginBottom = "2rem";
    filterDiv.style.width = "100%";
    filterDiv.style.display = "flex";
    filterDiv.style.alignItems = "center";
    filterDiv.style.gap = "1rem";
    filterDiv.style.backgroundColor = "rgba(14, 19, 34, 0.4)";
    filterDiv.style.padding = "1rem";
    filterDiv.style.borderRadius = "12px";
    filterDiv.style.border = "1px solid rgba(255, 255, 255, 0.06)";
    
    filterDiv.innerHTML = `
        <label for="estante-usuario-filtro" style="font-weight: 600; font-size: 0.95rem; color: #f8fafc;">
            Visualizar estante do leitor:
        </label>
        <select id="estante-usuario-filtro" onchange="selecionarUsuarioEstante(this.value)" style="max-width: 320px; background-color: #070a13; border: 1px solid rgba(255, 255, 255, 0.08); border-radius: 8px; color: white; padding: 0.5rem 1rem; font-family: Outfit, sans-serif; font-size: 0.9rem; outline: none;">
            <option value="">-- Escolha um Leitor para Ver a Estante --</option>
        </select>
    `;
    
    estanteHeader.after(filterDiv);
}

// ==========================================================================
// TRATAMENTO DE ERROS AMIGÁVEL EM PORTUGUÊS (VALIDAÇÕES PYDANTIC)
// ==========================================================================
async function exibirErroModal(response, errorBannerId) {
    const banner = document.getElementById(errorBannerId);
    banner.innerHTML = "";
    banner.classList.remove("hidden");

    try {
        const errData = await response.json();
        
        // Trata erro de validação de Schema do Pydantic (422 Unprocessable Entity)
        if (response.status === 422 && Array.isArray(errData.detail)) {
            let errorMsg = "<strong>Erro de digitação. Por favor, ajuste:</strong><br>";
            
            errData.detail.forEach(err => {
                const field = err.loc[err.loc.length - 1]; // Pega o nome do campo com erro
                const msgOriginal = err.msg;
                
                // Traduz e embeleza as validações core do Pydantic para português amigável
                if (field === "email") {
                    errorMsg += `• <strong>E-mail inválido</strong>: Use o formato correto (ex: nome@dominio.com).<br>`;
                } else if (field === "telefone") {
                    errorMsg += `• <strong>Telefone inválido</strong>: Insira DDD e o número formatado como (DD) 9XXXX-XXXX.<br>`;
                } else if (field === "nome" || field === "nome_livro") {
                    errorMsg += `• <strong>Nome inválido</strong>: Deve ter pelo menos 3 caracteres, começar com letra e não possuir números ou símbolos.<br>`;
                } else if (field === "endereco") {
                    errorMsg += `• <strong>Endereço inválido</strong>: Insira um endereço completo com pelo menos 5 caracteres.<br>`;
                } else if (field === "status") {
                    errorMsg += `• <strong>Status inválido</strong>: Selecione uma das opções de status permitidas.<br>`;
                } else {
                    errorMsg += `• Campo <strong>${field}</strong>: ${msgOriginal}<br>`;
                }
            });
            banner.innerHTML = errorMsg;
        } else if (errData.detail) {
            // Outro erro de negócio disparado pelo backend (ex: HTTPException 400 ou 404)
            banner.innerHTML = `<strong>Aviso do Sistema:</strong> ${errData.detail}`;
        } else {
            banner.innerHTML = "Ocorreu um erro inesperado ao salvar os dados. Verifique a conexão.";
        }
    } catch (e) {
        banner.innerHTML = "Erro ao processar a resposta do servidor. Certifique-se de que o backend está ativo.";
    }
}

function fecharErroBanner(bannerId) {
    const banner = document.getElementById(bannerId);
    banner.innerHTML = "";
    banner.classList.add("hidden");
}

// ==========================================================================
// OPERAÇÕES DO ACERVO DE LIVROS (TAB 1)
// ==========================================================================
async function carregarLivros() {
    const grid = document.getElementById("grid-livros");
    
    try {
        const res = await fetch(`${API_BASE_URL}/livros/`);
        if (!res.ok) throw new Error("Erro de rede");
        
        state.livros = await res.json();
        
        if (state.livros.length === 0) {
            grid.innerHTML = `
                <div class="empty-state">
                    <i data-lucide="book-x"></i>
                    <h3>Nenhum livro no acervo</h3>
                    <p>O acervo da biblioteca está vazio no momento. Clique no botão acima para adicionar a sua primeira obra!</p>
                </div>
            `;
            inicializarIcones();
            return;
        }

        grid.innerHTML = state.livros.map(livro => `
            <div class="card" id="livro-card-${livro.id_livro}">
                <div class="card-header">
                    <div class="card-icon">
                        <i data-lucide="book"></i>
                    </div>
                    <div class="card-info">
                        <h4 class="card-title">${livro.nome_livro}</h4>
                        <div class="card-subtitle">
                            <span>Autor: <strong>${livro.nome_autor || "Desconhecido"}</strong></span>
                            <span>Editora: <strong>${livro.nome_editora || "Desconhecida"}</strong></span>
                        </div>
                    </div>
                </div>
                <div class="card-actions">
                    <button class="btn btn-secondary btn-small" onclick="abrirModalLivro(${livro.id_livro})">
                        <i data-lucide="edit"></i> Editar
                    </button>
                    <button class="btn btn-danger btn-small" onclick="deletarLivro(${livro.id_livro})">
                        <i data-lucide="trash-2"></i> Excluir
                    </button>
                </div>
            </div>
        `).join("");
        
        inicializarIcones();
    } catch (err) {
        grid.innerHTML = `
            <div class="empty-state">
                <i data-lucide="wifi-off" style="color: var(--color-danger);"></i>
                <h3>Falha ao se conectar com a API</h3>
                <p>Não foi possível buscar os livros. Verifique se o servidor do backend está ligado executando <code>python -m uvicorn app.main:app --reload</code>.</p>
            </div>
        `;
        inicializarIcones();
    }
}

// Abre Modal de Livro (Cadastrar ou Editar)
async function abrirModalLivro(id = null) {
    const modal = document.getElementById("modal-livro");
    const form = document.getElementById("form-livro");
    form.reset();
    fecharErroBanner("modal-livro-error");
    
    // Popula as listas de Autores e Editoras nos dropdowns antes de exibir
    await carregarDropdownAutores();
    await carregarDropdownEditoras();

    if (id) {
        document.getElementById("modal-livro-titulo").innerText = "Editar Detalhes do Livro";
        document.getElementById("livro-id").value = id;
        
        try {
            const res = await fetch(`${API_BASE_URL}/livros/${id}`);
            const livro = await res.json();
            document.getElementById("livro-nome").value = livro.nome_livro;
            document.getElementById("livro-autor").value = livro.id_autor || "";
            document.getElementById("livro-editora").value = livro.id_editora || "";
        } catch (e) {
            console.error("Erro ao carregar livro para edição", e);
        }
    } else {
        document.getElementById("modal-livro-titulo").innerText = "Cadastrar Novo Livro";
        document.getElementById("livro-id").value = "";
    }

    modal.classList.add("active");
    inicializarIcones();
}

function fecharModalLivro() {
    document.getElementById("modal-livro").classList.remove("active");
}

// Salva Livro (Novo ou Edição)
async function salvarLivro(event) {
    event.preventDefault();
    fecharErroBanner("modal-livro-error");
    
    const id = document.getElementById("livro-id").value;
    const nome = document.getElementById("livro-nome").value;
    const id_autor = parseInt(document.getElementById("livro-autor").value) || null;
    const id_editora = parseInt(document.getElementById("livro-editora").value) || null;

    // Validação local condizente com o back-end
    const erroNome = validarNomeLivroJS(nome);
    if (erroNome) {
        exibirErroLocalModal(erroNome, "modal-livro-error");
        return;
    }

    const payload = {
        nome_livro: nome,
        id_editora: id_editora,
        id_autor: id_autor
    };

    let res;
    try {
        if (id) {
            // Edição
            res = await fetch(`${API_BASE_URL}/livros/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });
        } else {
            // Cadastro
            res = await fetch(`${API_BASE_URL}/livros/`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });
        }

        if (res.ok) {
            fecharModalLivro();
            carregarLivros();
        } else {
            exibirErroModal(res, "modal-livro-error");
        }
    } catch (err) {
        document.getElementById("modal-livro-error").innerHTML = "Erro ao enviar a requisição ao servidor backend.";
        document.getElementById("modal-livro-error").classList.remove("hidden");
    }
}

// Deleta Livro com confirmação explícita no navegador
async function deletarLivro(id) {
    if (confirm("Tem certeza absoluta que deseja excluir este livro do acervo?")) {
        try {
            const res = await fetch(`${API_BASE_URL}/livros/${id}`, { method: "DELETE" });
            if (res.ok) {
                carregarLivros();
            } else {
                alert("Não foi possível excluir o livro. Ele pode estar emprestado ou associado a um leitor.");
            }
        } catch (e) {
            alert("Erro ao tentar excluir.");
        }
    }
}

// ==========================================================================
// OPERAÇÕES DE LEITORES / USUÁRIOS (TAB 2)
// ==========================================================================
async function carregarLeitores() {
    const grid = document.getElementById("grid-leitores");
    
    try {
        const res = await fetch(`${API_BASE_URL}/usuarios/`);
        if (!res.ok) throw new Error();
        
        state.leitores = await res.json();
        
        if (state.leitores.length === 0) {
            grid.innerHTML = `
                <div class="empty-state">
                    <i data-lucide="user-x"></i>
                    <h3>Nenhum leitor cadastrado</h3>
                    <p>Não há leitores cadastrados no sistema. Adicione pessoas para que elas possam pegar livros emprestados!</p>
                </div>
            `;
            inicializarIcones();
            return;
        }

        grid.innerHTML = state.leitores.map(leitor => `
            <div class="card" id="leitor-card-${leitor.id_usuario}">
                <div class="card-header">
                    <div class="card-icon" style="background-color: rgba(16, 185, 129, 0.08); color: var(--color-success);">
                        <i data-lucide="user"></i>
                    </div>
                    <div class="card-info">
                        <h4 class="card-title">${leitor.nome}</h4>
                        <div class="card-subtitle">
                            <span>📧 E-mail: <strong>${leitor.email}</strong></span>
                            <span>📞 Tel: <strong>${leitor.telefone}</strong></span>
                            <span>📍 End: <strong>${leitor.endereco}</strong></span>
                        </div>
                    </div>
                </div>
                <div class="card-actions">
                    <button class="btn btn-secondary btn-small" onclick="abrirModalLeitor(${leitor.id_usuario})">
                        <i data-lucide="edit"></i> Editar
                    </button>
                    <button class="btn btn-danger btn-small" onclick="deletarLeitor(${leitor.id_usuario})">
                        <i data-lucide="trash-2"></i> Excluir
                    </button>
                </div>
            </div>
        `).join("");
        
        inicializarIcones();
        atualizarFiltroLeitoresEstanteDropdown();
    } catch (err) {
        grid.innerHTML = `
            <div class="empty-state">
                <i data-lucide="wifi-off" style="color: var(--color-danger);"></i>
                <h3>Falha ao carregar leitores</h3>
                <p>Verifique se o backend está de fato rodando.</p>
            </div>
        `;
        inicializarIcones();
    }
}

function abrirModalLeitor(id = null) {
    const modal = document.getElementById("modal-leitor");
    const form = document.getElementById("form-leitor");
    form.reset();
    fecharErroBanner("modal-leitor-error");

    if (id) {
        document.getElementById("modal-leitor-titulo").innerText = "Editar Detalhes do Leitor";
        document.getElementById("leitor-id").value = id;
        
        // Puxa os dados para preencher
        const leitor = state.leitores.find(u => u.id_usuario === id);
        if (leitor) {
            document.getElementById("leitor-nome").value = leitor.nome;
            document.getElementById("leitor-email").value = leitor.email;
            document.getElementById("leitor-telefone").value = leitor.telefone;
            document.getElementById("leitor-endereco").value = leitor.endereco;
        }
    } else {
        document.getElementById("modal-leitor-titulo").innerText = "Cadastrar Novo Leitor";
        document.getElementById("leitor-id").value = "";
    }

    modal.classList.add("active");
    inicializarIcones();
}

function fecharModalLeitor() {
    document.getElementById("modal-leitor").classList.remove("active");
}

async function salvarLeitor(event) {
    event.preventDefault();
    fecharErroBanner("modal-leitor-error");

    const id = document.getElementById("leitor-id").value;
    const nome = document.getElementById("leitor-nome").value;
    const email = document.getElementById("leitor-email").value;
    const telefone = document.getElementById("leitor-telefone").value;
    const endereco = document.getElementById("leitor-endereco").value;

    // Validações locais condizentes com o back-end
    const erroNome = validarNomeJS(nome);
    if (erroNome) {
        exibirErroLocalModal(erroNome, "modal-leitor-error");
        return;
    }

    const erroEmail = validarEmailJS(email);
    if (erroEmail) {
        exibirErroLocalModal(erroEmail, "modal-leitor-error");
        return;
    }

    const erroTelefone = validarTelefoneJS(telefone);
    if (erroTelefone) {
        exibirErroLocalModal(erroTelefone, "modal-leitor-error");
        return;
    }

    const erroEndereco = validarEnderecoJS(endereco);
    if (erroEndereco) {
        exibirErroLocalModal(erroEndereco, "modal-leitor-error");
        return;
    }

    const payload = { nome, email, telefone, endereco };

    let res;
    try {
        if (id) {
            // Atualização parcial de campos com o método PATCH que criamos no backend!
            res = await fetch(`${API_BASE_URL}/usuarios/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });
        } else {
            // Criação
            res = await fetch(`${API_BASE_URL}/usuarios/`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });
        }

        if (res.ok) {
            fecharModalLeitor();
            carregarLeitores();
        } else {
            exibirErroModal(res, "modal-leitor-error");
        }
    } catch (e) {
        document.getElementById("modal-leitor-error").innerText = "Erro na requisição.";
        document.getElementById("modal-leitor-error").classList.remove("hidden");
    }
}

async function deletarLeitor(id) {
    if (confirm("Deseja realmente remover este leitor? Todos os empréstimos ativos dele serão apagados!")) {
        try {
            const res = await fetch(`${API_BASE_URL}/usuarios/${id}`, { method: "DELETE" });
            if (res.ok) {
                carregarLeitores();
            } else {
                alert("Falha ao excluir.");
            }
        } catch (e) {
            alert("Erro.");
        }
    }
}

// ==========================================================================
// GESTÃO DE EMPRÉSTIMOS DE LIVROS (TAB 3)
// ==========================================================================
let filtroEmprestimoAtivo = "todos"; // 'todos' ou 'ativos'

function filtrarEmprestimos(tipo) {
    filtroEmprestimoAtivo = tipo;
    
    // Atualiza estado visual dos botões de filtro
    document.querySelectorAll(".filter-btn").forEach(btn => btn.classList.remove("active"));
    if (tipo === "todos") {
        document.getElementById("filter-emp-todos").classList.add("active");
    } else {
        document.getElementById("filter-emp-ativos").classList.add("active");
    }

    carregarEmprestimos();
}

async function carregarEmprestimos() {
    const grid = document.getElementById("grid-emprestimos");
    const url = filtroEmprestimoAtivo === "ativos" 
        ? `${API_BASE_URL}/emprestimos/ativos` 
        : `${API_BASE_URL}/emprestimos/`;

    try {
        const res = await fetch(url);
        if (!res.ok) throw new Error();
        
        state.emprestimos = await res.json();
        
        if (state.emprestimos.length === 0) {
            grid.innerHTML = `
                <div class="empty-state">
                    <i data-lucide="calendar-x"></i>
                    <h3>Nenhum empréstimo registrado</h3>
                    <p>Não há registros de empréstimos correspondentes a este filtro.</p>
                </div>
            `;
            inicializarIcones();
            return;
        }

        grid.innerHTML = state.emprestimos.map(emp => {
            const dataEmp = emp.data_emprestimo ? new Date(emp.data_emprestimo).toLocaleDateString("pt-BR") : "--";
            const dataPrazo = emp.data_prazo ? new Date(emp.data_prazo).toLocaleDateString("pt-BR") : "--";
            const dataDev = emp.data_devolucao ? new Date(emp.data_devolucao).toLocaleDateString("pt-BR") : null;
            
            // Badge colorido de status
            const statusClass = `status-${emp.status}`;
            
            // Exibe botões apenas para empréstimos não devolvidos
            const showActionButtons = emp.status !== "devolvido";

            return `
                <div class="card" id="emprestimo-card-${emp.id_emprestimo}">
                    <span class="badge-status ${statusClass}">${emp.status}</span>
                    <div class="card-header">
                        <div class="card-icon" style="background-color: rgba(245, 158, 11, 0.08); color: var(--color-warning);">
                            <i data-lucide="arrow-left-right"></i>
                        </div>
                        <div class="card-info">
                            <h4 class="card-title">${emp.nome_livro || "Livro Excluído"}</h4>
                            <div class="card-subtitle">
                                <span>👤 Pegou com: <strong>${emp.nome_usuario || "Leitor Excluído"}</strong></span>
                                <span>📅 Data Empréstimo: <strong>${dataEmp}</strong></span>
                                <span>⚠️ Prazo Limite: <strong>${dataPrazo}</strong></span>
                                ${dataDev ? `<span>✅ Devolvido em: <strong>${dataDev}</strong></span>` : ""}
                            </div>
                        </div>
                    </div>
                    ${showActionButtons ? `
                        <div class="card-actions">
                            <button class="btn btn-success btn-small" onclick="registrarDevolucao(${emp.id_emprestimo})" title="Registrar que o livro foi devolvido hoje">
                                <i data-lucide="check"></i> Devolvido
                            </button>
                            ${emp.status === "emprestado" ? `
                                <button class="btn btn-secondary btn-small" onclick="marcarComoAtrasado(${emp.id_emprestimo})" style="color: var(--color-warning); border-color: rgba(245, 158, 11, 0.2);">
                                    <i data-lucide="clock"></i> Atrasado
                                </button>
                            ` : ""}
                        </div>
                    ` : `
                        <div class="card-actions">
                            <button class="btn btn-danger btn-small" onclick="deletarEmprestimo(${emp.id_emprestimo})">
                                <i data-lucide="trash-2"></i> Apagar Histórico
                            </button>
                        </div>
                    `}
                </div>
            `;
        }).join("");
        
        inicializarIcones();
    } catch (e) {
        grid.innerHTML = `<div class="empty-state"><h3>Falha ao carregar empréstimos</h3></div>`;
    }
}

async function abrirModalEmprestimo() {
    const modal = document.getElementById("modal-emprestimo");
    const form = document.getElementById("form-emprestimo");
    form.reset();
    fecharErroBanner("modal-emprestimo-error");

    // Popula seletores
    const selectLeitor = document.getElementById("emprestimo-leitor");
    const selectLivro = document.getElementById("emprestimo-livro");

    // Popula Leitores
    await carregarLeitoresLocais();
    selectLeitor.innerHTML = '<option value="">-- Escolha o Leitor --</option>' + 
        state.leitores.map(u => `<option value="${u.id_usuario}">${u.nome}</option>`).join("");

    // Popula Livros
    await carregarLivrosLocais();
    selectLivro.innerHTML = '<option value="">-- Escolha o Livro --</option>' + 
        state.livros.map(l => `<option value="${l.id_livro}">${l.nome_livro}</option>`).join("");

    modal.classList.add("active");
    inicializarIcones();
}

function fecharModalEmprestimo() {
    document.getElementById("modal-emprestimo").classList.remove("active");
}

async function salvarEmprestimo(event) {
    event.preventDefault();
    fecharErroBanner("modal-emprestimo-error");

    const id_usuario = parseInt(document.getElementById("emprestimo-leitor").value);
    const id_livro = parseInt(document.getElementById("emprestimo-livro").value);

    try {
        const res = await fetch(`${API_BASE_URL}/emprestimos/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id_usuario, id_livro })
        });

        if (res.ok) {
            fecharModalEmprestimo();
            carregarEmprestimos();
        } else {
            exibirErroModal(res, "modal-emprestimo-error");
        }
    } catch (e) {
        document.getElementById("modal-emprestimo-error").innerText = "Erro na requisição.";
    }
}

async function registrarDevolucao(id) {
    try {
        const res = await fetch(`${API_BASE_URL}/emprestimos/${id}/devolver`, { method: "PUT" });
        if (res.ok) {
            carregarEmprestimos();
        } else {
            alert("Erro ao registrar devolução.");
        }
    } catch (e) {
        alert("Erro.");
    }
}

async function marcarComoAtrasado(id) {
    try {
        const res = await fetch(`${API_BASE_URL}/emprestimos/${id}/atrasado`, { method: "PUT" });
        if (res.ok) {
            carregarEmprestimos();
        } else {
            alert("Erro ao marcar como atrasado.");
        }
    } catch (e) {
        alert("Erro.");
    }
}

async function deletarEmprestimo(id) {
    if (confirm("Excluir este empréstimo do histórico permanente da biblioteca?")) {
        try {
            const res = await fetch(`${API_BASE_URL}/emprestimos/${id}`, { method: "DELETE" });
            if (res.ok) {
                carregarEmprestimos();
            }
        } catch (e) {}
    }
}

// ==========================================================================
// MINHA ESTANTE VIRTUAL PESSOAL (TAB 4)
// ==========================================================================

// Chamado quando o usuário escolhe um leitor no seletor do topo da estante
function selecionarUsuarioEstante(idUsuario) {
    state.estanteUsuarioAtivo = idUsuario ? parseInt(idUsuario) : null;
    atualizarVisualizacaoEstante();
}

// Popula o seletor da estante
function atualizarFiltroLeitoresEstanteDropdown() {
    const filtro = document.getElementById("estante-usuario-filtro");
    const modalSelect = document.getElementById("estante-leitor");
    
    const optionsHtml = '<option value="">-- Escolha um Leitor para Ver a Estante --</option>' + 
        state.leitores.map(u => `<option value="${u.id_usuario}">${u.nome}</option>`).join("");
        
    const modalOptionsHtml = '<option value="">-- Selecione o Usuário --</option>' + 
        state.leitores.map(u => `<option value="${u.id_usuario}">${u.nome}</option>`).join("");
        
    if (filtro) {
        // Guarda seleção
        const oldVal = filtro.value;
        filtro.innerHTML = optionsHtml;
        filtro.value = oldVal;
    }
    
    if (modalSelect) {
        modalSelect.innerHTML = modalOptionsHtml;
    }
}

// Renderiza as 3 colunas da estante virtual com base no leitor ativo
async function atualizarVisualizacaoEstante() {
    const listLendo = document.getElementById("estante-lendo-list");
    const listQuero = document.getElementById("estante-quero-list");
    const listLido = document.getElementById("estante-lido-list");
    
    const badgeLendo = document.getElementById("badge-lendo");
    const badgeQuero = document.getElementById("badge-quero");
    const badgeLido = document.getElementById("badge-lido");

    // Limpa visualização
    listLendo.innerHTML = "";
    listQuero.innerHTML = "";
    listLido.innerHTML = "";
    badgeLendo.innerText = "0";
    badgeQuero.innerText = "0";
    badgeLido.innerText = "0";

    if (!state.estanteUsuarioAtivo) {
        const msgVazia = `
            <div class="empty-state" style="padding: 2.5rem 1rem;">
                <i data-lucide="hand-metal"></i>
                <p>Selecione um leitor no seletor acima para ver a sua estante de livros pessoal!</p>
            </div>
        `;
        listLendo.innerHTML = msgVazia;
        listQuero.innerHTML = msgVazia;
        listLido.innerHTML = msgVazia;
        inicializarIcones();
        return;
    }

    try {
        const res = await fetch(`${API_BASE_URL}/estante/usuario/${state.estanteUsuarioAtivo}`);
        if (!res.ok) throw new Error();
        
        const estanteItens = await res.json();
        
        let countLendo = 0;
        let countQuero = 0;
        let countLido = 0;

        estanteItens.forEach(item => {
            const dataConclusao = item.data_atualizacao ? new Date(item.data_atualizacao).toLocaleDateString("pt-BR") : null;
            
            const cardHtml = `
                <div class="estante-card" id="estante-card-${item.id_estante}">
                    <h5 class="estante-card-title">${item.nome_livro || "Livro"}</h5>
                    <div class="estante-card-user">
                        <i data-lucide="user"></i>
                        <span>Leitor: ${item.nome_usuario || "Usuário"}</span>
                    </div>
                    ${item.status === 'lido' && dataConclusao ? `
                        <div class="estante-card-date" style="display: flex; align-items: center; gap: 0.35rem; font-size: 0.75rem; color: var(--text-secondary); margin-top: -0.25rem; margin-bottom: 0.25rem;">
                            <i data-lucide="calendar" style="width: 12px; height: 12px; color: var(--color-success);"></i>
                            <span>Lido em: <strong>${dataConclusao}</strong></span>
                        </div>
                    ` : ""}
                    <div class="estante-card-actions">
                        <!-- Botão de mover status rápido -->
                        ${item.status === 'quero ler' ? `
                            <button class="quick-move-btn" onclick="moverItemEstante(${item.id_livro}, 'lendo')" style="color: var(--color-primary);">
                                <i data-lucide="book-open"></i> Lendo agora
                            </button>
                        ` : ""}
                        ${item.status === 'lendo' ? `
                            <button class="quick-move-btn" onclick="moverItemEstante(${item.id_livro}, 'lido')" style="color: var(--color-success);">
                                <i data-lucide="check-circle2"></i> Marcar como Lido
                            </button>
                        ` : ""}
                        
                        <!-- Botão de remover da estante -->
                        <button class="quick-move-btn" onclick="removerDaEstante(${item.id_livro})" style="color: var(--color-danger); margin-left: auto;">
                            <i data-lucide="trash-2"></i> Tirar
                        </button>
                    </div>
                </div>
            `;

            if (item.status === "lendo") {
                listLendo.innerHTML += cardHtml;
                countLendo++;
            } else if (item.status === "quero ler") {
                listQuero.innerHTML += cardHtml;
                countQuero++;
            } else if (item.status === "lido") {
                listLido.innerHTML += cardHtml;
                countLido++;
            }
        });

        // Atualiza contadores
        badgeLendo.innerText = countLendo;
        badgeQuero.innerText = countQuero;
        badgeLido.innerText = countLido;

        // Banners vazios nas colunas vazias
        const colVaziaHtml = `<div class="empty-state" style="padding: 2.5rem 1rem;"><p>Nenhum livro neste status.</p></div>`;
        if (countLendo === 0) listLendo.innerHTML = colVaziaHtml;
        if (countQuero === 0) listQuero.innerHTML = colVaziaHtml;
        if (countLido === 0) listLido.innerHTML = colVaziaHtml;

        inicializarIcones();
    } catch (e) {
        console.error(e);
    }
}

// Move o status do livro na estante com um único clique
async function moverItemEstante(idLivro, novoStatus) {
    if (!state.estanteUsuarioAtivo) return;
    
    try {
        const res = await fetch(`${API_BASE_URL}/estante/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                id_usuario: state.estanteUsuarioAtivo,
                id_livro: idLivro,
                status: novoStatus
            })
        });

        if (res.ok) {
            atualizarVisualizacaoEstante();
        }
    } catch (e) {
        console.error(e);
    }
}

// Remove livro da estante do usuário ativo
async function removerDaEstante(idLivro) {
    if (!state.estanteUsuarioAtivo) return;
    
    if (confirm("Quer tirar este livro da sua estante virtual?")) {
        try {
            const res = await fetch(`${API_BASE_URL}/estante/usuario/${state.estanteUsuarioAtivo}/livro/${idLivro}`, {
                method: "DELETE"
            });
            if (res.ok) {
                atualizarVisualizacaoEstante();
            }
        } catch (e) {}
    }
}

// Abre Modal de adicionar item na estante
async function abrirModalEstante() {
    const modal = document.getElementById("modal-estante");
    const form = document.getElementById("form-estante");
    form.reset();
    fecharErroBanner("modal-estante-error");

    const selectLeitor = document.getElementById("estante-leitor");
    const selectLivro = document.getElementById("estante-livro");

    // Popula dropdowns
    await carregarLeitoresLocais();
    selectLeitor.innerHTML = '<option value="">-- Selecione o Usuário --</option>' + 
        state.leitores.map(u => `<option value="${u.id_usuario}">${u.nome}</option>`).join("");
        
    await carregarLivrosLocais();
    selectLivro.innerHTML = '<option value="">-- Selecione o Livro --</option>' + 
        state.livros.map(l => `<option value="${l.id_livro}">${l.nome_livro}</option>`).join("");

    // Se houver leitor selecionado no filtro de estante, já deixa selecionado no modal!
    if (state.estanteUsuarioAtivo) {
        selectLeitor.value = state.estanteUsuarioAtivo;
    }

    modal.classList.add("active");
    inicializarIcones();
}

function fecharModalEstante() {
    document.getElementById("modal-estante").classList.remove("active");
}

async function salvarEstante(event) {
    event.preventDefault();
    fecharErroBanner("modal-estante-error");

    const id_usuario = parseInt(document.getElementById("estante-leitor").value);
    const id_livro = parseInt(document.getElementById("estante-livro").value);
    const status = document.getElementById("estante-status").value;

    try {
        const res = await fetch(`${API_BASE_URL}/estante/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id_usuario, id_livro, status })
        });

        if (res.ok) {
            fecharModalEstante();
            // Se salvamos a estante do usuário que está selecionado no topo, recarrega a visualização
            if (state.estanteUsuarioAtivo === id_usuario) {
                atualizarVisualizacaoEstante();
            } else {
                // Senão, seleciona ele automaticamente no topo para exibir o novo livro!
                const filter = document.getElementById("estante-usuario-filtro");
                filter.value = id_usuario;
                selecionarUsuarioEstante(id_usuario);
            }
        } else {
            exibirErroModal(res, "modal-estante-error");
        }
    } catch (e) {
        document.getElementById("modal-estante-error").innerText = "Erro na requisição.";
    }
}

// ==========================================================================
// FUNÇÕES AUXILIARES / POPULAÇÃO DE DROPDOWNS E CACHE LOCAL
// ==========================================================================
async function carregarLeitoresLocais() {
    try {
        const res = await fetch(`${API_BASE_URL}/usuarios/`);
        state.leitores = await res.json();
    } catch (e) {}
}

async function carregarLivrosLocais() {
    try {
        const res = await fetch(`${API_BASE_URL}/livros/`);
        state.livros = await res.json();
    } catch (e) {}
}

async function carregarDropdownAutores() {
    const select = document.getElementById("livro-autor");
    try {
        const res = await fetch(`${API_BASE_URL}/autores/`);
        state.autores = await res.json();
        
        select.innerHTML = '<option value="">-- Selecione o Autor --</option>' +
            state.autores.map(a => `<option value="${a.id_autor}">${a.nome}</option>`).join("");
    } catch (e) {
        select.innerHTML = '<option value="">Erro ao carregar autores</option>';
    }
}

async function carregarDropdownEditoras() {
    const select = document.getElementById("livro-editora");
    try {
        const res = await fetch(`${API_BASE_URL}/editoras/`);
        state.editoras = await res.json();
        
        select.innerHTML = '<option value="">-- Selecione a Editora --</option>' +
            state.editoras.map(e => `<option value="${e.id_editora}">${e.nome}</option>`).join("");
    } catch (e) {
        select.innerHTML = '<option value="">Erro ao carregar editoras</option>';
    }
}

// ==========================================================================
// SUB-MODAIS DE ATALHO RÁPIDO (CRIAR AUTOR E EDITORA DENTRO DO LIVRO)
// ==========================================================================
function abrirSubModalAutor() {
    document.getElementById("modal-sub-autor").classList.add("active");
    document.getElementById("form-sub-autor").reset();
    fecharErroBanner("modal-sub-autor-error");
}

function fecharSubModalAutor() {
    document.getElementById("modal-sub-autor").classList.remove("active");
}

async function salvarSubAutor(event) {
    event.preventDefault();
    fecharErroBanner("modal-sub-autor-error");
    
    const nome = document.getElementById("sub-autor-nome").value;

    // Validação local condizente com o back-end
    const erroNome = validarNomeJS(nome);
    if (erroNome) {
        exibirErroLocalModal(erroNome, "modal-sub-autor-error");
        return;
    }
    
    try {
        const res = await fetch(`${API_BASE_URL}/autores/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ nome })
        });
        
        if (res.ok) {
            const novoAutor = await res.json();
            fecharSubModalAutor();
            
            // Recarrega o dropdown de autores do livro e já seleciona o recém-criado!
            await carregarDropdownAutores();
            document.getElementById("livro-autor").value = novoAutor.id_autor;
        } else {
            exibirErroModal(res, "modal-sub-autor-error");
        }
    } catch (e) {
        document.getElementById("modal-sub-autor-error").innerText = "Erro de conexão.";
    }
}

function abrirSubModalEditora() {
    document.getElementById("modal-sub-editora").classList.add("active");
    document.getElementById("form-sub-editora").reset();
    fecharErroBanner("modal-sub-editora-error");
}

function fecharSubModalEditora() {
    document.getElementById("modal-sub-editora").classList.remove("active");
}

async function salvarSubEditora(event) {
    event.preventDefault();
    fecharErroBanner("modal-sub-editora-error");
    
    const nome = document.getElementById("sub-editora-nome").value;

    // Validação local condizente com o back-end
    const erroNome = validarNomeJS(nome);
    if (erroNome) {
        exibirErroLocalModal(erroNome, "modal-sub-editora-error");
        return;
    }
    
    try {
        const res = await fetch(`${API_BASE_URL}/editoras/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ nome })
        });
        
        if (res.ok) {
            const novaEditora = await res.json();
            fecharSubModalEditora();
            
            // Recarrega o dropdown de editoras do livro e seleciona o recém-criado!
            await carregarDropdownEditoras();
            document.getElementById("livro-editora").value = novaEditora.id_editora;
        } else {
            exibirErroModal(res, "modal-sub-editora-error");
        }
    } catch (e) {
        document.getElementById("modal-sub-editora-error").innerText = "Erro de conexão.";
    }
}

// ==========================================================================
// FUNÇÕES DE VALIDAÇÃO CONDIZENTES COM O BACK-END (PYTHON)
// ==========================================================================

function exibirErroLocalModal(mensagem, errorBannerId) {
    const banner = document.getElementById(errorBannerId);
    banner.innerHTML = `<strong>Ajuste o seguinte campo:</strong><br>• ${mensagem}`;
    banner.classList.remove("hidden");
}

function validarNomeJS(nome) {
    nome = nome.trim();
    if (!nome) return "O nome não pode ser vazio";
    if (nome.length < 3) return "O nome deve ter pelo menos 3 caracteres";
    if (/\d/.test(nome)) return "O nome não pode conter números";
    
    // Começa e termina com letra (incluindo acentuadas brasileiras)
    const regexLetraInicioFim = /^[a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ].*[a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]$/;
    if (!regexLetraInicioFim.test(nome)) {
        return "O nome não pode ter caracteres especiais no início nem no final";
    }
    return null;
}

function validarNomeLivroJS(nome) {
    nome = nome.trim();
    if (!nome) return "O nome do livro não pode ser vazio";
    if (nome.length < 2) return "O nome do livro deve ter pelo menos 2 caracteres";
    if (nome.length > 255) return "O nome do livro deve ter no máximo 255 caracteres";
    if (nome.includes("  ")) return "O nome do livro não pode conter espaços duplos";
    
    const regexAlfanumericoInicioFim = /^[a-zA-Z0-9áàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ].*[a-zA-Z0-9áàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]$/;
    if (!regexAlfanumericoInicioFim.test(nome)) {
        return "O nome do livro deve começar e terminar com letra ou número";
    }
    return null;
}

function validarEmailJS(email) {
    email = email.trim();
    if (!email) return "O e-mail não pode ser vazio";
    if (/\s/.test(email)) return "O e-mail não pode conter espaços em branco";
    if ((email.match(/@/g) || []).length !== 1) return "O e-mail deve conter exatamente um '@'";
    
    const partes = email.split("@");
    const local = partes[0];
    const dominio = partes[1];
    
    if (!local || !dominio) return "O e-mail não pode ter a parte antes ou depois do '@' vazia";
    if (email.includes("..")) return "O e-mail não pode conter pontos consecutivos (..)";
    if (local.startsWith(".") || local.endsWith(".")) return "A parte antes do '@' não pode começar ou terminar com ponto";
    
    const regexAlfanumericoInicioFim = /^[a-zA-Z0-9].*[a-zA-Z0-9]$/;
    if (!regexAlfanumericoInicioFim.test(local)) return "A parte antes do '@' deve começar e terminar com letra ou número";
    
    const regexLocal = /^[a-zA-Z0-9._-]+$/;
    if (!regexLocal.test(local)) return "Antes do '@', use apenas letras, números, ponto, hífen e sublinhado";
    
    const regexDominio = /^[a-zA-Z0-9.-]+$/;
    if (!regexDominio.test(dominio)) return "No domínio do e-mail, use apenas letras, números, ponto e hífen";
    
    if (!dominio.endsWith(".com")) return "O e-mail deve terminar com '.com'";
    if (email.length < 11) return "O e-mail deve ter pelo menos 11 caracteres";
    
    return null;
}

function validarEnderecoJS(endereco) {
    endereco = endereco.trim();
    if (!endereco) return "O endereço não pode ser vazio";
    if (endereco.length < 10) return "O endereço deve ter pelo menos 10 caracteres";
    if (endereco.length > 150) return "O endereço deve ter no máximo 150 caracteres";
    if (/[\r\n\t]/.test(endereco)) return "O endereço não pode conter tabulação nem quebra de linha";
    
    const regexAlfanumericoInicioFim = /^[a-zA-Z0-9áàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ].*[a-zA-Z0-9áàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]$/;
    if (!regexAlfanumericoInicioFim.test(endereco)) return "O endereço deve começar e terminar com letra ou número";
    if (!/[a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]/.test(endereco)) return "O endereço deve conter pelo menos uma letra";
    if (endereco.includes("  ")) return "O endereço não pode conter dois espaços seguidos";
    
    return null;
}

function validarTelefoneJS(telefone) {
    telefone = telefone.trim();
    if (!telefone) return "O telefone não pode ser vazio";
    
    let apenasDigitos = telefone.replace(/\D/g, "");
    if (apenasDigitos.startsWith("55")) {
        apenasDigitos = apenasDigitos.substring(2);
    }
    
    if (apenasDigitos.length !== 10 && apenasDigitos.length !== 11) {
        return "Use DDD + número (fixo com 8 dígitos ou celular com 9)";
    }
    
    const ddd = apenasDigitos.substring(0, 2);
    const numero = apenasDigitos.substring(2);
    
    if (ddd[0] === "0" || ddd === "00") {
        return "O DDD do telefone é inválido";
    }
    
    if (numero.length === 9) {
        if (numero[0] !== "9") {
            return "Celular deve começar com 9 após o DDD";
        }
    }
    return null;
}

