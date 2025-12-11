namespace CodeAcademyECommerce.API
{
    public class Category : AuditLogging
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
