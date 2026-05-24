using BibliotecaApi.Core;
using BibliotecaApi.Models;
using Npgsql;

namespace BibliotecaApi.Repositories;

public class EmprestimoRepository
{
    private readonly Database _database;

    public EmprestimoRepository(Database database)
    {
        _database = database;
    }

    public Emprestimo Criar(Emprestimo emprestimo)
    {
        emprestimo.DataEmprestimo ??= DateTime.Now;
        emprestimo.CalcularPrazo();

        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            INSERT INTO emprestimos 
                (id_usuario, id_livro, data_emprestimo, data_prazo, status)
            VALUES 
                (@idUsuario, @idLivro, @dataEmprestimo, @dataPrazo, @status)
            RETURNING id_emprestimo;
        """ , conn);

        cmd.Parameters.AddWithValue("@idUsuario" , emprestimo.IdUsuario!);
        cmd.Parameters.AddWithValue("@idLivro" , emprestimo.IdLivro!);
        cmd.Parameters.AddWithValue("@dataEmprestimo" , emprestimo.DataEmprestimo!);
        cmd.Parameters.AddWithValue("@dataPrazo" , emprestimo.DataPrazo!);
        cmd.Parameters.AddWithValue("@status" , "emprestado");

        emprestimo.IdEmprestimo = Convert.ToInt32(cmd.ExecuteScalar());
        emprestimo.Status = "emprestado";

        return emprestimo;
    }

    public List<Emprestimo> Listar()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT
                e.id_emprestimo,
                e.id_usuario,
                e.id_livro,
                e.data_emprestimo,
                e.data_prazo,
                e.data_devolucao,
                e.status,
                u.nome,
                l.nome_livro
            FROM emprestimos e
            LEFT JOIN usuarios u ON u.id_usuario = e.id_usuario
            LEFT JOIN livros l ON l.id_livro = e.id_livro
            ORDER BY e.id_emprestimo ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var lista = new List<Emprestimo>();

        while(reader.Read())
            lista.Add(MapearEmprestimo(reader));

        return lista;
    }

    public Emprestimo? BuscarPorId(int idEmprestimo)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT
                e.id_emprestimo,
                e.id_usuario,
                e.id_livro,
                e.data_emprestimo,
                e.data_prazo,
                e.data_devolucao,
                e.status,
                u.nome,
                l.nome_livro
            FROM emprestimos e
            LEFT JOIN usuarios u ON u.id_usuario = e.id_usuario
            LEFT JOIN livros l ON l.id_livro = e.id_livro
            WHERE e.id_emprestimo = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idEmprestimo);

        using var reader = cmd.ExecuteReader();

        if(!reader.Read())
            return null;

        return MapearEmprestimo(reader);
    }

    public List<Emprestimo> ListarEmprestimosAtivos()
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            SELECT
                e.id_emprestimo,
                e.id_usuario,
                e.id_livro,
                e.data_emprestimo,
                e.data_prazo,
                e.data_devolucao,
                e.status,
                u.nome,
                l.nome_livro
            FROM emprestimos e
            LEFT JOIN usuarios u ON u.id_usuario = e.id_usuario
            LEFT JOIN livros l ON l.id_livro = e.id_livro
            WHERE e.status = 'emprestado'
            ORDER BY e.data_prazo ASC;
        """ , conn);

        using var reader = cmd.ExecuteReader();
        var lista = new List<Emprestimo>();

        while(reader.Read())
            lista.Add(MapearEmprestimo(reader));

        return lista;
    }

    public bool RegistrarDevolucao(int idEmprestimo)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            UPDATE emprestimos
            SET data_devolucao = CURRENT_TIMESTAMP,
                status = 'devolvido'
            WHERE id_emprestimo = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idEmprestimo);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool MarcarComoAtrasado(int idEmprestimo)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            UPDATE emprestimos
            SET status = 'atrasado'
            WHERE id_emprestimo = @id
              AND status = 'emprestado';
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idEmprestimo);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Deletar(int idEmprestimo)
    {
        using var conn = _database.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("""
            DELETE FROM emprestimos
            WHERE id_emprestimo = @id;
        """ , conn);

        cmd.Parameters.AddWithValue("@id" , idEmprestimo);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static Emprestimo MapearEmprestimo(NpgsqlDataReader reader)
    {
        return new Emprestimo
        {
            IdEmprestimo = reader.GetInt32(0) ,
            IdUsuario = reader.GetInt32(1) ,
            IdLivro = reader.GetInt32(2) ,
            DataEmprestimo = reader.IsDBNull(3) ? null : reader.GetDateTime(3) ,
            DataPrazo = reader.IsDBNull(4) ? null : reader.GetDateTime(4) ,
            DataDevolucao = reader.IsDBNull(5) ? null : reader.GetDateTime(5) ,
            Status = reader.IsDBNull(6) ? null : reader.GetString(6) ,
            NomeUsuario = reader.IsDBNull(7) ? null : reader.GetString(7) ,
            NomeLivro = reader.IsDBNull(8) ? null : reader.GetString(8)
        };
    }
}