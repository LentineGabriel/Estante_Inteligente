namespace BibliotecaApi.Dtos;

public class LivroAutorCreate
{
    public int IdLivro { get; set; }
    public int IdAutor { get; set; }
}

public class LivroAutorSchema : LivroAutorCreate
{
}