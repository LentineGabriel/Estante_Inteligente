using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class EstanteRepository
{
    private readonly Database _database;

    public EstanteRepository(Database database)
    {
        _database = database;
    }

    public Estante? AdicionarOuAtualizar(int idUsuario , int idLivro , string status)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO estante 
                (id_usuario, id_livro, status, data_atualizacao)
            VALUES 
                (@idUsuario, @idLivro, @status, CURRENT_TIMESTAMP)
            ON CONFLICT (id_usuario, id_livro)
            DO UPDATE SET 
                status = EXCLUDED.status,
                data_atualizacao = CURRENT_TIMESTAMP
            RETURNING id_estante, data_atualizacao;
        """ , conn);

        cmd.Parameters.AddWithValue("@idUsuario" , idUsuario);
        cmd.Parameters.AddWithValue("@idLivro" , idLivro);
        cmd.Parameters.AddWithValue("@status" , status);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return new Estante
        {
            IdEstante = reader.GetInt32(0) ,
            IdUsuario = idUsuario ,
            IdLivro = idLivro ,
            Status = status ,
            DataAtualizacao = reader.GetDateTime(1)
        };
    }

    public List<Estante> ListarPorUsuario(int idUsuario)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT 
                e.id_estante,
                e.id_usuario,
                e.id_livro,
                e.status,
                e.data_atualizacao,
                l.nome_livro,
                u.nome
            FROM estante e
            INNER JOIN livros l ON l.id_livro = e.id_livro
            INNER JOIN usuarios u ON u.id_usuario = e.id_usuario
            WHERE e.id_usuario = @idUsuario
            ORDER BY e.data_atualizacao DESC;
        """ , conn);

        cmd.Parameters.AddWithValue("@idUsuario" , idUsuario);

        using var reader = cmd.ExecuteReader();
        var lista = new List<Estante>();

        while(reader.Read())
            lista.Add(MapearEstante(reader));

        return lista;
    }

    public bool Remover(int idUsuario , int idLivro)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM estante
            WHERE id_usuario = @idUsuario
              AND id_livro = @idLivro;
        """ , conn);

        cmd.Parameters.AddWithValue("@idUsuario" , idUsuario);
        cmd.Parameters.AddWithValue("@idLivro" , idLivro);

        return cmd.ExecuteNonQuery() > 0;
    }

    public Estante? BuscarPorUsuarioELivro(int idUsuario , int idLivro)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT 
                e.id_estante,
                e.id_usuario,
                e.id_livro,
                e.status,
                e.data_atualizacao,
                l.nome_livro,
                u.nome
            FROM estante e
            INNER JOIN livros l ON l.id_livro = e.id_livro
            INNER JOIN usuarios u ON u.id_usuario = e.id_usuario
            WHERE e.id_usuario = @idUsuario
              AND e.id_livro = @idLivro;
        """ , conn);

        cmd.Parameters.AddWithValue("@idUsuario" , idUsuario);
        cmd.Parameters.AddWithValue("@idLivro" , idLivro);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return MapearEstante(reader);
    }

    private static Estante MapearEstante(NpgsqlDataReader reader)
    {
        return new Estante
        {
            IdEstante = reader.GetInt32(0) ,
            IdUsuario = reader.GetInt32(1) ,
            IdLivro = reader.GetInt32(2) ,
            Status = reader.GetString(3) ,
            DataAtualizacao = reader.IsDBNull(4) ? null : reader.GetDateTime(4) ,
            NomeLivro = reader.IsDBNull(5) ? null : reader.GetString(5) ,
            NomeUsuario = reader.IsDBNull(6) ? null : reader.GetString(6)
        };
    }
}