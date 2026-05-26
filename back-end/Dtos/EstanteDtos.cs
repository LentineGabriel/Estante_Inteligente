using BibliotecaApi.Core;

namespace BibliotecaApi.Dtos;

public class EstanteCreate
{
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public string Status { get; set; } = string.Empty;

    public void Validar()
    {
        Status = Validations.ValidarStatusEstante(Status);
    }
}

public class EstanteSchema
{
    public int IdEstante { get; set; }
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DataAtualizacao { get; set; }
    public string? NomeLivro { get; set; }
    public string? NomeUsuario { get; set; }
}