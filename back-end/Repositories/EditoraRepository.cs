using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class EditoraRepository
{
    private readonly Database _database;

    public EditoraRepository(Database database)
    {
        _database = database;
    }

    public Editora Criar(Editora editora)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO editora (nome)
            VALUES (@nome)
            RETURNING id_editora;
        """ , conn);

        cmd.Parameters.AddWithValue("@nome" , editora.Nome!);
        editora.IdEditora = Convert.ToInt32(cmd.ExecuteScalar());

        return editora;
    }

    public List<Editora> Listar()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_editora, nome
            FROM editora
            ORDER BY id_editora ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var editoras = new List<Editora>();

        while(reader.Read())
        {
            editoras.Add(new Editora
            {
                IdEditora = reader.GetInt32(0) ,
                Nome = reader.GetString(1)
            });
        }

        return editoras;
    }

    public Editora? BuscarPorId(int idEditora)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_editora, nome
            FROM editora
            WHERE id_editora = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idEditora);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return new Editora
        {
            IdEditora = reader.GetInt32(0) ,
            Nome = reader.GetString(1)
        };
    }

    public bool AtualizarNome(int idEditora , string novoNome)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            UPDATE editora
            SET nome = @nome
            WHERE id_editora = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@nome" , novoNome);
        cmd.Parameters.AddWithValue("@id" , idEditora);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Deletar(int idEditora)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM editora
            WHERE id_editora = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idEditora);

        return cmd.ExecuteNonQuery() > 0;
    }
}