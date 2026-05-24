using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class LivroRepository
{
    private readonly Database _database;

    private const string ColunasLivroComAutorEditora = """
        l.id_livro,
        l.nome_livro,
        l.id_editora,
        (
            SELECT MIN(la2.id_autor)
            FROM livro_autor la2
            WHERE la2.id_livro = l.id_livro
        ) AS id_autor,
        (
            SELECT STRING_AGG(a2.nome, ', ' ORDER BY a2.id_autor)
            FROM livro_autor la2
            INNER JOIN autor a2 ON a2.id_autor = la2.id_autor
            WHERE la2.id_livro = l.id_livro
        ) AS nome_autor,
        ed.nome AS nome_editora
    """;

    public LivroRepository(Database database)
    {
        _database = database;
    }

    public Livro Criar(Livro livro)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var transaction = conn.BeginTransaction();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO livros (nome_livro, id_editora)
            VALUES (@nome, @idEditora)
            RETURNING id_livro;
        """ , conn , transaction);

        cmd.Parameters.AddWithValue("@nome" , livro.NomeLivro!);
        cmd.Parameters.AddWithValue("@idEditora" , livro.IdEditora ?? (object)DBNull.Value);

        livro.IdLivro = Convert.ToInt32(cmd.ExecuteScalar());

        if(livro.IdAutor is not null)
        {
            using var cmdAutor = new NpgsqlCommand("""
                INSERT INTO livro_autor (id_livro, id_autor)
                VALUES (@idLivro, @idAutor);
            """ , conn , transaction);

            cmdAutor.Parameters.AddWithValue("@idLivro" , livro.IdLivro.Value);
            cmdAutor.Parameters.AddWithValue("@idAutor" , livro.IdAutor.Value);
            cmdAutor.ExecuteNonQuery();
        }

        transaction.Commit();
        return livro;
    }

    public List<Livro> Listar()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand($"""
            SELECT {ColunasLivroComAutorEditora}
            FROM livros l
            LEFT JOIN editora ed ON ed.id_editora = l.id_editora
            ORDER BY l.id_livro ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var livros = new List<Livro>();

        while(reader.Read())
            livros.Add(MapearLivro(reader));

        return livros;
    }

    public Livro? BuscarPorId(int idLivro)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand($"""
            SELECT {ColunasLivroComAutorEditora}
            FROM livros l
            LEFT JOIN editora ed ON ed.id_editora = l.id_editora
            WHERE l.id_livro = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idLivro);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return MapearLivro(reader);
    }

    public bool AtualizarCampo(int idLivro , string campo , object? novoValor)
    {
        var camposPermitidos = new HashSet<string> { "nome_livro" , "id_editora" , "id_autor" };

        if(!camposPermitidos.Contains(campo))
            throw new ArgumentException("Campo inválido.");

        using var conn = _database.GetConnection();
        conn.Open();

        using var transaction = conn.BeginTransaction();

        if(campo == "id_autor")
        {
            using var deleteCmd = new NpgsqlCommand("""
                DELETE FROM livro_autor
                WHERE id_livro = @idLivro;
            """ , conn , transaction);

            deleteCmd.Parameters.AddWithValue("@idLivro" , idLivro);
            deleteCmd.ExecuteNonQuery();

            if(novoValor is not null)
            {
                using var insertCmd = new NpgsqlCommand("""
                    INSERT INTO livro_autor (id_livro, id_autor)
                    VALUES (@idLivro, @idAutor);
                """ , conn , transaction);

                insertCmd.Parameters.AddWithValue("@idLivro" , idLivro);
                insertCmd.Parameters.AddWithValue("@idAutor" , novoValor);
                insertCmd.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }

        using var cmd = new NpgsqlCommand($"""
            UPDATE livros
            SET {campo} = @valor
            WHERE id_livro = @id;
        """ , conn , transaction);

        cmd.Parameters.AddWithValue("@valor" , novoValor ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id" , idLivro);

        var atualizado = cmd.ExecuteNonQuery() > 0;

        transaction.Commit();
        return atualizado;
    }

    public bool Deletar(int idLivro)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM livros
            WHERE id_livro = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idLivro);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static Livro MapearLivro(NpgsqlDataReader reader)
    {
        return new Livro
        {
            IdLivro = reader.GetInt32(0) ,
            NomeLivro = reader.GetString(1) ,
            IdEditora = reader.IsDBNull(2) ? null : reader.GetInt32(2) ,
            IdAutor = reader.IsDBNull(3) ? null : reader.GetInt32(3) ,
            NomeAutor = reader.IsDBNull(4) ? null : reader.GetString(4) ,
            NomeEditora = reader.IsDBNull(5) ? null : reader.GetString(5)
        };
    }
}