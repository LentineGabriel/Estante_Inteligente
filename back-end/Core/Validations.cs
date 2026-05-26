using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BibliotecaApi.Core;

public static class Validations
{
    public static string ValidarNome(string valor)
    {
        valor = valor?.Trim() ?? "";

        if(string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Nome é obrigatório.");

        if(valor.Length < 2)
            throw new ArgumentException("Nome deve ter pelo menos 2 caracteres.");

        return valor;
    }

    public static string ValidarNomeLivro(string valor)
    {
        valor = valor?.Trim() ?? "";

        if(string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Nome do livro é obrigatório.");

        if(valor.Length < 2)
            throw new ArgumentException("Nome do livro deve ter pelo menos 2 caracteres.");

        return valor;
    }

    public static string ValidarEmail(string valor)
    {
        valor = valor?.Trim() ?? "";

        if(string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Email é obrigatório.");

        try
        {
            _ = new MailAddress(valor);
        }
        catch
        {
            throw new ArgumentException("Email inválido.");
        }

        return valor;
    }

    public static string ValidarEndereco(string valor)
    {
        valor = valor?.Trim() ?? "";

        if(string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Endereço é obrigatório.");

        if(valor.Length < 5)
            throw new ArgumentException("Endereço deve ter pelo menos 5 caracteres.");

        return valor;
    }

    public static string ValidarTelefone(string valor)
    {
        valor = valor?.Trim() ?? "";

        if(string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Telefone é obrigatório.");

        var apenasNumeros = Regex.Replace(valor , @"\D" , "");

        if(apenasNumeros.Length < 10 || apenasNumeros.Length > 11)
            throw new ArgumentException("Telefone inválido.");

        return valor;
    }

    public static string ValidarStatusEstante(string valor)
    {
        valor = valor?.Trim().ToLower() ?? "";

        var permitidos = new[] { "lido" , "lendo" , "quero ler" };

        if(!permitidos.Contains(valor))
            throw new ArgumentException("Status inválido. Deve ser 'lido', 'lendo' ou 'quero ler'.");

        return valor;
    }
}