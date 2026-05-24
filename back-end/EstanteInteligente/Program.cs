using BibliotecaApi.Core;
using BibliotecaApi.Dtos;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Liberado" , policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<Database>();

builder.Services.AddScoped<AutorRepository>();
builder.Services.AddScoped<EditoraRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<LivroRepository>();
builder.Services.AddScoped<LivroAutorRepository>();
builder.Services.AddScoped<EmprestimoRepository>();
builder.Services.AddScoped<EstanteRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Liberado");

// =====================
// AUTORES
// =====================

app.MapPost("/autores" , (AutorCreate dto , AutorRepository repo) =>
{
    dto.Validar();

    var autor = repo.Criar(new Autor
    {
        Nome = dto.Nome
    });

    return Results.Created($"/autores/{autor.IdAutor}" , autor);
});

app.MapGet("/autores" , (AutorRepository repo) =>
{
    return Results.Ok(repo.Listar());
});

app.MapGet("/autores/{id:int}" , (int id , AutorRepository repo) =>
{
    var autor = repo.BuscarPorId(id);
    return autor is null ? Results.NotFound() : Results.Ok(autor);
});

app.MapPut("/autores/{id:int}" , (int id , AutorCreate dto , AutorRepository repo) =>
{
    dto.Validar();

    var atualizado = repo.AtualizarNome(id , dto.Nome);
    return atualizado ? Results.Ok(new { mensagem = "Autor atualizado com sucesso." }) : Results.NotFound();
});

app.MapDelete("/autores/{id:int}" , (int id , AutorRepository repo) =>
{
    var removido = repo.Deletar(id);
    return removido ? Results.Ok(new { mensagem = "Autor removido com sucesso." }) : Results.NotFound();
});

// =====================
// EDITORAS
// =====================

app.MapPost("/editoras" , (EditoraCreate dto , EditoraRepository repo) =>
{
    dto.Validar();

    var editora = repo.Criar(new Editora
    {
        Nome = dto.Nome
    });

    return Results.Created($"/editoras/{editora.IdEditora}" , editora);
});

app.MapGet("/editoras" , (EditoraRepository repo) =>
{
    return Results.Ok(repo.Listar());
});

app.MapGet("/editoras/{id:int}" , (int id , EditoraRepository repo) =>
{
    var editora = repo.BuscarPorId(id);
    return editora is null ? Results.NotFound() : Results.Ok(editora);
});

app.MapPut("/editoras/{id:int}" , (int id , EditoraCreate dto , EditoraRepository repo) =>
{
    dto.Validar();

    var atualizado = repo.AtualizarNome(id , dto.Nome);
    return atualizado ? Results.Ok(new { mensagem = "Editora atualizada com sucesso." }) : Results.NotFound();
});

app.MapDelete("/editoras/{id:int}" , (int id , EditoraRepository repo) =>
{
    var removido = repo.Deletar(id);
    return removido ? Results.Ok(new { mensagem = "Editora removida com sucesso." }) : Results.NotFound();
});

// =====================
// USUÁRIOS
// =====================

app.MapPost("/usuarios" , (UsuarioCreate dto , UsuarioRepository repo) =>
{
    dto.Validar();

    var usuario = repo.Criar(new Usuario
    {
        Nome = dto.Nome ,
        Email = dto.Email ,
        Endereco = dto.Endereco ,
        Telefone = dto.Telefone
    });

    return Results.Created($"/usuarios/{usuario.IdUsuario}" , usuario);
});

app.MapGet("/usuarios" , (UsuarioRepository repo) =>
{
    return Results.Ok(repo.Listar());
});

app.MapGet("/usuarios/{id:int}" , (int id , UsuarioRepository repo) =>
{
    var usuario = repo.BuscarPorId(id);
    return usuario is null ? Results.NotFound() : Results.Ok(usuario);
});

app.MapPut("/usuarios/{id:int}" , (int id , UsuarioUpdate dto , UsuarioRepository repo) =>
{
    dto.Validar();

    var alterou = false;

    if(dto.Nome is not null)
        alterou |= repo.AtualizarCampo(id , "nome" , dto.Nome);

    if(dto.Email is not null)
        alterou |= repo.AtualizarCampo(id , "email" , dto.Email);

    if(dto.Endereco is not null)
        alterou |= repo.AtualizarCampo(id , "endereco" , dto.Endereco);

    if(dto.Telefone is not null)
        alterou |= repo.AtualizarCampo(id , "telefone" , dto.Telefone);

    return alterou ? Results.Ok(new { mensagem = "Usuário atualizado com sucesso." }) : Results.NotFound();
});

app.MapDelete("/usuarios/{id:int}" , (int id , UsuarioRepository repo) =>
{
    var removido = repo.Deletar(id);
    return removido ? Results.Ok(new { mensagem = "Usuário removido com sucesso." }) : Results.NotFound();
});

