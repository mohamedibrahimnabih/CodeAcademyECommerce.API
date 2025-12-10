namespace CodeAcademyECommerce.API
{
    public class Brand : AuditLogging
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string logo { get; set; } = string.Empty;
        
    }
}
