using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using EcosferaDigital.Models;
using System.Data;

namespace EcosferaDigital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertaController : ControllerBase
    {
        private readonly string _connectionString;

        public AlertaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        // GET: /alerta
        [HttpGet]
        public ActionResult<IEnumerable<Alerta>> GetAlertas()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Comando para buscar todos os alertas
            using var command = new OracleCommand("SELECT alerta_id, usuario_id, data_hora, mensagem, tipo_alerta FROM Alerta", connection);

            using var reader = command.ExecuteReader();
            var alertas = new List<Alerta>();

            while (reader.Read())
            {
                alertas.Add(new Alerta
                {
                    Id = reader.GetInt32(0), // alerta_id
                    UsuarioId = reader.GetInt32(1), // usuario_id
                    DataHora = reader.GetDateTime(2), // data_hora
                    Mensagem = reader.IsDBNull(3) ? null : reader.GetString(3), // mensagem
                    TipoAlerta = reader.IsDBNull(4) ? null : reader.GetString(4) // tipo_alerta
                });
            }

            return Ok(alertas);
        }

        // GET: /alerta/{id}
        [HttpGet("{id}")]
        public ActionResult<Alerta> GetAlertaById(int id)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();
            using var command = new OracleCommand("SELECT alerta_id, usuario_id, data_hora, mensagem, tipo_alerta FROM Alerta WHERE alerta_id = :id", connection);
            command.Parameters.Add(new OracleParameter("id", id));

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return Ok(new Alerta
                {
                    Id = reader.GetInt32(0), // alerta_id
                    UsuarioId = reader.GetInt32(1), // usuario_id
                    DataHora = reader.GetDateTime(2), // data_hora
                    Mensagem = reader.IsDBNull(3) ? null : reader.GetString(3), // mensagem
                    TipoAlerta = reader.IsDBNull(4) ? null : reader.GetString(4) // tipo_alerta
                });
            }

            return NotFound();
        }

        // POST: /alerta
        [HttpPost]
        public ActionResult<Alerta> CreateAlerta([FromBody] AlertaPost alertaPost)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // 1. Buscar o maior ID já presente no banco
            using var command = new OracleCommand("SELECT MAX(alerta_id) FROM Alerta", connection);
            var maxId = command.ExecuteScalar();
            var novoId = (maxId == DBNull.Value) ? 1 : Convert.ToInt32(maxId) + 1; // Incrementar o maior ID

            // 2. Inserir o novo alerta com o ID gerado manualmente
            using var insertCommand = new OracleCommand("INSERT INTO Alerta (alerta_id, usuario_id, data_hora, mensagem, tipo_alerta) VALUES (:alerta_id, :usuario_id, :data_hora, :mensagem, :tipo_alerta)", connection);
            insertCommand.Parameters.Add(new OracleParameter("alerta_id", novoId)); // alerta_id
            insertCommand.Parameters.Add(new OracleParameter("usuario_id", alertaPost.UsuarioId)); // usuario_id
            insertCommand.Parameters.Add(new OracleParameter("data_hora", alertaPost.DataHora)); // data_hora
            insertCommand.Parameters.Add(new OracleParameter("mensagem", alertaPost.Mensagem )); // mensagem
            insertCommand.Parameters.Add(new OracleParameter("tipo_alerta", alertaPost.TipoAlerta )); // tipo_alerta

            insertCommand.ExecuteNonQuery();

            // 3. Criar o alerta com o ID manualmente gerado
            var alerta = new Alerta
            {
                Id = novoId,
                UsuarioId = alertaPost.UsuarioId,
                DataHora = alertaPost.DataHora,
                Mensagem = alertaPost.Mensagem,
                TipoAlerta = alertaPost.TipoAlerta
            };

            // 4. Retornar o alerta criado com o ID gerado manualmente
            return CreatedAtAction(nameof(GetAlertaById), new { id = alerta.Id }, alerta);
        }

        // PUT: /alerta/{id}
        [HttpPut("{id}")]
        public ActionResult<Alerta> UpdateAlerta(int id, [FromBody] AlertaPost alertaPost)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Atualiza o alerta com base no ID
            using var command = new OracleCommand("UPDATE Alerta SET usuario_id = :usuario_id, data_hora = :data_hora, mensagem = :mensagem, tipo_alerta = :tipo_alerta WHERE alerta_id = :id", connection);
            command.Parameters.Add(new OracleParameter("usuario_id", alertaPost.UsuarioId));
            command.Parameters.Add(new OracleParameter("data_hora", alertaPost.DataHora));
            command.Parameters.Add(new OracleParameter("mensagem", alertaPost.Mensagem ));
            command.Parameters.Add(new OracleParameter("tipo_alerta", alertaPost.TipoAlerta ));
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                return NotFound();
            }

            // Retorna o alerta atualizado
            return Ok(new Alerta
            {
                Id = id,
                UsuarioId = alertaPost.UsuarioId,
                DataHora = alertaPost.DataHora,
                Mensagem = alertaPost.Mensagem,
                TipoAlerta = alertaPost.TipoAlerta
            });
        }

        // DELETE: /alerta/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteAlerta(int id)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Comando de deleção
            using var command = new OracleCommand("DELETE FROM Alerta WHERE alerta_id = :id", connection);
            command.Parameters.Add(new OracleParameter("id", id));

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
