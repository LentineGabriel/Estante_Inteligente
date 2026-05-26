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
*   **Formulários "Sem Rodeios"**: Se o usuário estiver cadastrando um livro e perceber que o Autor ou Editora não existem na listagem, ele pode criá-los clicando em um pequeno botão `[ + ]` direto no formulário.
*   **Validações Inteligentes Síncronas (Front + Back)**: O front-end atua de forma síncrona com o back-end, realizando validações idênticas locais imediatas ao tentar submeter cadastros.

---

## 🏗️ Arquitetura do Sistema (.NET C#)

O projeto foi migrado e estruturado utilizando as melhores práticas do ecossistema .NET:

```text
Estante_Inteligente/
├── .github/                 # Automação de pipelines DevOps (GitHub Actions)
│   └── workflows/
│       └── ci.yml           # Pipeline de integração contínua (CI) de Compilação .NET
├── back-end/                # Camada de Backend (C# .NET 10 API Minimal e PostgreSQL)
│   ├── Core/                # Configurações de Banco de Dados e Injeção de Dependência
│   ├── Dtos/                # Objetos de Transferência de Dados e Validações
│   ├── Models/              # Entidades de negócio tipadas
│   ├── Repositories/        # Persistência SQL com Npgsql (sem uso de ORM pesado)
│   ├── Program.cs           # Ponto de entrada, Minimal APIs, configuração de CORS e Swagger
│   ├── appsettings.json     # Configuração de Connection String e Log
│   └── init.sql             # Script automático de criação física de tabelas no Postgres
├── front-end/               # Camada de Apresentação (Acessível, Clean e Premium)
│   ├── index.html           # Esqueleto HTML5 da SPA estruturado com suporte a favicon
│   ├── style.css            # Folha de estilos Dark Mode, Glassmorphism e Transições
│   ├── app.js               # Lógica do lado do cliente (consumo de API e navegação)
│   └── favicon.png          # Ícone premium
└── docker-compose.yml       # Orquestrador local do Banco de Dados PostgreSQL
```

---

## 🚦 Como Executar a Aplicação

A execução deste projeto é dividida entre o Banco de Dados (via Docker) e a API (via Visual Studio).

### Passo 1: Subir o Banco de Dados (PostgreSQL)
Abra o terminal na raiz do projeto e inicie o banco de dados via Docker:
```bash
docker compose up -d
```
*Isso vai iniciar uma instância do PostgreSQL na porta 5432 utilizando a senha segura configurada no projeto.*

### Passo 2: Executar o Back-end
1. Abra a pasta `back-end` ou o arquivo `.slnx` utilizando o **Visual Studio**.
2. Clique no botão de **Run (Play)** no topo do Visual Studio.
3. O Visual Studio irá rodar o servidor em segundo plano e a API estará ativa (com HTTPS) na porta **7280**. O Swagger será exibido para testes diretos dos Endpoints.

### Passo 3: Acessar a Interface do Usuário (Front-end)
O nosso front-end é uma aplicação web construída com HTML, CSS e JavaScript puros. Para evitar problemas de bloqueio de segurança dos navegadores (erros de CORS) ao abrir os arquivos localmente, recomendamos o uso de um servidor web simples.

A forma mais prática é utilizar a extensão **Live Server** do Visual Studio Code:

1. Abra a pasta `front-end` no seu Visual Studio Code.
2. Clique com o botão direito no arquivo `index.html`.
3. Selecione a opção **"Open with Live Server"** (ou clique em "Go Live" na barra inferior).
4. O seu navegador padrão será aberto automaticamente com a interface do sistema funcionando e conectada ao seu Back-end em C#.

---

## 🎓 Instituição de Ensino
*   **Universidade Nove de Julho — UNINOVE**
*   **Campus**: Santo Amaro, São Paulo - SP.
*   **Projeto Acadêmico de Análise e Desenvolvimento de Sistemas**
