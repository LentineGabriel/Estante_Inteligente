using BibliotecaApi.Core;

namespace BibliotecaApi.Dtos;

public class UsuarioCreate
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    public void Validar()
    {
        Nome = Validations.ValidarNome(Nome);
        Email = Validations.ValidarEmail(Email);
        Endereco = Validations.ValidarEndereco(Endereco);
        Telefone = Validations.ValidarTelefone(Telefone);
    }
}

public class UsuarioUpdate
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone { get; set; }

    public void Validar()
    {
        if(Nome is not null)
            Nome = Validations.ValidarNome(Nome);

        if(Email is not null)
            Email = Validations.ValidarEmail(Email);

        if(Endereco is not null)
            Endereco = Validations.ValidarEndereco(Endereco);

        if(Telefone is not null)
            Telefone = Validations.ValidarTelefone(Telefone);
    }
}

public class UsuarioSchema
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}