namespace EcosferaDigital.Models
{
    public class Automacao
    {
        public int Id { get; set; }
        public int DispositivoId { get; set; }
        public DateTime? DataHora { get; set; }
        public string? Acao { get; set; }
        public string? Motivo { get; set; }

    }

    public class AutomacaoPost
    {
        public int DispositivoId { get; set; }
        public DateTime? DataHora { get; set; }
        public string? Acao { get; set; }
        public string? Motivo { get; set; }

    }
}
