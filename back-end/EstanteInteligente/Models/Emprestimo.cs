// Models/Emprestimo.cs
namespace BibliotecaApi.Models;

public class Emprestimo
{
    public int? IdEmprestimo { get; set; }
    public int? IdUsuario { get; set; }
    public int? IdLivro { get; set; }
    public DateTime? DataEmprestimo { get; set; }
    public DateTime? DataPrazo { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public string? Status { get; set; }

    public Usuario? Usuario { get; set; }
    public Livro? Livro { get; set; }

    public string? NomeUsuario { get; set; }
    public string? NomeLivro { get; set; }

    public DateTime? CalcularPrazo()
    {
        if(DataEmprestimo.HasValue)
        {
            DataPrazo = DataEmprestimo.Value.AddDays(20);
            return DataPrazo;
        }

        return null;
    }

    public override string ToString()
    {
        var dataEmp = DataEmprestimo?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
        var dataPrazo = DataPrazo?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
        var dataDev = DataDevolucao?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

        var usuarioStr = !string.IsNullOrWhiteSpace(NomeUsuario)
            ? $"'{NomeUsuario}'"
            : $"id_usuario = {IdUsuario}";

        var livroStr = !string.IsNullOrWhiteSpace(NomeLivro)
            ? $"'{NomeLivro}'"
            : $"id_livro = {IdLivro}";

        return $"id = {IdEmprestimo}, usuario = {usuarioStr}, livro = {livroStr}, emprestado em = {dataEmp}, prazo = {dataPrazo}, devolvido em = {dataDev}, status = '{Status}'";
    }
}