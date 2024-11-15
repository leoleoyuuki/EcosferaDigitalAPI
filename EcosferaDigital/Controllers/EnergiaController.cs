using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using EcosferaDigital.Models;

namespace EcosferaDigital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnergiaController : ControllerBase
    {
        private readonly string _connectionString;

        public EnergiaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        // GET: /energia
        [HttpGet]
        public ActionResult<IEnumerable<Energia>> GetEnergia()
        {
            var energias = new List<Energia>();

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            using var command = new OracleCommand("SELECT * FROM Energia", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                energias.Add(new Energia
                {
                    Id = Convert.ToInt32(reader["energia_id"]),
                    DispositivoId = Convert.ToInt32(reader["dispositivo_id"]),
                    DataHora = Convert.ToDateTime(reader["data_hora"]),
                    ConsumoKWH = Convert.ToSingle(reader["consumo_kwh"]),
                    GeracaoKWH = Convert.ToSingle(reader["geracao_kwh"])
                });
            }

            return Ok(energias);
        }

        // GET: /energia/{id}
        [HttpGet("{id}")]
        public ActionResult<Energia> GetEnergiaById(int id)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            using var command = new OracleCommand("SELECT * FROM Energia WHERE energia_id = :energia_id", connection);
            command.Parameters.Add(new OracleParameter("energia_id", id));

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var energia = new Energia
                {
                    Id = Convert.ToInt32(reader["energia_id"]),
                    DispositivoId = Convert.ToInt32(reader["dispositivo_id"]),
                    DataHora = Convert.ToDateTime(reader["data_hora"]),
                    ConsumoKWH = Convert.ToSingle(reader["consumo_kwh"]),
                    GeracaoKWH = Convert.ToSingle(reader["geracao_kwh"])
                };
                return Ok(energia);
            }

            return NotFound($"Energia com ID {id} não encontrada.");
        }

        // POST: /energia
        [HttpPost]
        public ActionResult<Energia> CreateEnergia([FromBody] EnergiaPost energiaPost)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Gerar o próximo ID manualmente
            using var command = new OracleCommand("SELECT MAX(energia_id) FROM Energia", connection);
            var maxId = command.ExecuteScalar();
            var novoId = (maxId == DBNull.Value) ? 1 : Convert.ToInt32(maxId) + 1;

            // Inserir a nova energia
            using var insertCommand = new OracleCommand("INSERT INTO Energia (energia_id, dispositivo_id, data_hora, consumo_kwh, geracao_kwh) VALUES (:energia_id, :dispositivo_id, :data_hora, :consumo_kwh, :geracao_kwh)", connection);
            insertCommand.Parameters.Add(new OracleParameter("energia_id", novoId));
            insertCommand.Parameters.Add(new OracleParameter("dispositivo_id", energiaPost.DispositivoId));
            insertCommand.Parameters.Add(new OracleParameter("data_hora", energiaPost.DataHora));
            insertCommand.Parameters.Add(new OracleParameter("consumo_kwh", energiaPost.ConsumoKWH));
            insertCommand.Parameters.Add(new OracleParameter("geracao_kwh", energiaPost.GeracaoKWH));

            insertCommand.ExecuteNonQuery();

            var energia = new Energia
            {
                Id = novoId,
                DispositivoId = energiaPost.DispositivoId,
                DataHora = energiaPost.DataHora,
                ConsumoKWH = energiaPost.ConsumoKWH,
                GeracaoKWH = energiaPost.GeracaoKWH
            };

            return CreatedAtAction(nameof(GetEnergiaById), new { id = energia.Id }, energia);
        }

        // PUT: /energia/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateEnergia(int id, [FromBody] EnergiaPost energiaPost)
        {
            if (energiaPost == null)
                return BadRequest("Dados inválidos.");

            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // Verificar se a energia existe
            using var checkCommand = new OracleCommand("SELECT COUNT(*) FROM Energia WHERE energia_id = :energia_id", connection);
            checkCommand.Parameters.Add(new OracleParameter("energia_id", id));
            var exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;

            if (!exists)
            {
                return NotFound($"Energia com ID {id} não encontrada.");
            }

            // Atualizar os dados de energia
            using var updateCommand = new OracleCommand("UPDATE Energia SET dispositivo_id = :dispositivo_id, data_hora = :data_hora, consumo_kwh = :consumo_kwh, geracao_kwh = :geracao_kwh WHERE energia_id = :energia_id", connection);
            updateCommand.Parameters.Add(new OracleParameter("dispositivo_id", energiaPost.DispositivoId));
            updateCommand.Parameters.Add(new OracleParameter("data_hora", energiaPost.DataHora));
            updateCommand.Parameters.Add(new OracleParameter("consumo_kwh", energiaPost.ConsumoKWH));
            updateCommand.Parameters.Add(new OracleParameter("geracao_kwh", energiaPost.GeracaoKWH));
            updateCommand.Parameters.Add(new OracleParameter("energia_id", id));

            updateCommand.ExecuteNonQuery();

            return Ok(new Energia
            {
                Id = id,
                DispositivoId = energiaPost.DispositivoId,
                DataHora = energiaPost.DataHora,
                ConsumoKWH = energiaPost.ConsumoKWH,
                GeracaoKWH = energiaPost.GeracaoKWH
            });
        }

        // DELETE: /energia/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEnergia(int id)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            using var command = new OracleCommand("DELETE FROM Energia WHERE energia_id = :id", connection);
            command.Parameters.Add(new OracleParameter("id", id));

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
                return NotFound($"Energia com ID {id} não encontrada.");

            return NoContent(); // 204
        }
    }
}
