// Models/LivrosAutor.cs
namespace BibliotecaApi.Models;

public class LivrosAutor
{
    public int? IdLivro { get; set; }
    public int? IdAutor { get; set; }

    public Livro? Livro { get; set; }
    public Autor? Autor { get; set; }

    public override string ToString()
    {
        return $"id_livro = {IdLivro}, id_autor = {IdAutor}";
    }
}