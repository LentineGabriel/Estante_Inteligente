# 🌟 Estante Inteligente - Biblioteca & Leitura Simplificada

> Um sistema de gestão de acervo e estante virtual pessoal projetado sob a ótica da **acessibilidade digital**, com interface clean e navegação direta, desenvolvido sob medida para pessoas que amam ler, mas possuem baixa afinidade com sistemas tecnológicos complexos.

Este projeto representa o trabalho acadêmico desenvolvido por alunos da **Universidade Nove de Julho (Uninove) — Campus Santo Amaro**.

---

## 👥 Integrantes e Desenvolvedores

| Nome Completo | Registro Acadêmico (RA) |
| :--- | :--- |
| **Gabriel Henrique Marinho Lentine** | `2225105706` |
| **Ana Clara Menezes Maciel** | `2225101484` |
| **Murillo do Nascimento Silva** | `2225102975` |
| **Bruno Pereira Silva** | `2225105721` |
| **Gustavo Oliveira Santos** | `2225105333` |
| **Rafael Azevedo Santos** | `2325103526` |

---

## 🎯 O Propósito Humano do Projeto

Muitos leitores ávidos — especialmente idosos ou pessoas que não cresceram imersas na era digital — sentem-se intimidados por sistemas de bibliotecas repletos de tabelas complexas, submenus confusos, campos de digitação minúsculos e jargões técnicos.

A **Estante Inteligente** resolve essa barreira com uma filosofia de design focada no acolhimento e no suporte ao usuário:
*   **Single Page Application (SPA)**: O usuário realiza todas as ações em uma única tela. Ele nunca se perde trocando de página ou carregando novos links no navegador.
*   **Design System Dark Mode Premium**: Cores escuras e relaxantes que eliminam a fadiga visual, acompanhadas da tipografia arredondada *Outfit* (Google Fonts), ideal para leitura em telas.
*   **Formulários "Sem Rodeios"**: Se o usuário estiver cadastrando um livro e perceber que o Autor ou Editora não existem na listagem, ele pode criá-los clicando em um pequeno botão `[ + ]` direto no formulário. O sistema cria o autor e o seleciona automaticamente, evitando fechamentos de janelas e perda de progresso.
*   **Ajuste de Layout para Livros Longos**: O design conta com proteção CSS contra quebra de caixas de texto. Títulos longos quebram a linha de forma elegante antes de atingir os badges de status de empréstimo absoluto, mantendo a visualização dos dados sempre visível e limpa.
*   **Tradutor de Erros para Linguagem Humana**: Em vez de códigos de erro misteriosos como `ValidationError: [422]`, o sistema traduz falhas de validação em alertas vermelhos didáticos e acolhedores (ex: *"O telefone precisa conter DDD e o número formatado como (DD) 9XXXX-XXXX"*).
*   **Validações Inteligentes Síncronas (Front + Back)**: O front-end atua de forma síncrona com o back-end, realizando validações idênticas locais imediatas ao tentar submeter cadastros. Isso previne o envio de requisições desnecessárias com dados incorretos e dá feedback instantâneo ao usuário.
*   **Favicon Personalizado Premium**: Ícone de aba customizado em formato de livro digital brilhante no estilo *glassmorphic*, conferindo uma identidade moderna e premium à aplicação.

---

## 🏗️ Arquitetura do Sistema

O projeto foi projetado utilizando as melhores práticas modernas de mercado, com separação de camadas do back-end, front-end baseado em tecnologias nativas e integração contínua (DevOps):

```text
Python_Biblioteca_Faculdade/
├── .github/                 # Automação de pipelines DevOps (GitHub Actions)
│   └── workflows/
│       └── ci.yml           # Pipeline de integração contínua (CI) de Linting e Builds
├── back-end/                # Camada de Backend (FastAPI, Python e PostgreSQL)
│   ├── app/
│   │   ├── api/             # Isolamento das rotas HTTP (APIRouter)
│   │   ├── core/            # Configurações, banco de dados e validações síncronas
│   │   ├── models/          # Entidades de negócio tipadas
│   │   ├── repositories/    # Persistência SQL bruta psycopg (sem ORMs pesados)
│   │   ├── schemas/         # Validação rígida de dados com Pydantic V2
│   │   └── main.py          # Inicializador da API FastAPI com Healthcheck e CORS
│   ├── Dockerfile           # Imagem otimizada (Multi-stage e Non-Root) da API
│   ├── init.sql             # Script automático de criação física de tabelas no Postgres
│   └── requirements.txt     # Dependências de pacotes
├── front-end/               # Camada de Apresentação (Acessível, Clean e Premium)
│   ├── index.html           # Esqueleto HTML5 da SPA estruturado com suporte a favicon
│   ├── style.css            # Folha de estilos Dark Mode, Glassmorphism e Transições
│   ├── app.js               # Validações locais inteligentes e consumo assíncrono (fetch)
│   └── favicon.png          # Ícone premium de livro digital brilhante na aba do navegador
└── docker-compose.yml       # Orquestrador de rede local do Banco + API com Healthchecks
```

