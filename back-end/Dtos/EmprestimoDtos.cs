namespace BibliotecaApi.Dtos;

public class EmprestimoCreate
{
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public DateTime? DataEmprestimo { get; set; }
}

public class EmprestimoSchema
{
    public int IdEmprestimo { get; set; }
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public DateTime? DataEmprestimo { get; set; }
    public DateTime? DataPrazo { get; set; }
    public DateTime? DataDevolucao { get; set; }
    public string? Status { get; set; }
    public string? NomeUsuario { get; set; }
    public string? NomeLivro { get; set; }
}