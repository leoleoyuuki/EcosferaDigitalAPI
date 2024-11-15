using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using EcosferaDigital.Models;

namespace EcosferaDigital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutomacaoController : ControllerBase
    {
        private readonly string _connectionString;

        public AutomacaoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        // GET: /automacao
        [HttpGet]
        public ActionResult<IEnumerable<Automacao>> GetAutomacoes()
        {
            var automacoes = new List<Automacao>();

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            using var command = new OracleCommand("SELECT * FROM Automacao", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                automacoes.Add(new Automacao
                {
                    Id = Convert.ToInt32(reader["automacao_id"]),
                    DispositivoId = Convert.ToInt32(reader["dispositivo_id"]),
                    DataHora = reader["data_hora"] as DateTime?,
                    Acao = reader["acao"]?.ToString(),
                    Motivo = reader["motivo"]?.ToString()
                });
            }

            return Ok(automacoes);
        }

        // GET: /automacao/{id}
        [HttpGet("{id}")]
        public ActionResult<Automacao> GetAutomacaoById(int id)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            using var command = new OracleCommand("SELECT * FROM Automacao WHERE automacao_id = :automacao_id", connection);
            command.Parameters.Add(new OracleParameter("automacao_id", id));

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var automacao = new Automacao
                {
                    Id = Convert.ToInt32(reader["automacao_id"]),
                    DispositivoId = Convert.ToInt32(reader["dispositivo_id"]),
                    DataHora = reader["data_hora"] as DateTime?,
                    Acao = reader["acao"]?.ToString(),
                    Motivo = reader["motivo"]?.ToString()
                };
                return Ok(automacao);
            }

            return NotFound($"Automação com ID {id} não encontrada.");
        }

        // POST: /automacao
        [HttpPost]
        public ActionResult<Automacao> CreateAutomacao([FromBody] AutomacaoPost automacaoPost)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Gerar o próximo ID manualmente
            using var command = new OracleCommand("SELECT MAX(automacao_id) FROM Automacao", connection);
            var maxId = command.ExecuteScalar();
            var novoId = (maxId == DBNull.Value) ? 1 : Convert.ToInt32(maxId) + 1;

            // Inserir a nova automação
            using var insertCommand = new OracleCommand("INSERT INTO Automacao (automacao_id, dispositivo_id, data_hora, acao, motivo) VALUES (:automacao_id, :dispositivo_id, :data_hora, :acao, :motivo)", connection);
            insertCommand.Parameters.Add(new OracleParameter("automacao_id", novoId));
            insertCommand.Parameters.Add(new OracleParameter("dispositivo_id", automacaoPost.DispositivoId));
            insertCommand.Parameters.Add(new OracleParameter("data_hora", automacaoPost.DataHora ?? (object)DBNull.Value));
            insertCommand.Parameters.Add(new OracleParameter("acao", automacaoPost.Acao ?? (object)DBNull.Value));
            insertCommand.Parameters.Add(new OracleParameter("motivo", automacaoPost.Motivo ?? (object)DBNull.Value));

            insertCommand.ExecuteNonQuery();

            var automacao = new Automacao
            {
                Id = novoId,
                DispositivoId = automacaoPost.DispositivoId,
                DataHora = automacaoPost.DataHora,
                Acao = automacaoPost.Acao,
                Motivo = automacaoPost.Motivo
            };

            return CreatedAtAction(nameof(GetAutomacaoById), new { id = automacao.Id }, automacao);
        }

        // PUT: /automacao/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateAutomacao(int id, [FromBody] AutomacaoPost automacaoPost)
        {
            if (automacaoPost == null)
                return BadRequest("Dados inválidos.");

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Verificar se a automação existe
            using var checkCommand = new OracleCommand("SELECT COUNT(*) FROM Automacao WHERE automacao_id = :automacao_id", connection);
            checkCommand.Parameters.Add(new OracleParameter("automacao_id", id));
            var exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;

            if (!exists)
            {
                return NotFound($"Automação com ID {id} não encontrada.");
            }

            // Atualizar a automação
            using var updateCommand = new OracleCommand("UPDATE Automacao SET dispositivo_id = :dispositivo_id, data_hora = :data_hora, acao = :acao, motivo = :motivo WHERE automacao_id = :automacao_id", connection);
            updateCommand.Parameters.Add(new OracleParameter("dispositivo_id", automacaoPost.DispositivoId));
            updateCommand.Parameters.Add(new OracleParameter("data_hora", automacaoPost.DataHora ?? (object)DBNull.Value));
            updateCommand.Parameters.Add(new OracleParameter("acao", automacaoPost.Acao ?? (object)DBNull.Value));
            updateCommand.Parameters.Add(new OracleParameter("motivo", automacaoPost.Motivo ?? (object)DBNull.Value));
            updateCommand.Parameters.Add(new OracleParameter("automacao_id", id));


            updateCommand.ExecuteNonQuery();

            return Ok(automacaoPost);

        }
        // DELETE: /automacao/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("DELETE FROM Automacao WHERE automacao_id = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        return NotFound($"Automação com ID {id} não encontrado.");
                }
            }

            return NoContent(); // 204
        }
    }
}
