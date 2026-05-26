using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class UsuarioRepository
{
    private readonly Database _database;

    public UsuarioRepository(Database database)
    {
        _database = database;
    }

    public Usuario Criar(Usuario usuario)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO usuarios (nome, email, endereco, telefone)
            VALUES (@nome, @email, @endereco, @telefone)
            RETURNING id_usuario;
        """ , conn);

        cmd.Parameters.AddWithValue("@nome" , usuario.Nome!);
        cmd.Parameters.AddWithValue("@email" , usuario.Email!);
        cmd.Parameters.AddWithValue("@endereco" , usuario.Endereco ?? "");
        cmd.Parameters.AddWithValue("@telefone" , usuario.Telefone ?? "");

        usuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar());

        return usuario;
    }

    public List<Usuario> Listar()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_usuario, nome, email, endereco, telefone
            FROM usuarios
            ORDER BY id_usuario ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var usuarios = new List<Usuario>();

        while(reader.Read())
        {
            usuarios.Add(new Usuario
            {
                IdUsuario = reader.GetInt32(0) ,
                Nome = reader.GetString(1) ,
                Email = reader.GetString(2) ,
                Endereco = reader.IsDBNull(3) ? null : reader.GetString(3) ,
                Telefone = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return usuarios;
    }

    public Usuario? BuscarPorId(int idUsuario)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT id_usuario, nome, email, endereco, telefone
            FROM usuarios
            WHERE id_usuario = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idUsuario);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return new Usuario
        {
            IdUsuario = reader.GetInt32(0) ,
            Nome = reader.GetString(1) ,
            Email = reader.GetString(2) ,
            Endereco = reader.IsDBNull(3) ? null : reader.GetString(3) ,
            Telefone = reader.IsDBNull(4) ? null : reader.GetString(4)
        };
    }

    public bool AtualizarCampo(int idUsuario , string campo , object? novoValor)
    {
        var camposPermitidos = new HashSet<string> { "nome" , "email" , "endereco" , "telefone" };

        if(!camposPermitidos.Contains(campo))
            throw new ArgumentException("Campo inválido para atualização.");

        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand($"""
            UPDATE usuarios
            SET {campo} = @valor
            WHERE id_usuario = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@valor" , novoValor ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id" , idUsuario);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Deletar(int idUsuario)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM usuarios
            WHERE id_usuario = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idUsuario);

        return cmd.ExecuteNonQuery() > 0;
    }
}