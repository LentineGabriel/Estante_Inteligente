using BibliotecaApi.Core;

namespace BibliotecaApi.Dtos;

public class AutorCreate
{
    public string Nome { get; set; } = string.Empty;

    public void Validar()
    {
        Nome = Validations.ValidarNome(Nome);
    }
}

public class AutorSchema
{
    public int IdAutor { get; set; }
    public string Nome { get; set; } = string.Empty;
}