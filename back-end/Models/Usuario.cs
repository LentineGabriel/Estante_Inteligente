// Models/Usuario.cs
namespace BibliotecaApi.Models;

public class Usuario
{
    public int? IdUsuario { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }

    public List<Emprestimo> Emprestimos { get; set; } = new();
    public List<Estante> Estante { get; set; } = new();

    public override string ToString()
    {
        return $"id = {IdUsuario}, nome = '{Nome}', email = '{Email}', endereco = '{Endereco}', telefone = '{Telefone}'";
    }
}