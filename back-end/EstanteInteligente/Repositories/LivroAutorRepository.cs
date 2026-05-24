using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class LivroAutorRepository
{
    private readonly Database _database;

    public LivroAutorRepository(Database database)
    {
        _database = database;
    }

    public LivrosAutor Criar(LivrosAutor livroAutor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO livro_autor (id_livro, id_autor)
            VALUES (@idLivro, @idAutor);
        """ , conn);

        cmd.Parameters.AddWithValue("@idLivro" , livroAutor.IdLivro!);
        cmd.Parameters.AddWithValue("@idAutor" , livroAutor.IdAutor!);

        cmd.ExecuteNonQuery();

        return livroAutor;
    }

    public List<LivrosAutor> Listar()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_livro, id_autor
            FROM livro_autor
            ORDER BY id_livro ASC, id_autor ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var lista = new List<LivrosAutor>();

        while(reader.Read())
        {
            lista.Add(new LivrosAutor
            {
                IdLivro = reader.GetInt32(0) ,
                IdAutor = reader.GetInt32(1)
            });
        }

        return lista;
    }

    public LivrosAutor? BuscarPorId(int idLivro , int idAutor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_livro, id_autor
            FROM livro_autor
            WHERE id_livro = @idLivro AND id_autor = @idAutor;
        """ , conn);

        cmd.Parameters.AddWithValue("@idLivro" , idLivro);
        cmd.Parameters.AddWithValue("@idAutor" , idAutor);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return new LivrosAutor
        {
            IdLivro = reader.GetInt32(0) ,
            IdAutor = reader.GetInt32(1)
        };
    }

    public List<LivrosAutor> ListarPorLivro(int idLivro)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_livro, id_autor
            FROM livro_autor
            WHERE id_livro = @idLivro
            ORDER BY id_autor ASC;
        """ , conn);

        cmd.Parameters.AddWithValue("@idLivro" , idLivro);

        using var reader = cmd.ExecuteReader();
        var lista = new List<LivrosAutor>();

        while(reader.Read())
        {
            lista.Add(new LivrosAutor
            {
                IdLivro = reader.GetInt32(0) ,
                IdAutor = reader.GetInt32(1)
            });
        }

        return lista;
    }

    public List<LivrosAutor> ListarPorAutor(int idAutor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_livro, id_autor
            FROM livro_autor
            WHERE id_autor = @idAutor
            ORDER BY id_livro ASC;
        """ , conn);

        cmd.Parameters.AddWithValue("@idAutor" , idAutor);

        using var reader = cmd.ExecuteReader();
        var lista = new List<LivrosAutor>();

        while(reader.Read())
        {
            lista.Add(new LivrosAutor
            {
                IdLivro = reader.GetInt32(0) ,
                IdAutor = reader.GetInt32(1)
            });
        }

        return lista;
    }

    public bool Deletar(int idLivro , int idAutor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM livro_autor
            WHERE id_livro = @idLivro AND id_autor = @idAutor;
        """ , conn);

        cmd.Parameters.AddWithValue("@idLivro" , idLivro);
        cmd.Parameters.AddWithValue("@idAutor" , idAutor);

        return cmd.ExecuteNonQuery() > 0;
    }
}