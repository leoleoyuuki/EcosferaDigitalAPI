namespace EcosferaDigital.Models
{
    public class Alerta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public string? Mensagem { get; set; }
        public string? TipoAlerta { get; set; }
    }

    public class AlertaPost
    {
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public string? Mensagem { get; set; }
        public string? TipoAlerta { get; set; }
    }
}
