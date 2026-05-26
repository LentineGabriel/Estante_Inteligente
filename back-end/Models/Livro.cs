// Models/Livro.cs
namespace BibliotecaApi.Models;

public class Livro
{
    public int? IdLivro { get; set; }
    public string? NomeLivro { get; set; }
    public int? IdEditora { get; set; }

    public Editora? Editora { get; set; }
    public List<LivrosAutor> LivrosAutores { get; set; } = new();
    public List<Emprestimo> Emprestimos { get; set; } = new();
    public List<Estante> Estantes { get; set; } = new();

    public int? IdAutor { get; set; }
    public string? NomeAutor { get; set; }
    public string? NomeEditora { get; set; }

    public override string ToString()
    {
        var partes = new List<string>
        {
            $"id = {IdLivro}",
            $"nome = '{NomeLivro}'"
        };

        if(!string.IsNullOrWhiteSpace(NomeAutor))
            partes.Add($"autor = '{NomeAutor.Trim()}'");
        else if(IdAutor.HasValue)
            partes.Add($"id_autor = {IdAutor}");

        if(!string.IsNullOrWhiteSpace(NomeEditora))
            partes.Add($"editora = '{NomeEditora.Trim()}'");
        else
            partes.Add($"id_editora = {IdEditora}");

        return string.Join(", " , partes);
    }
}