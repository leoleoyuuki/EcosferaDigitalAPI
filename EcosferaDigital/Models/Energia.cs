namespace EcosferaDigital.Models
{
    public class Energia
    {
        public int Id { get; set; }
        public int DispositivoId { get; set; }
        public DateTime DataHora { get; set; }
        public float ConsumoKWH { get; set; }
        public float GeracaoKWH { get; set; }
    }

    public class EnergiaPost
    {
        public int DispositivoId { get; set; }
        public DateTime DataHora { get; set; }
        public float ConsumoKWH { get; set; }
        public float GeracaoKWH { get; set; }
    }

}
