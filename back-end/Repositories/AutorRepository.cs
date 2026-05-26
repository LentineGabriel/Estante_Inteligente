using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class AutorRepository
{
    private readonly Database _database;

    public AutorRepository(Database database)
    {
        _database = database;
    }

    public Autor Criar(Autor autor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO autor (nome)
            VALUES (@nome)
            RETURNING id_autor;
        """ , conn);

        cmd.Parameters.AddWithValue("@nome" , autor.Nome!);
        autor.IdAutor = Convert.ToInt32(cmd.ExecuteScalar());

        return autor;
    }

    public List<Autor> Listar()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_autor, nome
            FROM autor
            ORDER BY id_autor ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var autores = new List<Autor>();

        while(reader.Read())
        {
            autores.Add(new Autor
            {
                IdAutor = reader.GetInt32(0) ,
                Nome = reader.GetString(1)
            });
        }

        return autores;
    }

    public Autor? BuscarPorId(int idAutor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_autor, nome
            FROM autor
            WHERE id_autor = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idAutor);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return new Autor
        {
            IdAutor = reader.GetInt32(0) ,
            Nome = reader.GetString(1)
        };
    }

    public bool AtualizarNome(int idAutor , string novoNome)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            UPDATE autor
            SET nome = @nome
            WHERE id_autor = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@nome" , novoNome);
        cmd.Parameters.AddWithValue("@id" , idAutor);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Deletar(int idAutor)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM autor
            WHERE id_autor = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idAutor);

        return cmd.ExecuteNonQuery() > 0;
    }
}