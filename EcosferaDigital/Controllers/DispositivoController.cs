using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using EcosferaDigital.Models;

namespace EcosferaDigital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DispositivoController : ControllerBase
    {
        private readonly string _connectionString;

        public DispositivoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        // GET: /dispositivo
        [HttpGet]
        public IActionResult Get()
        {
            List<Dispositivo> dispositivos = new List<Dispositivo>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("SELECT dispositivo_id, usuario_id, tipo_dispositivo, descricao, status FROM Dispositivo", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dispositivos.Add(new Dispositivo
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                TipoDispositivo = reader.GetString(2),
                                Descricao = reader.GetString(3),
                                Status = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            if (dispositivos.Count == 0)
                return NotFound("Nenhum dispositivo encontrado.");

            return Ok(dispositivos);
        }

        // GET: /dispositivo/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Dispositivo dispositivo = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("SELECT dispositivo_id, usuario_id, tipo_dispositivo, descricao, status FROM Dispositivo WHERE dispositivo_id = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dispositivo = new Dispositivo
                            {
                                Id = reader.GetInt32(0),
                                UsuarioId = reader.GetInt32(1),
                                TipoDispositivo = reader.GetString(2),
                                Descricao = reader.GetString(3),
                                Status = reader.GetString(4)
                            };
                        }
                    }
                }
            }

            if (dispositivo == null)
                return NotFound($"Dispositivo com ID {id} não encontrado.");

            return Ok(dispositivo);
        }

        // POST: /dispositivo
        [HttpPost]
        public ActionResult<Dispositivo> CreateDispositivo([FromBody] DispositivoPost dispositivoPost)
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();

            // 1. Buscar o maior ID já presente no banco
            using var command = new OracleCommand("SELECT MAX(dispositivo_id) FROM Dispositivo", connection);
            var maxId = command.ExecuteScalar();
            var novoId = (maxId == DBNull.Value) ? 1 : Convert.ToInt32(maxId) + 1; // Incrementar o maior ID

            // 2. Inserir o novo dispositivo com o ID gerado manualmente
            using var insertCommand = new OracleCommand("INSERT INTO Dispositivo (dispositivo_id, usuario_id, tipo_dispositivo, descricao, status) VALUES (:dispositivo_id, :usuario_id, :tipo_dispositivo, :descricao, :status)", connection);
            insertCommand.Parameters.Add(new OracleParameter("dispositivo_id", novoId)); // dispositivo_id
            insertCommand.Parameters.Add(new OracleParameter("usuario_id", dispositivoPost.UsuarioId)); // usuario_id
            insertCommand.Parameters.Add(new OracleParameter("tipo_dispositivo", dispositivoPost.TipoDispositivo)); // tipo_dispositivo
            insertCommand.Parameters.Add(new OracleParameter("descricao", dispositivoPost.Descricao)); // descricao
            insertCommand.Parameters.Add(new OracleParameter("status", dispositivoPost.Status)); // status

            insertCommand.ExecuteNonQuery();

            // 3. Criar o dispositivo com o ID manualmente gerado
            var dispositivo = new Dispositivo
            {
                Id = novoId,
                UsuarioId = dispositivoPost.UsuarioId,
                TipoDispositivo = dispositivoPost.TipoDispositivo,
                Descricao = dispositivoPost.Descricao,
                Status = dispositivoPost.Status
            };

            // 4. Retornar o dispositivo criado com o ID gerado manualmente
            return CreatedAtAction(nameof(GetById), new { id = dispositivo.Id }, dispositivo);
        }

        // PUT: /dispositivo/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] DispositivoPost dispositivoPost)
        {
            if (dispositivoPost == null)
                return BadRequest("Dados inválidos.");

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("UPDATE Dispositivo SET usuario_id = :usuario_id, tipo_dispositivo = :tipo_dispositivo, descricao = :descricao, status = :status WHERE dispositivo_id = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("usuario_id", dispositivoPost.UsuarioId));
                    command.Parameters.Add(new OracleParameter("tipo_dispositivo", dispositivoPost.TipoDispositivo));
                    command.Parameters.Add(new OracleParameter("descricao", dispositivoPost.Descricao));
                    command.Parameters.Add(new OracleParameter("status", dispositivoPost.Status));
                    command.Parameters.Add(new OracleParameter("id", id));


                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        return NotFound($"Dispositivo com ID {id} não encontrado.");
                }
            }

            return NoContent(); // 204
        }

        // DELETE: /dispositivo/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("DELETE FROM Dispositivo WHERE dispositivo_id = :id", connection))
                {
                    command.Parameters.Add(new OracleParameter("id", id));

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        return NotFound($"Dispositivo com ID {id} não encontrado.");
                }
            }

            return NoContent(); // 204
        }
    }
}