// =====================
// LIVROS
// =====================

app.MapPost("/livros" , (LivroCreate dto , LivroRepository repo) =>
{
    dto.Validar();

    var livro = repo.Criar(new Livro
    {
        NomeLivro = dto.NomeLivro ,
        IdEditora = dto.IdEditora ,
        IdAutor = dto.IdAutor
    });

    return Results.Created($"/livros/{livro.IdLivro}" , livro);
});

app.MapGet("/livros" , (LivroRepository repo) =>
{
    return Results.Ok(repo.Listar());
});

app.MapGet("/livros/{id:int}" , (int id , LivroRepository repo) =>
{
    var livro = repo.BuscarPorId(id);
    return livro is null ? Results.NotFound() : Results.Ok(livro);
});

app.MapPut("/livros/{id:int}" , (int id , LivroUpdate dto , LivroRepository repo) =>
{
    dto.Validar();

    var alterou = false;

    if(dto.NomeLivro is not null)
        alterou |= repo.AtualizarCampo(id , "nome_livro" , dto.NomeLivro);

    if(dto.IdEditora is not null)
        alterou |= repo.AtualizarCampo(id , "id_editora" , dto.IdEditora);

    if(dto.IdAutor is not null)
        alterou |= repo.AtualizarCampo(id , "id_autor" , dto.IdAutor);

    return alterou ? Results.Ok(new { mensagem = "Livro atualizado com sucesso." }) : Results.NotFound();
});

app.MapDelete("/livros/{id:int}" , (int id , LivroRepository repo) =>
{
    var removido = repo.Deletar(id);
    return removido ? Results.Ok(new { mensagem = "Livro removido com sucesso." }) : Results.NotFound();
});

// =====================
// EMPRÉSTIMOS
// =====================

app.MapPost("/emprestimos" , (EmprestimoCreate dto , EmprestimoRepository repo) =>
{
    var emprestimo = repo.Criar(new Emprestimo
    {
        IdUsuario = dto.IdUsuario ,
        IdLivro = dto.IdLivro ,
        DataEmprestimo = dto.DataEmprestimo
    });

    return Results.Created($"/emprestimos/{emprestimo.IdEmprestimo}" , emprestimo);
});

app.MapGet("/emprestimos" , (EmprestimoRepository repo) =>
{
    return Results.Ok(repo.Listar());
});

app.MapGet("/emprestimos/ativos" , (EmprestimoRepository repo) =>
{
    return Results.Ok(repo.ListarEmprestimosAtivos());
});

app.MapGet("/emprestimos/{id:int}" , (int id , EmprestimoRepository repo) =>
{
    var emprestimo = repo.BuscarPorId(id);
    return emprestimo is null ? Results.NotFound() : Results.Ok(emprestimo);
});

app.MapPut("/emprestimos/{id:int}/devolver" , (int id , EmprestimoRepository repo) =>
{
    var atualizado = repo.RegistrarDevolucao(id);
    return atualizado ? Results.Ok(new { mensagem = "Devolução registrada com sucesso." }) : Results.NotFound();
});

app.MapPut("/emprestimos/{id:int}/atrasado" , (int id , EmprestimoRepository repo) =>
{
    var atualizado = repo.MarcarComoAtrasado(id);
    return atualizado ? Results.Ok(new { mensagem = "Empréstimo marcado como atrasado." }) : Results.NotFound();
});

app.MapDelete("/emprestimos/{id:int}" , (int id , EmprestimoRepository repo) =>
{
    var removido = repo.Deletar(id);
    return removido ? Results.Ok(new { mensagem = "Empréstimo removido com sucesso." }) : Results.NotFound();
});

// =====================
// ESTANTE
// =====================

app.MapPost("/estante" , (EstanteCreate dto , EstanteRepository repo) =>
{
    dto.Validar();

    var item = repo.AdicionarOuAtualizar(dto.IdUsuario , dto.IdLivro , dto.Status);

    return item is null
        ? Results.BadRequest(new { erro = "Não foi possível adicionar o livro à estante." })
        : Results.Ok(item);
});

app.MapGet("/estante/usuario/{idUsuario:int}" , (int idUsuario , EstanteRepository repo) =>
{
    return Results.Ok(repo.ListarPorUsuario(idUsuario));
});

app.MapGet("/estante/usuario/{idUsuario:int}/livro/{idLivro:int}" , (int idUsuario , int idLivro , EstanteRepository repo) =>
{
    var item = repo.BuscarPorUsuarioELivro(idUsuario , idLivro);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapDelete("/estante/usuario/{idUsuario:int}/livro/{idLivro:int}" , (int idUsuario , int idLivro , EstanteRepository repo) =>
{
    var removido = repo.Remover(idUsuario , idLivro);
    return removido ? Results.Ok(new { mensagem = "Livro removido da estante." }) : Results.NotFound();
});

app.Run();