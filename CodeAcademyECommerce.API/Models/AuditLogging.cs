using System.ComponentModel.DataAnnotations.Schema;

namespace CodeAcademyECommerce.API
{
    public class AuditLogging
    {
        //public int Id { get; set; }
        //[Column(Order = 2)]
        public DateTime? CreateAT { get; set; } = DateTime.Now;
        public string? CreateById { get; set; }
        [ForeignKey(nameof(CreateById))]
        public ApplicationUser? ApplicationUser { get; set; }
        //[Column(Order = 3)]
        public DateTime? UpdatedAT { get; set; }
    }
}
