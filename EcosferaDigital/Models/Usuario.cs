namespace EcosferaDigital.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Endereco { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }

    public class UsuarioPost
    {
        public string? Nome { get; set; }
        public string? Endereco { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}
