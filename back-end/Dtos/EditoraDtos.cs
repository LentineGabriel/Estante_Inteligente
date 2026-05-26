using BibliotecaApi.Core;

namespace BibliotecaApi.Dtos;

public class EditoraCreate
{
    public string Nome { get; set; } = string.Empty;

    public void Validar()
    {
        Nome = Validations.ValidarNome(Nome);
    }
}

public class EditoraSchema
{
    public int IdEditora { get; set; }
    public string Nome { get; set; } = string.Empty;
}