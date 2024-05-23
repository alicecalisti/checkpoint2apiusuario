using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_api_usuario.Entidades;

namespace web_api_usuario.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly string? _connectionString;

        public UsuarioController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, senha from Usuario;";
            var result = await dbConnection.QueryAsync<Usuario>(sql);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, senha from Usuario WHERE id = @id;";

            var usuario = await dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            using IDbConnection dbConnection = OpenConnection();
            dbConnection.Execute("Insert into Usuario(nome,senha) values(@Nome, @Senha)", usuario);
            dbConnection.Close();
            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Usuario usuario)
        {
            using IDbConnection dbConnection = OpenConnection();

            var query = @"UPDATE Usuario SET
                        nome = @Nome,
                        senha = @Senha
                        WHERE id = @Id";
            dbConnection.Execute(query, usuario);

            return Ok();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "delete from Usuario where id = @id;";
            var usuario = await dbConnection.QueryAsync<Usuario>(sql, new { id });
            return Ok();
        }


    }
}