---

## 🚀 Metodologia DevOps e Segurança no Back-end

Com o intuito de aplicar conceitos profissionais de mercado, o back-end da aplicação foi totalmente refatorado com foco nas diretrizes modernas de **DevOps**:

1.  **Docker Multi-stage Builds (Otimização):**
    A construção da imagem contêiner é realizada em duas etapas de compilação. O estágio `builder` instala compiladores e monta as dependências Python em um ambiente virtual isolado (`/opt/venv`). O estágio final `runner` copia apenas o virtual environment pronto e limpo, reduzindo drasticamente o tamanho final do contêiner e eliminando ferramentas de build extras que poderiam servir como vulnerabilidades.
2.  **Segurança por Design (Non-Root User):**
    Por padrão, contêineres Docker rodam como usuário administrador `root`. Para mitigar riscos severos de invasão e privilégios escalados, criamos um usuário restrito de sistema chamado `appuser` (com o grupo `appgroup`). Toda a aplicação FastAPI é executada estritamente sob este usuário seguro (`USER appuser`).
3.  **Observabilidade de Saúde (`/health` & Healthcheck Nativo):**
    Implementamos um endpoint de integridade estruturado `/health` na API FastAPI. Ele realiza testes de conectividade física com o banco de dados PostgreSQL (`SELECT 1`).
    *   **200 OK:** Se o banco responder, atesta status `"healthy"`.
    *   **503 Service Unavailable:** Se a rede ou o banco falhar, emite status `"unhealthy"`.
    Injetamos a instrução nativa `HEALTHCHECK` no próprio `Dockerfile` para testar localmente o endpoint a cada 30 segundos, permitindo controle absoluto ao orquestrador de contêineres.
4.  **Automação e Integração Contínua (GitHub Actions):**
    Criamos um arquivo de pipeline automatizado na pasta `.github/workflows/ci.yml`. Toda vez que alterações de código forem enviadas para o repositório principal no GitHub, os servidores rodarão de forma transparente:
    *   Validação de estilo e padrão PEP 8 do Python com `black` e `ruff`.
    *   Build teste completo do contêiner Docker para validar que o empacotamento continue livre de erros.

---

## 🚦 Como Executar a Aplicação

### Passo 1: Subir o Backend (Docker)
Com o Docker instalado na máquina, abra o terminal na raiz do projeto e execute:
```bash
docker compose up --build
```
Isso inicializará de forma totalmente resiliente o banco de dados PostgreSQL e a API FastAPI. A API estará ativa na porta `http://localhost:8000/`. Você pode testar e interagir diretamente com os endpoints na documentação autogerada em **`http://localhost:8000/docs`** ou checar a saúde ativa da API em **`http://localhost:8000/health`**.

### Passo 2: Acessar a Interface do Usuário (Front-end)
Para que recursos sensíveis (como o **Favicon** personalizado na aba do site) carreguem com suporte completo de segurança, **recomendamos abrir o projeto através de um servidor local dinâmico** em vez de dar apenas dois cliques no arquivo HTML (`file://`):

1.  Abra o terminal do seu computador na pasta do front-end:
    ```bash
    cd front-end
    ```
2.  Suba um servidor web local super leve em segundos usando o próprio Python:
    ```bash
    python -m http.server 8000
    ```
3.  Acesse o seguinte endereço no seu navegador de internet:
    **[http://localhost:8000](http://localhost:8000)**

*Dica: Caso o ícone da aba não carregue de imediato por conta do cache persistente do seu navegador, acesse o link acima em uma **Janela Anônima**!*

---

## 🎓 Instituição de Ensino
*   **Universidade Nove de Julho — UNINOVE**
*   **Campus**: Santo Amaro, São Paulo - SP.
*   **Projeto Acadêmico de Análise e Desenvolvimento de Sistemas**
