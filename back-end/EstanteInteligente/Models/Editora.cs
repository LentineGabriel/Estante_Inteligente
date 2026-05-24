// Models/Editora.cs
namespace BibliotecaApi.Models;

public class Editora
{
    public int? IdEditora { get; set; }
    public string? Nome { get; set; }

    public List<Livro> Livros { get; set; } = new();

    public override string ToString()
    {
        return $"id = {IdEditora}, nome = '{Nome}'";
    }
}