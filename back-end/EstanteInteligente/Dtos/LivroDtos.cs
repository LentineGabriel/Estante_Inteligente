using BibliotecaApi.Core;

namespace BibliotecaApi.Dtos;

public class LivroCreate
{
    public string NomeLivro { get; set; } = string.Empty;
    public int IdEditora { get; set; }
    public int? IdAutor { get; set; }

    public void Validar()
    {
        NomeLivro = Validations.ValidarNomeLivro(NomeLivro);
    }
}

public class LivroUpdate
{
    public string? NomeLivro { get; set; }
    public int? IdEditora { get; set; }
    public int? IdAutor { get; set; }

    public void Validar()
    {
        if(NomeLivro is not null)
            NomeLivro = Validations.ValidarNomeLivro(NomeLivro);
    }
}

public class LivroSchema
{
    public int IdLivro { get; set; }
    public string NomeLivro { get; set; } = string.Empty;
    public int? IdEditora { get; set; }
    public int? IdAutor { get; set; }
    public string? NomeAutor { get; set; }
    public string? NomeEditora { get; set; }
}