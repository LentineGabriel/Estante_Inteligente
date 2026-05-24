// Models/Estante.cs
namespace BibliotecaApi.Models;

public class Estante
{
    public int? IdEstante { get; set; }
    public int? IdUsuario { get; set; }
    public int? IdLivro { get; set; }
    public string? Status { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public Usuario? Usuario { get; set; }
    public Livro? Livro { get; set; }

    public string? NomeLivro { get; set; }
    public string? NomeUsuario { get; set; }

    public override string ToString()
    {
        var partes = new List<string>
        {
            $"id_estante = {IdEstante}",
            $"status = '{Status}'"
        };

        partes.Add(!string.IsNullOrWhiteSpace(NomeLivro)
            ? $"livro = '{NomeLivro}'"
            : $"id_livro = {IdLivro}");

        partes.Add(!string.IsNullOrWhiteSpace(NomeUsuario)
            ? $"usuario = '{NomeUsuario}'"
            : $"id_usuario = {IdUsuario}");

        if(DataAtualizacao.HasValue)
            partes.Add($"atualizado_em = '{DataAtualizacao}'");

        return string.Join(", " , partes);
    }
}