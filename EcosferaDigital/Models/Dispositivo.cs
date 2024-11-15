namespace EcosferaDigital.Models
{
    public class Dispositivo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string? TipoDispositivo { get; set; }
        public string? Descricao { get; set; }
        public string? Status { get; set; }
    }

    public class DispositivoPost
    {
        public int UsuarioId { get; set; }
        public string? TipoDispositivo { get; set; }
        public string? Descricao { get; set; }
        public string? Status { get; set; }
    }
}
