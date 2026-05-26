// Models/Autor.cs
namespace BibliotecaApi.Models;

public class Autor
{
    public int? IdAutor { get; set; }
    public string? Nome { get; set; }

    public List<LivrosAutor> LivrosAutores { get; set; } = new();

    public override string ToString()
    {
        return $"id = {IdAutor}, nome = '{Nome}'";
    }
}